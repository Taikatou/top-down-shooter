using UnityEngine;

namespace Research.Common.Utils
{
    public static class AgentUtils
    {
        public static float GetDecision(float input, float increment=0.5f)
        {
            switch (Mathf.FloorToInt(input))
            {
                case 1:
                    // Left or Down
                    return -1;
                case 2:
                    // Right or Up
                    return 1;
                case 3:
                    return -increment;
                case 4:
                    return increment;
                case 5:
                    return -2 * increment;
                case 6:
                    return 2 * increment;
            }
            return 0;
        }
    }
}
