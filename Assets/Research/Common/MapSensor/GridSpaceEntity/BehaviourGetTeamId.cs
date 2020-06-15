using Unity.MLAgents.Policies;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public class BehaviourGetTeamId : IGetTeamId
    {
        public BehaviorParameters parameters;

        public override int GetTeamId => parameters.TeamId;
    }
}
