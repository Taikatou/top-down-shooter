using System.Collections.Generic;
using Research.CharacterDesign.Scripts;
using UnityEngine;

namespace Research.Common
{
    public class EnabledCharactersComponent : MonoBehaviour
    {
        public List<TopDownAgent> availableCharacters;

        public bool debugPrimaryMovement;
        
        private void Update()
        {
            if (debugPrimaryMovement)
            {
                var debug = "";
                foreach (var player in availableCharacters)
                {
                    debug += player.inputManager.PrimaryMovement + "\t";
                }
                Debug.Log(debug);   
            }
        }
    }
}
