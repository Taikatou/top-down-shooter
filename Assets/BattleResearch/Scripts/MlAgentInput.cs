using UnityEngine;

namespace BattleResearch.Scripts
{
    public class MlAgentInput : MonoBehaviour
    {
        [HideInInspector] public Vector2 PrimaryInput { get; set; }

        [HideInInspector] public Vector2 SecondaryInput { get; set; }
    }
}
