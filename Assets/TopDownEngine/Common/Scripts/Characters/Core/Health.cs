using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This class manages the health of an object, pilots its potential health bar, handles what happens when it takes damage,
	/// and what happens when it dies.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Core/Health")] 
	public class Health : MonoBehaviour
	{
        /// the model to disable (if set so)
        public GameObject Model;

		/// the current health of the character
		[MMReadOnly]
		public float CurrentHealth ;
		/// If this is true, this object can't take damage
		[MMReadOnly]
		public bool Invulnerable = false;	

		[Header("Health")]
		[MMInformation("Add this component to an object and it'll have health, will be able to get damaged and potentially die.",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
		/// the initial amount of health of the object
	    public int InitialHealth = 10;
	    /// the maximum amount of health of the object
	    public int MaximumHealth = 10;

        [Header("Damage")]
        [MMInformation("Here you can specify an effect and a sound FX to instantiate when the object gets damaged, and also how long the object should flicker when hit (only works for sprites).", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
        /// whether or not this object is immune to damage knockback
        public bool ImmuneToKnockback = false;
        /// the feedback to play when getting damage
        public MMFeedbacks DamageMMFeedbacks;

        [Header("Death")]
        [MMInformation("Here you can set an effect to instantiate when the object dies, a force to apply to it (topdown controller required), how many points to add to the game score, if the device should vibrate (only works on iOS and Android), and where the character should respawn (for non-player characters only).", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
        public bool DestroyOnDeath = true;
		/// the time (in seconds) before the character is destroyed or disabled
		public float DelayBeforeDestruction = 0f;
		/// the points the player gets when the object's health reaches zero
		public int PointsWhenDestroyed;
		/// if this is set to false, the character will respawn at the location of its death, otherwise it'll be moved to its initial position (when the scene started)
		public bool RespawnAtInitialLocation = false;
        /// if this is true, the controller will be disabled on death
        public bool DisableControllerOnDeath = true;
        /// if this is true, the model will be disabled instantly on death (if a model has been set)
        public bool DisableModelOnDeath = true;
        /// if this is true, collisions will be turned off when the character dies
		public bool DisableCollisionsOnDeath = true;
        /// the feedback to play when dying
        public MMFeedbacks DeathMMFeedbacks;

        // hit delegate
        public delegate void OnHitDelegate();
        public OnHitDelegate OnHit;

        // respawn delegate
        public delegate void OnReviveDelegate();
		public OnReviveDelegate OnRevive;

        // death delegate
		public delegate void OnDeathDelegate();
		public OnDeathDelegate OnDeath;

		protected Vector3 _initialPosition;
		protected Renderer _renderer;
		protected Character _character;
		protected TopDownController _controller;
	    protected MMHealthBar _healthBar;
	    protected Collider2D _collider2D;
        protected Collider _collider3D;
        protected CharacterController _characterController;
        protected bool _initialized = false;
		protected Color _initialColor;
        protected AutoRespawn _autoRespawn;
        protected Animator _animator;

        /// <summary>
        /// On Start, we initialize our health
        /// </summary>
        protected virtual void Start()
	    {
			Initialization();
	    }

	    /// <summary>
	    /// Grabs useful components, enables damage and gets the inital color
	    /// </summary>
		protected void Initialization()
		{
			_character = GetComponent<Character>();
            if (Model != null)
            {
                Model.SetActive(true);
            }        
            
            if (gameObject.MMGetComponentNoAlloc<Renderer>() != null)
			{
				_renderer = GetComponent<Renderer>();				
			}
			if (_character != null)
			{
				if (_character.CharacterModel != null)
				{
					if (_character.CharacterModel.GetComponentInChildren<Renderer> ()!= null)
					{
						_renderer = _character.CharacterModel.GetComponentInChildren<Renderer> ();	
					}
				}	
			}
            if (_renderer != null)
            {
                if (_renderer.material.HasProperty("_Color"))
                {
                    _initialColor = _renderer.material.color;
                }                    
            }            

            // we grab our animator
            if (_character != null)
            {
                if (_character.CharacterAnimator != null)
                {
                    _animator = _character.CharacterAnimator;
                }
                else
                {
                    _animator = GetComponent<Animator>();
                }
            }
            else
            {
                _animator = GetComponent<Animator>();
            }

            if (_animator != null)
            {
                _animator.logWarnings = false;
            }

            _autoRespawn = GetComponent<AutoRespawn>();
            _healthBar = GetComponent<MMHealthBar>();
            _controller = GetComponent<TopDownController>();
            _characterController = GetComponent<CharacterController>();
            _collider2D = GetComponent<Collider2D>();
            _collider3D = GetComponent<Collider>();

            //DamageFeedback.Initialization(this.gameObject);
            //DeathFeedback.Initialization(this.gameObject);
            DamageMMFeedbacks?.Initialization(this.gameObject);
            DeathMMFeedbacks?.Initialization(this.gameObject);

            _initialPosition = transform.position;
			_initialized = true;
			CurrentHealth = InitialHealth;
			DamageEnabled();
			UpdateHealthBar (false);
		}

		/// <summary>
		/// When the object is enabled (on respawn for example), we restore its initial health levels
		/// </summary>
	    protected virtual void OnEnable()
	    {
			CurrentHealth = InitialHealth;
            if (Model != null)
            {
                Model.SetActive(true);
            }            
			DamageEnabled();
			UpdateHealthBar (false);
	    }

		/// <summary>
		/// Called when the object takes damage
		/// </summary>
		/// <param name="damage">The amount of health points that will get lost.</param>
		/// <param name="instigator">The object that caused the damage.</param>
		/// <param name="flickerDuration">The time (in seconds) the object should flicker after taking the damage.</param>
		/// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
		public virtual void Damage(int damage, GameObject instigator, float flickerDuration, float invincibilityDuration)
		{
			// if the object is invulnerable, we do nothing and exit
			if (Invulnerable)
			{
				return;
			}

			// if we're already below zero, we do nothing and exit
			if ((CurrentHealth <= 0) && (InitialHealth != 0))
			{
				return;
			}

			// we decrease the character's health by the damage
			float previousHealth = CurrentHealth;
			CurrentHealth -= damage;

            if (OnHit != null)
            {
                OnHit();
            }

            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }

            // we prevent the character from colliding with Projectiles, Player and Enemies
            if (invincibilityDuration > 0)
			{
				DamageDisabled();
				StartCoroutine(DamageEnabled(invincibilityDuration));	
			}
            
			// we trigger a damage taken event
			MMDamageTakenEvent.Trigger(_character, instigator, CurrentHealth, damage, previousHealth);

            if (_animator != null)
            {
                _animator.SetTrigger("Damage");
            }

            DamageMMFeedbacks?.PlayFeedbacks(this.transform.position);
            
			// we update the health bar
			UpdateHealthBar(true);

			// if health has reached zero
			if (CurrentHealth <= 0)
			{
				// we set its health to zero (useful for the healthbar)
				CurrentHealth = 0;
				
				Kill();
			}
		}

		/// <summary>
		/// Kills the character, vibrates the device, instantiates death effects, handles points, etc
		/// </summary>
		public virtual void Kill()
        {
	        if (_character != null)
            {
                // we set its dead state to true
                _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Dead);
                _character.Reset();

                if (_character.CharacterType == Character.CharacterTypes.Player)
                {
                    TopDownEngineEvent.Trigger(TopDownEngineEventTypes.PlayerDeath, _character);
                }
            }
            CurrentHealth = 0;

            // we prevent further damage
            DamageDisabled();

            DeathMMFeedbacks?.PlayFeedbacks(this.transform.position);
            
			// Adds points if needed.
			if(PointsWhenDestroyed != 0)
			{
				// we send a new points event for the GameManager to catch (and other classes that may listen to it too)
				TopDownEnginePointEvent.Trigger(PointsMethods.Add, PointsWhenDestroyed);
			}

            if (_animator != null)
            {
                _animator.SetTrigger("Death");
            }
            // we make it ignore the collisions from now on
            if (DisableCollisionsOnDeath)
            {
                if (_collider2D != null)
                {
                    _collider2D.enabled = false;
                }
                if (_collider3D != null)
                {
                    _collider3D.enabled = false;
                }

                // if we have a controller, removes collisions, restores parameters for a potential respawn, and applies a death force
                if (_controller != null)
			    {				
					_controller.CollisionsOff();						
                }
			}

            OnDeath?.Invoke();

            if (DisableControllerOnDeath && (_controller != null))
            {
                _controller.enabled = false;
                //_controller.SetKinematic(true);
            }

            if (DisableControllerOnDeath && (_characterController != null))
            {
                _characterController.enabled = false;
            }

            if (DisableModelOnDeath && (Model != null))
            {
                Model.SetActive(false);
            }

			if (DelayBeforeDestruction > 0f)
			{
				Invoke ("DestroyObject", DelayBeforeDestruction);
			}
			else
			{
				// finally we destroy the object
				DestroyObject();	
			}
		}

		/// <summary>
		/// Revive this object.
		/// </summary>
		public virtual void Revive()
		{
			if (!_initialized)
			{
				return;
			}

            if (_collider2D != null)
            {
                _collider2D.enabled = true;
            }
            if (_collider3D != null)
            {
                _collider3D.enabled = true;
            }
            if (_characterController != null)
            {
                _characterController.enabled = true;
            }
            if (_controller != null)
			{
                _controller.enabled = true;
				_controller.CollisionsOn();
				_controller.Reset();
			}
			if (_character != null)
			{
				_character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
			}
            if (_renderer!= null)
            {
                _renderer.material.color = _initialColor;
            }            

            if (RespawnAtInitialLocation)
			{
				transform.position = _initialPosition;
			}
            if (_healthBar != null)
            {
                _healthBar.Initialization();
            }

            Initialization();
			UpdateHealthBar(false);
			if (OnRevive != null)
			{
				OnRevive ();
			}
		}

	    /// <summary>
	    /// Destroys the object, or tries to, depending on the character's settings
	    /// </summary>
	    protected virtual void DestroyObject()
		{
			if (!DestroyOnDeath)
			{
				return;
			}
			
            if (_autoRespawn == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                _autoRespawn.Kill();
            }
			

		}

		/// <summary>
		/// Called when the character gets health (from a stimpack for example)
		/// </summary>
		/// <param name="health">The health the character gets.</param>
		/// <param name="instigator">The thing that gives the character health.</param>
		public virtual float GetHealth(float health,GameObject instigator)
		{
			var previousHealth = CurrentHealth;
			// this function adds health to the character's Health and prevents it to go above MaxHealth.
			CurrentHealth = Mathf.Min (CurrentHealth + health,MaximumHealth);
			UpdateHealthBar(true);
			return CurrentHealth - previousHealth;
		}

	    /// <summary>
	    /// Resets the character's health to its max value
	    /// </summary>
	    public virtual void ResetHealthToMaxHealth()
	    {
			CurrentHealth = MaximumHealth;
			UpdateHealthBar (false);
	    }	

	    /// <summary>
	    /// Updates the character's health bar progress.
	    /// </summary>
		protected virtual void UpdateHealthBar(bool show)
	    {
	    	if (_healthBar != null)
	    	{
				_healthBar.UpdateBar(CurrentHealth, 0f, MaximumHealth, show);
	    	}

	    	if (_character != null)
	    	{
	    		if (_character.CharacterType == Character.CharacterTypes.Player)
	    		{
					// We update the health bar
					if (GUIManager.Instance != null)
					{
						GUIManager.Instance.UpdateHealthBar(CurrentHealth, 0f, MaximumHealth, _character.PlayerID);
					}
	    		}
	    	}
	    }

	    /// <summary>
	    /// Prevents the character from taking any damage
	    /// </summary>
	    public virtual void DamageDisabled()
	    {
			Invulnerable = true;
	    }

	    /// <summary>
	    /// Allows the character to take damage
	    /// </summary>
	    public virtual void DamageEnabled()
	    {
	    	Invulnerable = false;
	    }

		/// <summary>
	    /// makes the character able to take damage again after the specified delay
	    /// </summary>
	    /// <returns>The layer collision.</returns>
	    public virtual IEnumerator DamageEnabled(float delay)
		{
			yield return new WaitForSeconds (delay);
			Invulnerable = false;
		}

        /// <summary>
        /// On Disable, we prevent any delayed destruction from running
        /// </summary>
        protected virtual void OnDisable()
        {
            CancelInvoke();
        }
	}
}