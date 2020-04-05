using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;

namespace MoreMountains.TopDownEngine
{
    public class AutoBindAutoFocus : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        /// the AutoFocus component on the camera
        public MMAutoFocus AutoFocus { get; set; }

        protected virtual void Start()
        {
            AutoFocus = FindObjectOfType<MMAutoFocus>();
        }
        
        public virtual void OnMMEvent(MMCameraEvent cameraEvent)
        {
            switch (cameraEvent.EventType)
            {
                case MMCameraEventTypes.StartFollowing:
                    if (AutoFocus == null)
                    {
                        AutoFocus = FindObjectOfType<MMAutoFocus>();
                    }
                    if (AutoFocus != null)
                    {
                        AutoFocus.FocusTargets = new Transform[1];
                        AutoFocus.FocusTargets[0] = LevelManager.Instance.Players[0].transform; 
                    }
                    break;
            }
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMCameraEvent>();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMCameraEvent>();
        }
    }
}
