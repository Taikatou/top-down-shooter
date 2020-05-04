using System;
using System.Collections.Generic;
using Research.CharacterDesign.Scripts;
using Research.LevelDesign.NuclearThrone;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class AiAccessor : MonoBehaviour
    {
        public NuclearThroneLevelGenerator levelGenerator;

        public GridSpace[,] Map
        {
            get
            {
                if (levelGenerator != null)
                {
                    return levelGenerator.Map;
                }
                return null;
            }
        }

        public List<Tuple<TopDownAgent, Vector3Int>> AgentPosition
        {
            get
            {
                var agents = agentQueue.GetComponentsInChildren<TopDownAgent>();
                var positions = new List<Tuple<TopDownAgent, Vector3Int>>();
                foreach (var agent in agents)
                {
                    if (agent.enabled)
                    {
                        var position = agent.transform.position;
                        var cell = levelGenerator.GetPosition(position);
                        var pair = new Tuple<TopDownAgent, Vector3Int>(agent, cell);
                        positions.Add(pair);
                    }
                }

                return positions;
            }
        }

        public AgentQueue agentQueue;
    }
}
