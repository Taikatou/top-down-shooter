using System.Collections.Generic;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public interface ISense
    {
        float[] GetObservations();
    }

    public class ISenseMethods
    {
        public static List<float> ToArray(Vector2 position)
        {
            return new List<float> {position.x, position.y};
        }
        
        private static List<float> ToArray(Vector3 position)
        {
            return new List<float> {position.x, position.y};
        }
    }
}
