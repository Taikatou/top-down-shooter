﻿using Research.CharacterDesign.Scripts;
using Research.CharacterDesign.Scripts.Environment;
using Research.LevelDesign.NuclearThrone;
using Unity.Simulation.Games;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class AnalysisTool : MonoBehaviour
    {
        public DataLogger dataLogger;
        public NuclearThroneLevelGenerator generator;

        public void AddResult(WinLossCondition condition)
        {
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