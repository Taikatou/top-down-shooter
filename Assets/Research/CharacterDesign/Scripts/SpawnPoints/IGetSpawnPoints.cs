using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.SpawnPoints
{
    public interface IEntityClass
    {
        void SetId(int id);
        GridSpace GetGridSpace();
        int GetId();
    }

    public abstract class GetSpawnPoints<T> : MonoBehaviour where T : MonoBehaviour, IEntityClass
    {
        public abstract T[] Points { get; }
    }
}
