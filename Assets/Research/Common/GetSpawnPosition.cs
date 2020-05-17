using UnityEngine;

namespace Research.Common
{
    public class GetSpawnPosition : MonoBehaviour
    {
        public Transform returnTransform;
        
        public Transform GetParent()
        {
            return returnTransform;
        }
    }
}
