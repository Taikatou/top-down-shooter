using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class ProfileChecker : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log("FixedTime: " + Time.fixedTime);
            Debug.Log("DeltaTime: " + Time.deltaTime);
            
        }
    }
}
