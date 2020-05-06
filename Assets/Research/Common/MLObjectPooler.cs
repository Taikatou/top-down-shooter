using MoreMountains.Tools;

namespace Research.Common
{
    public class MLObjectPooler : MMSimpleObjectPooler
    {
        public void DestroySpawnable()
        {
            foreach (var obj in _pooledGameObjects)
            {
                if (obj.gameObject)
                {
                    Destroy(obj.gameObject);
                }
            }
        }
    }
}
