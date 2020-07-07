using Research.LevelDesign.NuclearThrone.Scripts;

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
