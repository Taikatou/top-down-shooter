using System.Collections.Generic;
using Research.CharacterDesign.Scripts.AgentInput;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Test.Scripts
{
    public class TestGridAgent : Agent
    {
        public TestGridController controller;
        public DirectionsKeyMapper directionsKeyMapper;

        public List<TestPickup> coinPickup;
        
        private Vector3 _mTransform;
        
        public float punishValue = -0.0005f;

        public CoinSpawnLocations spawnLocations;
        
        public override void Initialize()
        {
            var position = transform.position;
            _mTransform = new Vector3(position.x, position.y, position.z);
        }

        public void CompleteMovement()
        {
            controller.CompleteMovement();
        }

        public override void CollectObservations(VectorSensor sensor)
        {

        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            //TODO FIX THIS
            // actionsOut.DiscreteActions[0] = (int) directionsKeyMapper.PrimaryDirections;
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            AddReward(punishValue);
            
            var primaryDirection = directionsKeyMapper.GetVectorDirection(Mathf.FloorToInt(actions.DiscreteActions[0]));
            controller.UpdateInput(primaryDirection);
        }

        public override void OnEpisodeBegin()
        {
            transform.position = _mTransform;
            controller.ResetMovePoint(_mTransform);
            
            var random = new System.Random();
            var positionSet = new HashSet<Transform>();

            foreach (var coin in coinPickup)
            {
                var found = false;
                while (!found)
                {
                    var randomIndex = random.Next(0, spawnLocations.positions.Count);
                    var randomTransform = spawnLocations.positions[randomIndex];
                
                    if (!positionSet.Contains(randomTransform))
                    {
                        coin.ResetPosition(randomTransform);
                        positionSet.Add(randomTransform);
                        found = true;
                    }
                }
            }
        }
    }
}
