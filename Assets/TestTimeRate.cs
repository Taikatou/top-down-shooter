using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class TestTimeRate : MonoBehaviour
{
    void FixedUpdate()
    {
        //Academy.Instance.DisableAutomaticStepping();
        Academy.Instance.EnvironmentStep();
        
        Debug.Log(Time.deltaTime);
    }
}
