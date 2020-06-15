using System.Collections;
using Research.Common.MapSensor.GridSpaceEntity;

namespace Research.Test.Scripts
{
    public class TestEnvironmentInstance : GetEnvironmentMapPositions
    {
        public override BaseMapPosition[] EntityMapPositions => GetComponentsInChildren<BaseMapPosition>();
        public override IEnumerator Restart()
        {
            throw new System.NotImplementedException();
        }
    }
}
