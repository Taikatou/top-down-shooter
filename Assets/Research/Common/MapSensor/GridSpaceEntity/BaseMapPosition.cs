using Research.LevelDesign.Scripts.MLAgents;
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
        public abstract EntityMapReturn[] GetGridSpaceType(int teamId);
    }
}
