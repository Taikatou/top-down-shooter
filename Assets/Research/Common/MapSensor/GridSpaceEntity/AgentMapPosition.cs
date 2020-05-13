using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Policies;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public class AgentMapPosition : EntityMapPosition
    {
        public BehaviorParameters behaviorParameters;
        public override GridSpace GetGridSpaceType()
        {
            return behaviorParameters.TeamId == 0 ? GridSpace.Team1 : GridSpace.Team2;
        }
    }
}
