using System.Collections.Generic;
using Research.CharacterDesign.Scripts;
using Research.CharacterDesign.Scripts.Environment;
using Research.LevelDesign.NuclearThrone;
using Unity.Simulation.Games;
using UnityEngine;

using UnityEngine.Analytics;

namespace Research.LevelDesign.Scripts
{
    public class AnalysisTool : MonoBehaviour
    {
        public DataLogger dataLogger;
        public NuclearThroneLevelGenerator generator;

        public void AddResult(WinLossCondition condition)
        {
            var customParams = new Dictionary<string, object>
            {
                {"map_counter", generator.instanceMapCounter}, {"condition", condition}
            };

            AnalyticsEvent.Custom("map_complete", customParams);

            if (MlLevelManager.UnitySimulation)
            {
                var valueId = condition.ToString();
                GameSimManager.Instance.IncrementCounter(valueId, 1);
            }
            else
            {
                dataLogger.AddResultAgent(condition, generator.instanceMapCounter);
            }
        }
    }
}
