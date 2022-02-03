using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts.MLAgents;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public class HealthMapPosition : BaseMapPosition
    {
        public override EntityMapReturn[] GetGridSpaceType(int teamId)
        {
            return new[] { new EntityMapReturn { GridSpace = GridSpace.Health, Position = transform.position }};
        }
    }
}
