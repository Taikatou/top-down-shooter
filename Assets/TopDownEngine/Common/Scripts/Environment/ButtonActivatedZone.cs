using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Add this component to a collider 2D and you'll be able to have it perform an action on enter/exit and while staying
    /// </summary>
    [AddComponentMenu("TopDown Engine/Environment/Button Activated Zone")]
    public class ButtonActivatedZone : ButtonActivated
    {
        [Header("Zone")]
        public UnityEvent OnEnter;
        public UnityEvent OnExit;
        public UnityEvent OnStay;

        protected bool _staying = false;

        public override void TriggerButtonAction()
        {

            if (!CheckNumberOfUses())
            {
                PromptError();
                return;
            }

            ActivateZone();

            _staying = true;
            if (OnEnter != null)
            {
                OnEnter.Invoke();
            }
        }

        public override void TriggerExitAction(GameObject collider)
        {
            _staying = false;
            if (OnExit != null)
            {
                OnExit.Invoke();
            }
        }

        protected virtual void Update()
        {
            if (_staying && (OnStay != null))
            {
                OnStay.Invoke();
            }
        }
    }
}
