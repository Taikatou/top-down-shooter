using System.Collections;
using UnityEngine;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public abstract class GetEnvironmentMapPositions : MonoBehaviour
    {
        public abstract BaseMapPosition[] EntityMapPositions { get; }

        public abstract IEnumerator Restart();
    }
}
