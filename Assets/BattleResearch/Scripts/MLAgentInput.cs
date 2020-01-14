using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Assets.BattleResearch.Scripts
{
    public class MLAgentInput : MonoBehaviour
    {
        private InputManager _inputManager;
        private void Start()
        {
            var character = GetComponent<Character>();
            _inputManager = character.LinkedInputManager;
        }
    }
}
