using System.Collections;
using UnityEngine;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public abstract class GetEnvironmentMapPositions : MonoBehaviour
    {
        public abstract EntityMapPosition[] EntityMapPositions { get; }

        public abstract IEnumerator Restart();
    }
}
