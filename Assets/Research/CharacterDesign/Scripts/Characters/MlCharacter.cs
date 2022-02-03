using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Environment;
using Unity.MLAgents;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Characters
{
    public class MlCharacter : Character
    {
        public override void SetInputManager()
        {
            var inputManager = GetComponent<TopDownInputManager>();
            SetInputManager(inputManager);
        }

        public void UpdateFrame()
        {
            EveryFrame();
        }
    }
}
