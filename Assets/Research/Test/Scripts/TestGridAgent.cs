using System.Collections.Generic;
using Research.CharacterDesign.Scripts.AgentInput;
using Unity.MLAgents;
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
            _mTransform = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        public void CompleteMovement()
        {
            controller.CompleteMovement();
        }

        public override void CollectObservations(VectorSensor sensor)
        {

        }

        public override void Heuristic(float[] actionsOut)
        {
            var index = 0;
            actionsOut[index++] = (int) directionsKeyMapper.PrimaryDirections;
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            AddReward(punishValue);

            var counter = 0;
            var primaryDirection = directionsKeyMapper.GetVectorDirection(vectorAction[counter++]);
            controller.UpdateInput(primaryDirection);
            
            Debug.Log("Action Received");
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
                    var randomIndex = random.Next(0, spawnLocations.possibleSpawnPositions.Count);
                    var randomTransform = spawnLocations.possibleSpawnPositions[randomIndex];
                
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
