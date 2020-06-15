using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Characters;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class HealthPickup : PickableItem
    {
        public float addHealth = 5;
        protected override void Pick(GameObject picker)
        {
            var health = picker.GetComponent<MlHealth>();
            health.AddHealth(addHealth);
        }
    }
}
