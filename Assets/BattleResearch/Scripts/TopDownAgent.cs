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
        
        public LayerMask otherMask;

        private enum Directions { Left, Right, Up, Down }

        private Dictionary<Directions, KeyCode> _directions;

        private Dictionary<Directions, KeyCode> _secondaryDirections;

        private int divisions = 24;

        public MlAgentInput AgentInput => GetComponent<MlAgentInput>();

        private ISense[] Senses => GetComponentsInChildren<ISense>();

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

            var secondaryShootButtonDown = Convert.ToBoolean(vectorAction[5]);
            AgentInput.SetSecondaryShootButtonState(secondaryShootButtonDown);
            
            var reloadButtonDown = Convert.ToBoolean(vectorAction[6]);
            AgentInput.SetReloadButtonState(reloadButtonDown);
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
                
                AddVectorObs(observations);
            }
            
            foreach (var sense in Senses)
            {
                var obs = sense.GetObservations();

                AddVectorObs(obs);
            }
            
            var agentRb = GetComponent<Rigidbody2D>();
            var position = agentRb.transform.position;
            
            var agentSenses = FindObjectsOfType<AgentSense>();

            foreach (var agentSense in agentSenses)
            {
                if (agentSense.gameObject != gameObject)
                {
                    var obs = agentSense.GetObservationsOtherAgent(position);
                    AddVectorObs(obs);
                }
            }

            AddVectorObs(CurrentPoints);
        }

        public override float[] Heuristic()
        {
            var x = 0.0f;
            var y = 0.0f;
            var secondaryX = 0.0f;
            var secondaryY = 0.0f;

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

            var shootButtonState = Input.GetKey(KeyCode.X);
            var shootButtonInput = Convert.ToSingle(shootButtonState);

            var secondaryShootButtonState = Input.GetKey(KeyCode.C);
            var secondaryShootButtonInput = Convert.ToSingle(secondaryShootButtonState);

            var reloadButtonState = Input.GetKey(KeyCode.End);
            var reloadButtonInput = Convert.ToSingle(reloadButtonState);

            var output = new[] {x, y, secondaryX, secondaryY, shootButtonInput, secondaryShootButtonInput, reloadButtonInput };

            return output;
        }

        public override void AgentReset()
        {
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.MlCuriculum, null);
        }
    }
}
