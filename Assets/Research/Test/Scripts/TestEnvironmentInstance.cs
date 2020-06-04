using System.Collections;
using Research.Common.MapSensor.GridSpaceEntity;

namespace Research.Test.Scripts
{
    public class TestEnvironmentInstance : GetEnvironmentMapPositions
    {
        public override EntityMapPosition[] EntityMapPositions => GetComponentsInChildren<EntityMapPosition>();
        public override IEnumerator Restart()
        {
            throw new System.NotImplementedException();
        }
    }
}
