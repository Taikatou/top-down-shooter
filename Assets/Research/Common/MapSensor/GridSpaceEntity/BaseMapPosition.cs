using System.Collections.Generic;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public struct EntityMapReturn
    {
        public GridSpace GridSpace;
        public Vector3 Position;
    }
    public abstract class BaseMapPosition : MonoBehaviour
    {
        public abstract IEnumerable<EntityMapReturn> GetGridSpaceType(int teamId);
    }
}
