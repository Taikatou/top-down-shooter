using UnityEngine;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public abstract class IGetTeamId : MonoBehaviour
    {
        public abstract int GetTeamId { get; }
    }
}
