using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.Common
{
    public abstract class AbstractAgentSense
    {
        public Character Agent;

        private Health _health;
        private MLObjectPooler _pool;

        protected Health Health
        {
            get
            {
                if (!Agent)
                {
                    return null;
                }
                if (_health == null)
                {
                    _health = Agent.GetComponent<Health>();
                }
                return _health;
            }
        }

        private MLObjectPooler Pool
        {
            get
            {
                if (!Agent)
                {
                    return null;
                }
                if (_pool == null)
                {
                    _pool = Agent.GetComponentInChildren<MLObjectPooler>();
                }
                return _pool;
            }
        }

        protected List<MLObjectPooler.PoolData> PoolData(Vector2 relativePosition)
        {
            return Pool == null ? new List<MLObjectPooler.PoolData>() : Pool.GetData(relativePosition);
        }
    }
    
    public class AgentSense : AbstractAgentSense
    {
        private readonly int _projectileCount = 2;
        private float GetHealth()
        {
            return Health? (Health.CurrentHealth / Health.MaximumHealth) : 0.0f;
        }

        public List<float> GetPooledStats(Vector2 relativePosition)
        {
            var projectiles = PoolData(relativePosition);
            while (projectiles.Count < _projectileCount)
            {
                var blankProj = new MLObjectPooler.PoolData(new Vector2(), 0.0f);
                projectiles.Add(blankProj);
            }
            projectiles = projectiles.GetRange(0, _projectileCount);
            var stats = new List<float>();
            {
                stats.Add(GetHealth());
                foreach (var projectile in projectiles)
                {
                    stats.Add(projectile.Position.x);
                    stats.Add(projectile.Position.y);
                    stats.Add(projectile.Rotation);
                }   
            }
            return stats;
        }
    }
}
