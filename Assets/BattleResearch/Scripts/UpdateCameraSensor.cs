using System.Linq;
using MLAgents.Sensor;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public class UpdateCameraSensor : MonoBehaviour
    {
        public CameraSensorComponent cameraSensor;
        // Start is called before the first frame update
        void Start()
        {
            var playerIds = FindObjectsOfType<CameraPlayerId>();

            var playerId = playerIds.SingleOrDefault(id => id.playerId == GetComponent<Character>().PlayerID);

            if (playerId != null)
            {
                var cameraComponent = playerId.GetComponent<Camera>();
                if (cameraSensor && cameraComponent)
                {
                    cameraSensor.camera = cameraComponent;
                }
            }
        }
    }
}
