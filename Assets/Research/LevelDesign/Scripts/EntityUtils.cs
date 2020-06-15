using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class EntityUtils : MonoBehaviour
    {
        public static float GetDistance(Vector3Int item, Vector3Int secondItem)
        {
            var xDiff = Mathf.Pow(item.x - secondItem.x, 2);
            var yDiff = Mathf.Pow(item.y - secondItem.y, 2);
            var distance = Mathf.Sqrt(xDiff + yDiff);
            return distance;
        }
    }
}
