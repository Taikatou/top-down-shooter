using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

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

        public List<Vector3> GetListObjects()
        {
            var returnValue = new List<Vector3>();
            foreach (var obj in _pooledGameObjects)
            {
                if (obj && obj.activeInHierarchy)
                {
                    returnValue.Add(obj.transform.position);
                }
            }

            return returnValue;
        }

        public struct PoolData
        {
            public readonly Vector2 Position;

            public readonly float Rotation;

            public PoolData(Rigidbody2D rigidBody)
            {
                Position = rigidBody.position;
                Rotation = rigidBody.rotation;
            }

            public PoolData(Vector2 position, float rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }

        public List<PoolData> GetData(Vector2 offsetPosition, int limit = 3)
        {
            var counter = 0;
            var output = new List<PoolData>();
            foreach (var obj in _pooledGameObjects)
            {
                if (obj && obj.activeInHierarchy)
                {
                    var rigidBody = obj.GetComponent<Rigidbody2D>();
                    output.Add(new PoolData(rigidBody));
                    counter++;
                    if (counter == limit)
                    {
                        break;
                    }
                }
            }
            return output;
        }

        protected override void CreateWaitingPool()
        {
            base.CreateWaitingPool();

            // _waitingPool.transform.parent = transform;
        }
    }
}
