using UnityEngine;

namespace Research.Common.Utils
{
    public static class AgentUtils
    {
        public static float GetDecision(float input, AimControl aimControl)
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
                    return -GetIncrement(aimControl);
                case 4:
                    return GetIncrement(aimControl);
                case 5:
                    return -2 * GetIncrement(aimControl);
                case 6:
                    return 2 * GetIncrement(aimControl);
            }
            return 0;
        }
        
        private static float GetIncrement(AimControl aimControl)
        {
            switch (aimControl)
            {
                case AimControl.SixTeenWay:
                    return 0.5f;
                case AimControl.ThirtyTwoWay:
                    return 0.33f;
            }
            return 1;
        }
    }
}
