using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will let you change the material of the target renderer everytime it's played.")]
    [FeedbackPath("GameObject/Material")]
    public class MMFeedbackMaterial : MMFeedback
    {
        public enum Methods { Sequential, Random }

        [Header("Material")]
        public Renderer TargetRenderer;
        public Methods Method;
        [MMFEnumCondition("Method", (int)Methods.Sequential)]
        public bool Loop = true;
        [MMFEnumCondition("Method", (int)Methods.Random)]
        public bool AlwaysNewMaterial = true;
        public int InitialIndex = 0;
        public List<Material> Materials;

        protected int _currentIndex;

        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            _currentIndex = InitialIndex;
        }

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Materials.Count == 0)
            {
                Debug.LogError("[MMFeedbackMaterial on " + this.name + "] The Materials array is empty.");
                return;
            }

            int newIndex = DetermineNextIndex();

            if (Materials[newIndex] == null)
            {
                Debug.LogError("[MMFeedbackMaterial on " + this.name + "] Attempting to switch to a null material.");
                return;
            }

            TargetRenderer.material = Materials[newIndex];
        }

        protected virtual int DetermineNextIndex()
        {
            switch(Method)
            {
                case Methods.Random:
                    int random = Random.Range(0, Materials.Count);
                    if (AlwaysNewMaterial)
                    {
                        while (_currentIndex == random)
                        {
                            random = Random.Range(0, Materials.Count);
                        }
                    }
                    _currentIndex = random;
                    return _currentIndex;                    

                case Methods.Sequential:
                    _currentIndex++;
                    if (_currentIndex >= Materials.Count)
                    {
                        _currentIndex = Loop ? 0 : _currentIndex;
                    }
                    return _currentIndex;
            }
            return 0;
        }
    }
}
