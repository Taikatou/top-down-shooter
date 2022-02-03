using Unity.MLAgents.Policies;
using UnityEngine;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public class GetTeamID : MonoBehaviour
    {
        public BehaviorParameters behaviorParameters;
        public int TeamId => behaviorParameters.TeamId;
    }
}
