using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using MoreMountains.MMInterface;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Simple start screen class.
	/// </summary>
	public class StartScreen : MonoBehaviour
	{
		/// the level to load after the start screen
		public string NextLevel;
		/// the delay after which the level should auto skip (if less than 1s, won't autoskip)
		public float AutoSkipDelay = 0f;

		[Header("Fades")]
        /// the duration of the fade from black at the start of the level
		public float FadeInDuration = 1f;
        /// the duration of the fade to black at the end of the level
		public float FadeOutDuration = 1f;

		[Header("Sound Settings Bindings")]
        /// the switch used to turn the music on or off
		public MMSwitch MusicSwitch;
        /// the switch used to turn the SFX on or off
		public MMSwitch SfxSwitch;

		/// <summary>
		/// Initialization
		/// </summary>
		protected virtual void Awake()
		{	
			GUIManager.Instance.SetHUDActive (false);
			MMFadeOutEvent.Trigger(FadeInDuration);
            Cursor.visible = true;
			if (AutoSkipDelay > 1f)
			{
				FadeOutDuration = AutoSkipDelay;
				StartCoroutine (LoadFirstLevel ());
			}
		}

        /// <summary>
        /// On Start, initializes the music and sfx switches
        /// </summary>
		protected virtual void Start()
		{
			if (MusicSwitch != null)
			{
				MusicSwitch.CurrentSwitchState = SoundManager.Instance.Settings.MusicOn ? MMSwitch.SwitchStates.Right : MMSwitch.SwitchStates.Left;
				MusicSwitch.InitializeState ();
			}

			if (SfxSwitch != null)
			{
				SfxSwitch.CurrentSwitchState = SoundManager.Instance.Settings.SfxOn ? MMSwitch.SwitchStates.Right : MMSwitch.SwitchStates.Left;
				SfxSwitch.InitializeState ();
			}
		}

		/// <summary>
		/// During update we simply wait for the user to press the "jump" button.
		/// </summary>
		protected virtual void Update()
		{
			if (!Input.GetButtonDown ("Player1_Jump"))
				return;
			
			ButtonPressed ();
		}

		/// <summary>
		/// What happens when the main button is pressed
		/// </summary>
		public virtual void ButtonPressed()
		{
			MMFadeInEvent.Trigger(FadeOutDuration);
			// if the user presses the "Jump" button, we start the first level.
			StartCoroutine (LoadFirstLevel ());
		}

		/// <summary>
		/// Loads the next level.
		/// </summary>
		/// <returns>The first level.</returns>
		protected virtual IEnumerator LoadFirstLevel()
		{
			yield return new WaitForSeconds (FadeOutDuration);
			LoadingSceneManager.LoadScene (NextLevel);
		}
	}
}