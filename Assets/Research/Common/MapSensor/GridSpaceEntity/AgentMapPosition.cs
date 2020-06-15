using System.Collections.Generic;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Policies;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public class AgentMapPosition : BaseMapPosition
    {
        public IGetTeamId getTeamId;
        public override IEnumerable<EntityMapReturn> GetGridSpaceType(int teamId)
        {
            var value  =  getTeamId.GetTeamId == teamId ? GridSpace.Team1 : GridSpace.Team2;
            return new[] {new EntityMapReturn{GridSpace = value, Position = transform.position}};
        }
    }
}
