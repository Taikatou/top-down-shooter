using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public abstract class EntityMapPosition : MonoBehaviour
    {
        public abstract GridSpace GetGridSpaceType(int teamId);
    }
}
