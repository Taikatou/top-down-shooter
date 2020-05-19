using Research.Common.MapSensor.GridSpaceEntity;

namespace Research.Test.Scripts
{
    public class TestEnvironmentInstance : GetEnvironmentMapPositions
    {
        public override EntityMapPosition[] EntityMapPositions => GetComponentsInChildren<EntityMapPosition>();
        public override void Restart()
        {
            throw new System.NotImplementedException();
        }
    }
}
