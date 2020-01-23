using System;
using System.Collections.Generic;
using MLAgents;
using MoreMountains.TopDownEngine;
using TopDownEngine.Demos.Grasslands.Scripts;
using UnityEngine;

namespace BattleResearch.Scripts
{
    [RequireComponent(typeof(MlAgentInput))]
    public class TopDownAgent : Agent
    {
        public bool useVectorObs;

        public RayPerception2D rayPerception;

        public LayerMask playerMask;

        public LayerMask otherMask;
        
        private Health _healthComponent;
        private enum Directions { Left, Right, Up, Down }

        private Dictionary<Directions, KeyCode> _directions;

        private Dictionary<Directions, KeyCode> _secondaryDirections;
        
        private Rigidbody2D _agentRb;

        private CharacterHandleWeapon _handleWeaponAbility;

        private int divisions = 24;
        
        public bool heuristicEnabled;

        private static bool _heuristicSetup;
        
        public MlAgentInput AgentInput => GetComponent<MlAgentInput>();

        private int CurrentPoints
        {
            get
            {
                var level = FindObjectOfType<MLAgentsGrasslandsMultiplayerLevelManager>();
                return level? level.GetPoints(AgentInput.PlayerId): 0;
            }
        }

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

            _healthComponent = GetComponent<Health>();
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
            
            AgentInput.SetShootButtonState(shootButtonDown);
            
            var reloadButtonDown = Convert.ToBoolean(vectorAction[5]);
            AgentInput.SetReloadButtonState(reloadButtonDown);
            
            var dashButtonDown = Convert.ToBoolean(vectorAction[6]);
            AgentInput.SetDashButtonState(dashButtonDown);
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
            
            //AddVectorObs(new Vector2(direction.x, direction.y));

            var aim2d = GetComponentInChildren<WeaponAim2D>();
            if (aim2d)
            {
                var angle = aim2d.InputMovement;
                AddVectorObs(angle);
            }

            var position = _agentRb.transform.position;
            var position2d = new Vector2(position.x, position.y);
            AddVectorObs(position2d);

            // Add weapon state
            var state = _handleWeaponAbility.CurrentWeapon ?
                (int) _handleWeaponAbility.CurrentWeapon.WeaponState.CurrentState
                : -1;
            AddVectorObs(state);

            if (_healthComponent)
            {
                AddVectorObs(_healthComponent.CurrentHealth);    
            }

            var ammo = _handleWeaponAbility.CurrentWeapon ? 
                _handleWeaponAbility.CurrentWeapon.CurrentAmmoLoaded : 0;

            AddVectorObs(ammo);

            var reload = _handleWeaponAbility.CurrentWeapon && _handleWeaponAbility.CurrentWeapon.Reloading;
            AddVectorObs(reload);

            AddVectorObs(CurrentPoints);
            
        }

        public override float[] Heuristic()
        {
            var x = 0.0f;
            var y = 0.0f;
            var secondaryX = 0.0f;
            var secondaryY = 0.0f;
            var shootButtonInput = 0.0f;
            var reloadButtonInput = 0.0f;
            var dashButtonInput = 0.0f;

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

                var shootButtonState = Input.GetKey(KeyCode.KeypadEnter);
                shootButtonInput = Convert.ToSingle(shootButtonState);

                var reloadButtonState = Input.GetKey(KeyCode.End);
                reloadButtonInput = Convert.ToSingle(reloadButtonState);
                
                var dashButtonState = Input.GetKey(KeyCode.Q);
                dashButtonInput = Convert.ToSingle(dashButtonState);
            }

            var output = new[] {x, y, secondaryX, secondaryY, shootButtonInput, reloadButtonInput, dashButtonInput};

            return output;
        }

        public override void AgentReset()
        {
            _heuristicSetup = false;
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.MlCuriculum, null);
        }
    }
}
