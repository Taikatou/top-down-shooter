using System;
using System.Collections.Generic;
using MLAgents;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace BattleResearch.Scripts
{
    [RequireComponent(typeof(MlAgentInput))]
    public class TopDownAgent : Agent
    {
        public bool useVectorObs;

        public RayPerception2D rayPerception;

        public LayerMask playerMask;

        public LayerMask otherMask;
        
        public Health healthComponent;
        public MlAgentInput AgentInput => GetComponent<MlAgentInput>();

        private enum Directions { Left, Right, Up, Down }

        private Dictionary<Directions, KeyCode> _directions;

        private Dictionary<Directions, KeyCode> _secondaryDirections;
        
        private Rigidbody2D _agentRb;

        private CharacterHandleWeapon _handleWeaponAbility;

        private int divisions = 24;
        
        public bool heuristicEnabled;

        private static bool _heuristicSetup;

        public override void InitializeAgent()
        {
            base.InitializeAgent();

            if (!_heuristicSetup)
            {
                _heuristicSetup = true;
                heuristicEnabled = true;
            }
            
            _directions = new Dictionary<Directions, KeyCode>()
            {
                {Directions.Left, KeyCode.A},
                {Directions.Right, KeyCode.D },
                {Directions.Down, KeyCode.S},
                {Directions.Up, KeyCode.W }
            };
            _secondaryDirections = new Dictionary<Directions, KeyCode>()
            {
                {Directions.Left, KeyCode.LeftArrow},
                {Directions.Right, KeyCode.RightArrow },
                {Directions.Down, KeyCode.DownArrow },
                {Directions.Up, KeyCode.UpArrow }
            };
            _agentRb = GetComponent<Rigidbody2D>();

            _handleWeaponAbility = GetComponent<CharacterHandleWeapon>();
        }

        private int GetDecision(float input)
        {
            var output = 0;
            switch (Mathf.FloorToInt(input))
            {
                case 1:
                    // Left or Down
                    output = -1;
                    break;
                case 2:
                    // Right or Up
                    output = 1;
                    break;
            }
            return output;
        }

        public override void AgentAction(float[] vectorAction)
        {
            var xInput = GetDecision(vectorAction[0]);
            var yInput = GetDecision(vectorAction[1]);

            AgentInput.PrimaryInput = new Vector2(xInput, yInput);

            var secondaryXInput = GetDecision(vectorAction[2]);
            var secondaryYInput = GetDecision(vectorAction[3]);
            var secondary = new Vector2(secondaryXInput, secondaryYInput);

            AgentInput.SecondaryInput = secondary;

            var shootButtonDown = Convert.ToBoolean(vectorAction[4]);
            
            AgentInput.SetButton(shootButtonDown);
        }

        private int GetInput(KeyCode negativeKey, KeyCode positiveKey)
        {
            var negative = Input.GetKey(negativeKey);
            var positive = Input.GetKey(positiveKey);
            if (negative ^ positive)
            {
                return negative ? 1 : 2;
            }
            return 0;
        }
        
        public override void CollectObservations()
        {
            if (useVectorObs)
            {
                var angles = new float[divisions];

                var degrees = (360 / divisions);
                
                for (var i = 0; i < divisions; i++)
                {
                    angles[i] =  degrees * i;
                }

                var observations = rayPerception.Perceive(25, angles, new[] {"walls"}, 
                                                            otherMask, 0, 0, Color.red);
                
                
                
                var playerOBs = rayPerception.Perceive(25, angles, new [] {"Player"}, 
                                                        playerMask,0, 0, Color.blue);

                AddVectorObs(observations);
                AddVectorObs(playerOBs);
            }

            AddVectorObs(transform.InverseTransformDirection(_agentRb.velocity));

            var state = _handleWeaponAbility.CurrentWeapon ?
                (int) _handleWeaponAbility.CurrentWeapon.WeaponState.CurrentState
                : -1;
            AddVectorObs(state);
            
            AddVectorObs(healthComponent.CurrentHealth);
        }

        public override float[] Heuristic()
        {
            var x = 0.0f;
            var y = 0.0f;
            var secondaryX = 0.0f;
            var secondaryY = 0.0f;
            var buttonInput = 0.0f;

            if (heuristicEnabled)
            {
                if (_directions!=null)
                {
                    x = GetInput(_directions[Directions.Left], _directions[Directions.Right]);
                    y = GetInput(_directions[Directions.Down], _directions[Directions.Up]);
                }
                if (_secondaryDirections != null)
                {
                    secondaryX = GetInput(_secondaryDirections[Directions.Left], _secondaryDirections[Directions.Right]);
                    secondaryY = GetInput(_secondaryDirections[Directions.Down], _secondaryDirections[Directions.Up]);
                }

                var buttonState = Input.GetKey(KeyCode.KeypadEnter);
                buttonInput = Convert.ToSingle(buttonState);
            }

            return new [] { x, y, secondaryX, secondaryY, buttonInput};
        }

        public override void AgentReset()
        {
            _heuristicSetup = false;
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.MlCuriculum, null);
        }
    }
}
