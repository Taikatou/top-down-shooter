using Research.LevelDesign.Scripts.MLAgents;
using Unity.MLAgents.Policies;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public class AgentMapPosition : BaseMapPosition
    {
        public BehaviorParameters behavior;
        public override EntityMapReturn[] GetGridSpaceType(int teamId)
        {
            var value = GridSpace.Wall;
            if (!behavior.transform.parent.gameObject.CompareTag("Walls"))
            {
                value  =  behavior.TeamId == 0 ? GridSpace.Team1 : GridSpace.Team2;   
            }
            return new[] {new EntityMapReturn{GridSpace = value, Position = transform.position}};
        }
    }
}
