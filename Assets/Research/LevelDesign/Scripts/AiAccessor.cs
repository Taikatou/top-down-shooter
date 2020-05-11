using System;
using System.Collections.Generic;
using Research.CharacterDesign.Scripts;
using Research.Common;
using Research.LevelDesign.NuclearThrone;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class AiAccessor : MonoBehaviour
    {
    
        public GameObject agentParent;

        public IEnumerable<Tuple<AgentPosition, Vector3Int>> AgentPosition
        {
            get
            {
                var agents = agentParent.GetComponentsInChildren<AgentPosition>();
                var positions = new List<Tuple<AgentPosition, Vector3Int>>();
                foreach (var agent in agents)
                {
                    if (agent.enabled)
                    {
                        var position = agent.transform.position;
                        var cell = mapAccessor.GetPosition(position);
                        var pair = new Tuple<AgentPosition, Vector3Int>(agent, cell);
                        positions.Add(pair);
                    }
                }

                return positions;
            }
        }
    }
}
