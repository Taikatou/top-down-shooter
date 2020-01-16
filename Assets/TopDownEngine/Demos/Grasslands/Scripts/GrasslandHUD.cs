using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
    public class GrasslandHUD : MonoBehaviour, MMEventListener<TopDownEngineEvent>
    {
        public string PlayerID = "Player1";
        public MMProgressBar HealthBar;
        public Text PlayerName;
        public MMRadialProgressBar AvatarBar;
        public Text CoinCounter;
        public CanvasGroup DeadMask;
        public CanvasGroup WinnerScreen;

        private bool _usingAi = true;
        
        protected virtual void Start()
        {
            CoinCounter.text = "0";
            DeadMask.gameObject.SetActive(false);
            WinnerScreen.gameObject.SetActive(false);
        }

        public virtual void OnMMEvent(TopDownEngineEvent tdEvent)
        {
            switch (tdEvent.EventType)
            {
                case TopDownEngineEventTypes.PlayerDeath:
                    if (tdEvent.OriginCharacter.PlayerID == PlayerID)
                    {
                        if (!_usingAi)
                        {
                            DeadMask.gameObject.SetActive(true);
                            DeadMask.alpha = 0f;
                            StartCoroutine(MMFade.FadeCanvasGroup(DeadMask, 0.5f, 0.8f, true));
                        }
                    }
                    break;
                case TopDownEngineEventTypes.Repaint:
                    var grasslandPoints = (LevelManager.Instance as GrasslandsMultiplayerLevelManager)?.Points;
                    if (grasslandPoints != null)
                        foreach (GrasslandsMultiplayerLevelManager.GrasslandPoints points in
                            grasslandPoints)
                        {
                            if (points.PlayerID == PlayerID)
                            {
                                CoinCounter.text = points.Points.ToString();
                            }
                        }

                    break;
                case 
                TopDownEngineEventTypes.GameOver:
                    if (!_usingAi)
                    {
                        if (PlayerID == (LevelManager.Instance as GrasslandsMultiplayerLevelManager)?.WinnerID)
                        {
                            WinnerScreen.gameObject.SetActive(true);
                            WinnerScreen.alpha = 0f;
                            StartCoroutine(MMFade.FadeCanvasGroup(WinnerScreen, 0.5f, 0.8f, true));
                        }
                    }
                    else
                    {
                        Start();
                    }
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
