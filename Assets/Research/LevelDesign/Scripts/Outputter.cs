using Research.CharacterDesign.Scripts;
using Research.CharacterDesign.Scripts.Environment;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class Outputter : MonoBehaviour
    {
        public DataLogger dataLogger;
        public MapAccessor mapAccessor;
        
        public void AddResult(WinLossCondition condition)
        {
            dataLogger.AddResultAgent(condition, mapAccessor.mapId);
        }
    }
}
