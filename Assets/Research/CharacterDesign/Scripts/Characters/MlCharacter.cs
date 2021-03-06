﻿using MoreMountains.TopDownEngine;
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

        protected override void Update()
        {

        }

        public void UpdateFrame()
        {
            EveryFrame();
        }
    }
}
