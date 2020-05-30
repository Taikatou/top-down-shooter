using System.Collections.Generic;
using Research.CharacterDesign.Scripts;
using Research.CharacterDesign.Scripts.Environment;
using Research.LevelDesign.NuclearThrone;
using Unity.Simulation.Games;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class AnalysisTool : MonoBehaviour
    {
        public static readonly bool UnitySimulation = false;
        public DataLogger dataLogger;
        public MapAccessor mapAccessor;

        public void AddResult(WinLossCondition condition)
        {
            if (UnitySimulation)
            {
                var valueId = condition.ToString();
                GameSimManager.Instance.IncrementCounter(valueId, 1);
            }
            else
            {
                dataLogger.AddResultAgent(condition, NuclearThroneLevelGenerator.MapIntCounter);   
            }
        }
    }
}
