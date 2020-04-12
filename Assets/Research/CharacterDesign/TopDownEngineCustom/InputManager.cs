using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.CharacterDesign.TopDownEngineCustom
{	
	/// <summary>
	/// This persistent singleton handles the inputs and sends commands to the player.
	/// IMPORTANT : this script's Execution Order MUST be -100.
	/// You can define a script's execution order by clicking on the script's file and then clicking on the Execution Order button at the bottom right of the script's inspector.
	/// See https://docs.unity3d.com/Manual/class-ScriptExecution.html for more details
	/// </summary>
	[AddComponentMenu("TopDown Engine/Managers/Input ManagerResearch")]
	public class InputManager : MMSingleton<InputManager>
	{
		[Header("Status")]
		/// set this to false to prevent input to be detected
		public bool InputDetectionActive = true;

        /// a string identifying the target player(s). You'll need to set this exact same string on your Character, and set its type to Player
        public string PlayerID => GetComponent<Character>().PlayerID;


		/// the possible modes for this input manager
		public enum InputForcedModes { None, Mobile, Desktop }
		/// the possible kinds of control used for movement
		public enum MovementControls { Joystick, Arrows }
		[Header("Mobile controls")]
		[MMInformation("If you check Auto Mobile Detection, the engine will automatically switch to mobile controls when your build target is Android or iOS. You can also force mobile or desktop (keyboard, gamepad) controls using the dropdown below.\nNote that if you don't need mobile controls and/or GUI this component can also work on its own, just put it on an empty GameObject instead.",MMInformationAttribute.InformationType.Info,false)]
		/// if this is set to true, the InputManager will try to detect what mode it should be in, based on the current target device
		public bool AutoMobileDetection = true;
		/// use this to force desktop (keyboard, pad) or mobile (touch) mode
		public InputForcedModes InputForcedMode;
        /// if this is true, the weapon mode will be forced to the selected WeaponForcedMode
        public bool ForceWeaponMode = false;
        /// use this to force a control mode for weapons
        [MMCondition("ForceWeaponMode", true)]
        public WeaponAim.AimControls WeaponForcedMode;
		/// if this is true, mobile controls will be hidden in editor mode, regardless of the current build target or the forced mode
		public bool HideMobileControlsInEditor = false;
		/// use this to specify whether you want to use the default joystick or arrows to move your character
		public MovementControls MovementControl = MovementControls.Joystick;
		/// if this is true, we're currently in mobile mode
		public bool IsMobile { get; protected set; }

		[Header("Movement settings")]
		[MMInformation("Turn SmoothMovement on to have inertia in your controls (meaning there'll be a small delay between a press/release of a direction and your character moving/stopping). You can also define here the horizontal and vertical thresholds.",MMInformationAttribute.InformationType.Info,false)]
		/// If set to true, acceleration / deceleration will take place when moving / stopping
		public bool SmoothMovement=true;
		/// the minimum horizontal and vertical value you need to reach to trigger movement on an analog controller (joystick for example)
		public Vector2 Threshold = new Vector2(0.1f, 0.4f);

		/// the jump button, used for jumps and validation
		public MMInput.IMButton JumpButton { get; protected set; }
		/// the run button
		public MMInput.IMButton RunButton { get; protected set; }
		/// the dash button
		public MMInput.IMButton DashButton { get; protected set; }
		/// the crouch button
		public MMInput.IMButton CrouchButton { get; protected set; }
		/// the shoot button
		public MMInput.IMButton ShootButton { get; protected set; }
        /// the activate button, used for interactions with zones
        public MMInput.IMButton InteractButton { get; protected set; }
        /// the shoot button
        public MMInput.IMButton SecondaryShootButton { get; protected set; }
        /// the reload button
        public MMInput.IMButton ReloadButton { get; protected set; }
        /// the pause button
        public MMInput.IMButton PauseButton { get; protected set; }
        /// the time control button
        public MMInput.IMButton TimeControlButton { get; protected set; }
        /// the button used to switch character (either via model or prefab switch)
        public MMInput.IMButton SwitchCharacterButton { get; protected set; }
        /// the switch weapon button
        public MMInput.IMButton SwitchWeaponButton { get; protected set; }
		/// the shoot axis, used as a button (non analogic)
		public MMInput.ButtonStates ShootAxis { get; protected set; }
        /// the shoot axis, used as a button (non analogic)
        public MMInput.ButtonStates SecondaryShootAxis { get; protected set; }
        /// the primary movement value (used to move the character around)
        public virtual Vector2 PrimaryMovement { get { return _primaryMovement; } }
        /// the secondary movement (usually the right stick on a gamepad), used to aim
        public virtual Vector2 SecondaryMovement { get { return _secondaryMovement; } }
        /// the primary movement value (used to move the character around)
        public Vector2 LastNonNullPrimaryMovement { get; set; }
        /// the secondary movement (usually the right stick on a gamepad), used to aim
        public Vector2 LastNonNullSecondaryMovement { get; set; }

        protected List<MMInput.IMButton> ButtonList;
		protected Vector2 _primaryMovement = Vector2.zero;
		protected Vector2 _secondaryMovement = Vector2.zero;
		protected string _axisHorizontal;
		protected string _axisVertical;
		protected string _axisSecondaryHorizontal;
		protected string _axisSecondaryVertical;
		protected string _axisShoot;
        protected string _axisShootSecondary;
        
        
        public virtual MMInput.ButtonStates ReloadButtonState => ReloadButton.State.CurrentState;

        public virtual MMInput.ButtonStates ShootButtonState => ShootButton.State.CurrentState;
		
        public virtual MMInput.ButtonStates SecondaryShootButtonState => SecondaryShootButton.State.CurrentState;

        /// <summary>
        /// On Start we look for what mode to use, and initialize our axis and buttons
        /// </summary>
        protected virtual void Start()
		{
			ControlsModeDetection();
			InitializeButtons ();
			InitializeAxis();
		}

		/// <summary>
		/// Turns mobile controls on or off depending on what's been defined in the inspector, and what target device we're on
		/// </summary>
		public virtual void ControlsModeDetection()
		{
			if (GUIManager.Instance!=null)
			{
				GUIManager.Instance.SetMobileControlsActive(false);
				IsMobile=false;
				if (AutoMobileDetection)
				{
					#if UNITY_ANDROID || UNITY_IPHONE
					GUIManager.Instance.SetMobileControlsActive(true,MovementControl);
					IsMobile = true;
					#endif
				}
				if (InputForcedMode==InputForcedModes.Mobile)
				{
					// GUIManager.Instance.SetMobileControlsActive(true,MovementControl);
					// IsMobile = true;
				}
				if (InputForcedMode==InputForcedModes.Desktop)
				{
					GUIManager.Instance.SetMobileControlsActive(false);
					IsMobile = false;					
				}
				if (HideMobileControlsInEditor)
				{
					#if UNITY_EDITOR
					GUIManager.Instance.SetMobileControlsActive(false);
					IsMobile = false;	
					#endif
				}
			}
		}

		/// <summary>
		/// Initializes the buttons. If you want to add more buttons, make sure to register them here.
		/// </summary>
		protected virtual void InitializeButtons()
		{
			ButtonList = new List<MMInput.IMButton> ();
			ButtonList.Add(JumpButton = new MMInput.IMButton (PlayerID, "Jump", JumpButtonDown, JumpButtonPressed, JumpButtonUp));
			ButtonList.Add(RunButton  = new MMInput.IMButton (PlayerID, "Run", RunButtonDown, RunButtonPressed, RunButtonUp));
            ButtonList.Add(InteractButton = new MMInput.IMButton(PlayerID, "Interact", InteractButtonDown, InteractButtonPressed, InteractButtonUp));
            ButtonList.Add(DashButton  = new MMInput.IMButton (PlayerID, "Dash", DashButtonDown, DashButtonPressed, DashButtonUp));
			ButtonList.Add(CrouchButton  = new MMInput.IMButton (PlayerID, "Crouch", CrouchButtonDown, CrouchButtonPressed, CrouchButtonUp));
            ButtonList.Add(SecondaryShootButton = new MMInput.IMButton(PlayerID, "SecondaryShoot", SecondaryShootButtonDown, SecondaryShootButtonPressed, SecondaryShootButtonUp));
            ButtonList.Add(ShootButton = new MMInput.IMButton (PlayerID, "Shoot", ShootButtonDown, ShootButtonPressed, ShootButtonUp)); 
			ButtonList.Add(ReloadButton = new MMInput.IMButton (PlayerID, "Reload", ReloadButtonDown, ReloadButtonPressed, ReloadButtonUp));
			ButtonList.Add(SwitchWeaponButton = new MMInput.IMButton (PlayerID, "SwitchWeapon", SwitchWeaponButtonDown, SwitchWeaponButtonPressed, SwitchWeaponButtonUp));
            ButtonList.Add(PauseButton = new MMInput.IMButton(PlayerID, "Pause", PauseButtonDown, PauseButtonPressed, PauseButtonUp));
            ButtonList.Add(TimeControlButton = new MMInput.IMButton(PlayerID, "TimeControl", TimeControlButtonDown, TimeControlButtonPressed, TimeControlButtonUp));
            ButtonList.Add(SwitchCharacterButton = new MMInput.IMButton(PlayerID, "SwitchCharacter", SwitchCharacterButtonDown, SwitchCharacterButtonPressed, SwitchCharacterButtonUp));
        }

        /// <summary>
        /// Initializes the axis strings.
        /// </summary>
        protected virtual void InitializeAxis()
		{
			_axisHorizontal = PlayerID+"_Horizontal";
			_axisVertical = PlayerID+"_Vertical";
			_axisSecondaryHorizontal = PlayerID+"_SecondaryHorizontal";
			_axisSecondaryVertical = PlayerID+"_SecondaryVertical";
			_axisShoot = PlayerID+"_ShootAxis";
            _axisShootSecondary = PlayerID + "_SecondaryShootAxis";
        }

		/// <summary>
		/// On LateUpdate, we process our button states
		/// </summary>
		protected virtual void LateUpdate()
		{
			ProcessButtonStates();
		}

		/// <summary>
		/// At update, we check the various commands and update our values and states accordingly.
		/// </summary>
		protected virtual void Update()
		{
            if (!IsMobile && InputDetectionActive)
			{	
				SetMovement();	
				SetSecondaryMovement ();
				SetShootAxis ();
				GetInputButtons ();
                GetLastNonNullValues();
			}									
		}

        /// <summary>
        /// Gets the last non null values for both primary and secondary axis
        /// </summary>
        protected virtual void GetLastNonNullValues()
        {
            if (_primaryMovement.magnitude > Threshold.x)
            {
                LastNonNullPrimaryMovement = _primaryMovement;
            }
            if (_secondaryMovement.magnitude > Threshold.x)
            {
                LastNonNullSecondaryMovement = _secondaryMovement;
            }
        }

        /// <summary>
        /// If we're not on mobile, watches for input changes, and updates our buttons states accordingly
        /// </summary>
        protected virtual void GetInputButtons()
		{
			foreach(MMInput.IMButton button in ButtonList)
			{
				if (Input.GetButton(button.ButtonID))
				{
					button.TriggerButtonPressed ();
				}
				if (Input.GetButtonDown(button.ButtonID))
				{
					button.TriggerButtonDown ();
				}
				if (Input.GetButtonUp(button.ButtonID))
				{
					button.TriggerButtonUp ();
				}
			}
		}

		/// <summary>
		/// Called at LateUpdate(), this method processes the button states of all registered buttons
		/// </summary>
		public virtual void ProcessButtonStates()
		{
			// for each button, if we were at ButtonDown this frame, we go to ButtonPressed. If we were at ButtonUp, we're now Off
			foreach (MMInput.IMButton button in ButtonList)
			{
				if (button.State.CurrentState == MMInput.ButtonStates.ButtonDown)
				{
					button.State.ChangeState(MMInput.ButtonStates.ButtonPressed);				
				}	
				if (button.State.CurrentState == MMInput.ButtonStates.ButtonUp)
				{
					button.State.ChangeState(MMInput.ButtonStates.Off);				
				}	
			}
		}

		/// <summary>
		/// Called every frame, if not on mobile, gets primary movement values from input
		/// </summary>
		public virtual void SetMovement()
		{
			if (!IsMobile && InputDetectionActive)
			{
				if (SmoothMovement)
				{
					_primaryMovement.x = Input.GetAxis(_axisHorizontal);
					_primaryMovement.y = Input.GetAxis(_axisVertical);		
				}
				else
				{
					_primaryMovement.x = Input.GetAxisRaw(_axisHorizontal);
					_primaryMovement.y = Input.GetAxisRaw(_axisVertical);		
				}
			}
		}

		/// <summary>
		/// Called every frame, if not on mobile, gets secondary movement values from input
		/// </summary>
		protected void SetSecondaryMovement()
		{
			if (!IsMobile && InputDetectionActive)
			{
				if (SmoothMovement)
				{
					_secondaryMovement.x = Input.GetAxis(_axisSecondaryHorizontal);
					_secondaryMovement.y = Input.GetAxis(_axisSecondaryVertical);		
				}
				else
				{
					_secondaryMovement.x = Input.GetAxisRaw(_axisSecondaryHorizontal);
					_secondaryMovement.y = Input.GetAxisRaw(_axisSecondaryVertical);		
				}
			}
		}

		/// <summary>
		/// Called every frame, if not on mobile, gets shoot axis values from input
		/// </summary>
		protected virtual void SetShootAxis()
		{
			if (!IsMobile && InputDetectionActive)
			{
				ShootAxis = MMInput.ProcessAxisAsButton (_axisShoot, Threshold.y, ShootAxis);
                SecondaryShootAxis = MMInput.ProcessAxisAsButton(_axisShootSecondary, -Threshold.y, SecondaryShootAxis, MMInput.AxisTypes.Negative);
            }
		}

		/// <summary>
		/// If you're using a touch joystick, bind your main joystick to this method
		/// </summary>
		/// <param name="movement">Movement.</param>
		public virtual void SetMovement(Vector2 movement)
		{
			if (IsMobile && InputDetectionActive)
			{
				_primaryMovement.x = movement.x;
				_primaryMovement.y = movement.y;	
			}
		}

		/// <summary>
		/// If you're using a touch joystick, bind your secondary joystick to this method
		/// </summary>
		/// <param name="movement">Movement.</param>
		public virtual void SetSecondaryMovement(Vector2 movement)
		{
			if (IsMobile && InputDetectionActive)
			{
				_secondaryMovement.x = movement.x;
				_secondaryMovement.y = movement.y;	
			}
		}

		/// <summary>
		/// If you're using touch arrows, bind your left/right arrows to this method
		/// </summary>
		/// <param name="">.</param>
		public virtual void SetHorizontalMovement(float horizontalInput)
		{
			if (IsMobile && InputDetectionActive)
			{
				_primaryMovement.x = horizontalInput;
			}
		}

		/// <summary>
		/// If you're using touch arrows, bind your secondary down/up arrows to this method
		/// </summary>
		/// <param name="">.</param>
		public virtual void SetVerticalMovement(float verticalInput)
		{
			if (IsMobile && InputDetectionActive)
			{
				_primaryMovement.y = verticalInput;
			}
		}

		/// <summary>
		/// If you're using touch arrows, bind your secondary left/right arrows to this method
		/// </summary>
		/// <param name="">.</param>
		public virtual void SetSecondaryHorizontalMovement(float horizontalInput)
		{
			if (IsMobile && InputDetectionActive)
			{
				_secondaryMovement.x = horizontalInput;
			}
		}

		/// <summary>
		/// If you're using touch arrows, bind your down/up arrows to this method
		/// </summary>
		/// <param name="">.</param>
		public virtual void SetSecondaryVerticalMovement(float verticalInput)
		{
			if (IsMobile && InputDetectionActive)
			{
				_secondaryMovement.y = verticalInput;
			}
		}

		public virtual void JumpButtonDown()		{ JumpButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void JumpButtonPressed()		{ JumpButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void JumpButtonUp()			{ JumpButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

		public virtual void DashButtonDown()		{ DashButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void DashButtonPressed()		{ DashButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void DashButtonUp()			{ DashButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

		public virtual void CrouchButtonDown()		{ CrouchButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void CrouchButtonPressed()	{ CrouchButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void CrouchButtonUp()		{ CrouchButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

		public virtual void RunButtonDown()			{ RunButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void RunButtonPressed()		{ RunButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void RunButtonUp()			{ RunButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

		public virtual void ReloadButtonDown()		{ ReloadButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void ReloadButtonPressed()	{ ReloadButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void ReloadButtonUp()		{ ReloadButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

        public virtual void InteractButtonDown() { InteractButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
        public virtual void InteractButtonPressed() { InteractButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
        public virtual void InteractButtonUp() { InteractButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

        public virtual void ShootButtonDown()		{ ShootButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void ShootButtonPressed()	{ ShootButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void ShootButtonUp()			{ ShootButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

        public virtual void SecondaryShootButtonDown() { SecondaryShootButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
        public virtual void SecondaryShootButtonPressed() { SecondaryShootButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
        public virtual void SecondaryShootButtonUp() { SecondaryShootButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

        public virtual void PauseButtonDown() { PauseButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
        public virtual void PauseButtonPressed() { PauseButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
        public virtual void PauseButtonUp() { PauseButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

        public virtual void TimeControlButtonDown() { TimeControlButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
        public virtual void TimeControlButtonPressed() { TimeControlButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
        public virtual void TimeControlButtonUp() { TimeControlButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

        public virtual void SwitchWeaponButtonDown()		{ SwitchWeaponButton.State.ChangeState (MMInput.ButtonStates.ButtonDown); }
		public virtual void SwitchWeaponButtonPressed()		{ SwitchWeaponButton.State.ChangeState (MMInput.ButtonStates.ButtonPressed); }
		public virtual void SwitchWeaponButtonUp()			{ SwitchWeaponButton.State.ChangeState (MMInput.ButtonStates.ButtonUp); }

        public virtual void SwitchCharacterButtonDown() { SwitchCharacterButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
        public virtual void SwitchCharacterButtonPressed() { SwitchCharacterButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
        public virtual void SwitchCharacterButtonUp() { SwitchCharacterButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }
    }
}