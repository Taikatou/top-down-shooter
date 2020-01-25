using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using BattleResearch.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;

using Cinemachine;
using UnityEditor.UIElements;


namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// Spawns the player, handles checkpoints and respawn
	/// </summary>
	[AddComponentMenu("TopDown Engine/Managers/Level Manager")]
	public class LevelManager : Singleton<LevelManager>, MMEventListener<TopDownEngineEvent>
	{	
		/// the prefab you want for your player
		[Header("Playable Characters")]
		[Information("The LevelManager is responsible for handling spawn/respawn, checkpoints management and level bounds. Here you can define one or more playable characters for your level..",InformationAttribute.InformationType.Info,false)]
		/// the list of player prefabs to instantiate
		public Character[] PlayerPrefabs ;
		/// should the player IDs be auto attributed (usually yes)
		public bool AutoAttributePlayerIDs = true;
        /// the checkpoint to use as initial spawn point if no point of entry is specified
        public CheckPoint InitialSpawnPoint;
        /// the currently active checkpoint (the last checkpoint passed by the player)
        public CheckPoint CurrentCheckpoint;

        [Header("Points of Entry")]
        /// A list of this level's points of entry, which can be used from other levels as initial targets
        public Transform[] PointsOfEntry;
        				
		[Space(10)]
		[Header("Intro and Outro durations")]
		[Information("Here you can specify the length of the fade in and fade out at the start and end of your level. You can also determine the delay before a respawn.",InformationAttribute.InformationType.Info,false)]
		/// duration of the initial fade in (in seconds)
		public float IntroFadeDuration=1f;
		/// duration of the fade to black at the end of the level (in seconds)
		public float OutroFadeDuration=1f;
        /// the ID to use when triggering the event (should match the ID on the fader you want to use)
        public int FaderID = 0;
        /// the curve to use for in and out fades
        public MMTween.MMTweenCurve FadeCurve = MMTween.MMTweenCurve.EaseInCubic;
        /// duration between a death of the main character and its respawn
        public float RespawnDelay = 2f;

        [Header("Respawn Loop")]
        /// the delay, in seconds, before displaying the death screen once the player is dead
        public float DelayBeforeDeathScreen = 1f;

        /// the level limits, camera and player won't go beyond this point.
        public Bounds LevelBounds {  get { return _collider.bounds; } }
        public Collider BoundsCollider { get; protected set; }

		/// the elapsed time since the start of the level
		public TimeSpan RunningTime { get { return DateTime.UtcNow - _started ;}}

        /// the AutoFocus component on the camera
        public MMAutoFocus AutoFocus { get; set; } 


        // private stuff
        public List<CheckPoint> Checkpoints { get; protected set; }
        public List<Character> Players { get; protected set; }
	    protected DateTime _started;
	    protected int _savedPoints;
        protected Collider _collider;
        protected Vector3 _initialSpawnPointPosition;
		
		/// <summary>
		/// On awake, instantiates the player
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
            _collider = this.GetComponent<Collider>();
            _initialSpawnPointPosition = (InitialSpawnPoint == null) ? Vector3.zero : InitialSpawnPoint.transform.position;
        }

        /// <summary>
        /// On Start we grab our dependencies and initialize spawn
        /// </summary>
        protected virtual void Start()
        {
            BoundsCollider = _collider;

            Initialization();
        }

        /// <summary>
        /// A method meant to be overridden by each multiplayer level manager to describe how to spawn characters
        /// </summary>
        protected virtual void SpawnMultipleCharacters()
        {

        }

        public string team1Layer;

        public string team2Layer;

        /// <summary>
        /// Instantiate playable characters based on the ones specified in the PlayerPrefabs list in the LevelManager's inspector.
        /// </summary>
        protected virtual void InstantiatePlayableCharacters()
		{
			Players = new List<Character> ();

            // we check if there's a stored character in the game manager we should instantiate
            if (GameManager.Instance.StoredCharacter != null)
            {
                Character newPlayer = Instantiate(GameManager.Instance.StoredCharacter, _initialSpawnPointPosition, Quaternion.identity);
                newPlayer.name = GameManager.Instance.StoredCharacter.name;
                Players.Add(newPlayer);
                return;
            }

            if (PlayerPrefabs == null) { return; }

			// player instantiation
			if (PlayerPrefabs.Count() != 0)
			{
				var count = 0;
				foreach (Character playerPrefab in PlayerPrefabs)
				{
					count++;
					
					Character newPlayer = Instantiate (playerPrefab, _initialSpawnPointPosition, Quaternion.identity);
					newPlayer.name = playerPrefab.name;
					var character = newPlayer.GetComponent<Character>();
					if (character)
					{
						var curentTeam = count <= 2;
						character.PlayerID = "Player"+count;
						
						character.teamTag = curentTeam? team1Layer : team2Layer;
						
						character.enemyTag = !curentTeam? team1Layer : team2Layer;
					}

					Players.Add(newPlayer);

					if (playerPrefab.CharacterType != Character.CharacterTypes.Player)
					{
						Debug.LogWarning ("LevelManager : The Character you've set in the LevelManager isn't a Player, which means it's probably not going to move. You can change that in the Character component of your prefab.");
					}
				}
			}
			else
			{
				//Debug.LogWarning ("LevelManager : The Level Manager doesn't have any Player prefab to spawn. You need to select a Player prefab from its inspector.");
				return;
			}	
		}

        /// <summary>
		/// Assigns all respawnable objects in the scene to their checkpoint
		/// </summary>
		protected virtual void CheckpointAssignment()
        {
            // we get all respawnable objects in the scene and attribute them to their corresponding checkpoint
            IEnumerable<Respawnable> listeners = FindObjectsOfType<MonoBehaviour>().OfType<Respawnable>();
            AutoRespawn autoRespawn;
            foreach (Respawnable listener in listeners)
            {
                for (int i = Checkpoints.Count - 1; i >= 0; i--)
                {
                    autoRespawn = (listener as MonoBehaviour).GetComponent<AutoRespawn>();
                    if (autoRespawn == null)
                    {
                        Checkpoints[i].AssignObjectToCheckPoint(listener);
                        continue;
                    }
                    else
                    {
                        if (autoRespawn.IgnoreCheckpointsAlwaysRespawn)
                        {
                            Checkpoints[i].AssignObjectToCheckPoint(listener);
                            continue;
                        }
                        else
                        {
                            if (autoRespawn.AssociatedCheckpoints.Contains(Checkpoints[i]))
                            {
                                Checkpoints[i].AssignObjectToCheckPoint(listener);
                                continue;
                            }
                            continue;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Gets current camera, points number, start time, etc.
        /// </summary>
        protected virtual void Initialization()
		{
			InstantiatePlayableCharacters();

			MMCameraEvent.Trigger(MMCameraEventTypes.SetConfiner, null, BoundsCollider);

			AutoFocus = FindObjectOfType<MMAutoFocus>();
			
            Checkpoints = FindObjectsOfType<CheckPoint>().OrderBy(o => o.CheckPointOrder).ToList();
            _savedPoints =GameManager.Instance.Points;
			_started = DateTime.UtcNow;
			
			if (Players == null || Players.Count == 0) { return; }

			// we handle the spawn of the character(s)
			if (Players.Count == 1)
			{
				SpawnSingleCharacter();
			}
			else
			{
				SpawnMultipleCharacters ();
			}

			CheckpointAssignment();

			// we trigger a level start event
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.LevelStart, null);
			MMGameEvent.Trigger("Load");

			MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, Players[0]);
			MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
			if (AutoFocus != null)
			{
				AutoFocus.FocusTargets = new Transform[1];
				AutoFocus.FocusTargets[0] = Players[0].transform;
			}
			MMGameEvent.Trigger("CameraBound");
		}

		/// <summary>
		/// Spawns a playable character into the scene
		/// </summary>
		protected virtual void SpawnSingleCharacter()
		{
            PointsOfEntryStorage point = GameManager.Instance.GetPointsOfEntry(SceneManager.GetActiveScene().name);
            if ((point != null) && (PointsOfEntry.Length >= (point.PointOfEntryIndex + 1)))
            {
                Players[0].RespawnAt(PointsOfEntry[point.PointOfEntryIndex], Character.FacingDirections.East);
                return;
            }

            if (InitialSpawnPoint != null)
            {
                InitialSpawnPoint.SpawnPlayer(Players[0]);
                return;
            }
        }

		/// <summary>
		/// Every frame we check for checkpoint reach
		/// </summary>
		public virtual void Update()
		{
			if (Players == null)
			{
				return;
			}
						
			_savedPoints = GameManager.Instance.Points;
			_started = DateTime.UtcNow;
		}

		/// <summary>
		/// Gets the player to the specified level
		/// </summary>
		/// <param name="levelName">Level name.</param>
		public virtual void GotoLevel(string levelName)
		{		
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.LevelEnd, null);
			MMGameEvent.Trigger("Save");

	        StartCoroutine(GotoLevelCo(levelName));
	    }

	    /// <summary>
	    /// Waits for a short time and then loads the specified level
	    /// </summary>
	    /// <returns>The level co.</returns>
	    /// <param name="levelName">Level name.</param>
	    protected virtual IEnumerator GotoLevelCo(string levelName)
		{
			if (Players != null && Players.Count > 0)
	        { 
				foreach (Character player in Players)
				{
					player.Disable ();	
				}	    		
	        }

            MMFadeInEvent.Trigger(OutroFadeDuration);
            if (Time.timeScale > 0.0f)
	        { 
	            yield return new WaitForSeconds(OutroFadeDuration);
			}
			// we trigger an unPause event for the GameManager (and potentially other classes)
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.UnPause, null);

	        if (string.IsNullOrEmpty(levelName))
	        {
				LoadingSceneManager.LoadScene("StartScreen");
			}
			else
			{
				LoadingSceneManager.LoadScene(levelName);
			}			
		}

		/// <summary>
		/// Kills the player.
		/// </summary>
		public virtual void PlayerDead(Character playerCharacter)
		{
			if (Players.Count < 2)
			{
				StartCoroutine (PlayerDeadCo ());
			}
		}

        /// <summary>
        /// Triggers the death screen display after a short delay
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator PlayerDeadCo()
        {
            yield return new WaitForSeconds(DelayBeforeDeathScreen);

            GUIManager.Instance.SetDeathScreen(true);
        }

        /// <summary>
        /// Initiates the respawn
        /// </summary>
        protected virtual void Respawn()
        {
            if (Players.Count < 2)
            {
                StartCoroutine(SoloModeRestart());
            }
        }

	    /// <summary>
	    /// Coroutine that kills the player, stops the camera, resets the points.
	    /// </summary>
	    /// <returns>The player co.</returns>
	    protected virtual IEnumerator SoloModeRestart()
		{
			if (PlayerPrefabs.Count() <= 0)
            {
                yield break;
			}

            // if we've setup our game manager to use lives (meaning our max lives is more than zero)
            if (GameManager.Instance.MaximumLives > 0)
            {
                // we lose a life
                GameManager.Instance.LoseLife();
                // if we're out of lives, we check if we have an exit scene, and move there
                if (GameManager.Instance.CurrentLives <= 0)
                {
                    TopDownEngineEvent.Trigger(TopDownEngineEventTypes.GameOver, null);
                    if (!string.IsNullOrEmpty(GameManager.Instance.GameOverScene))
                    {
                        LoadingSceneManager.LoadScene(GameManager.Instance.GameOverScene);
                    }
                }
            }

            MMCameraEvent.Trigger(MMCameraEventTypes.StopFollowing);

            MMFadeInEvent.Trigger(OutroFadeDuration, FadeCurve, FaderID, true, Players[0].transform.position);
            yield return new WaitForSeconds(OutroFadeDuration);

            yield return new WaitForSeconds(RespawnDelay);
            GUIManager.Instance.SetPauseScreen(false);
            GUIManager.Instance.SetDeathScreen(false);
            MMFadeOutEvent.Trigger(OutroFadeDuration, FadeCurve, FaderID, true, Players[0].transform.position);


            MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);

            if (CurrentCheckpoint == null)
            {
                CurrentCheckpoint = InitialSpawnPoint;
            }

            if (Players[0] == null)
            {
                InstantiatePlayableCharacters();
            }

            if (CurrentCheckpoint != null)
            {
                CurrentCheckpoint.SpawnPlayer(Players[0]);
			}
            _started = DateTime.UtcNow;

            // we send a new points event for the GameManager to catch (and other classes that may listen to it too)
            TopDownEnginePointEvent.Trigger(PointsMethods.Set, 0);
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RespawnComplete, Players[0]);
            yield break;
		}


		/// <summary>
		/// Toggles Character Pause
		/// </summary>
		public virtual void ToggleCharacterPause()
		{
            foreach (Character player in Players)
            {
                CharacterPause characterPause = player.GetComponent<CharacterPause>();
                if (characterPause == null)
                {
                    break;
                }

                if (GameManager.Instance.Paused)
                {
                    characterPause.PauseCharacter();
                }
                else
                {
                    characterPause.UnPauseCharacter();
                }
            }
        }

        /// <summary>
        /// Sets the current checkpoint with the one set in parameter. This checkpoint will be saved and used should the player die.
        /// </summary>
        /// <param name="newCheckPoint"></param>
        public virtual void SetCurrentCheckpoint(CheckPoint newCheckPoint)
        {
            if (newCheckPoint.ForceAssignation)
            {
                CurrentCheckpoint = newCheckPoint;
                return;
            }

            if (CurrentCheckpoint == null)
            {
                CurrentCheckpoint = newCheckPoint;
                return;
            }
            if (newCheckPoint.CheckPointOrder >= CurrentCheckpoint.CheckPointOrder)
            {
                CurrentCheckpoint = newCheckPoint;
            }
        }

        /// <summary>
        /// Catches TopDownEngineEvents and acts on them, playing the corresponding sounds
        /// </summary>
        /// <param name="engineEvent">TopDownEngineEvent event.</param>
        public virtual void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case TopDownEngineEventTypes.PlayerDeath:
                    PlayerDead(engineEvent.OriginCharacter);
                    break;
                case TopDownEngineEventTypes.RespawnStarted:
                    Respawn();
                    break;
            }
        }

        /// <summary>
        /// OnDisable, we start listening to events.
        /// </summary>
        protected virtual void OnEnable()
        {
            this.MMEventStartListening<TopDownEngineEvent>();
        }

        /// <summary>
        /// OnDisable, we stop listening to events.
        /// </summary>
        protected virtual void OnDisable()
        {
            this.MMEventStopListening<TopDownEngineEvent>();
        }
    }
}