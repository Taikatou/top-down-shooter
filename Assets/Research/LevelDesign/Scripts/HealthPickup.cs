using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Characters;
using Research.CharacterDesign.Scripts.SpawnPoints;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts.MLAgents;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class HealthPickup : PickableItem, IEntityClass
    {
        public int id;
        public float addHealth = 5;
        protected override void Pick(GameObject picker)
        {
            var health = picker.GetComponent<MlHealth>();
            health.AddHealth(addHealth);
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public GridSpace GetGridSpace()
        {
            return GridSpace.Health;
        }

        public int GetId()
        {
            return id;
        }
    }
}
