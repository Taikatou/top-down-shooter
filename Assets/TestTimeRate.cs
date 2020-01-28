using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class TestTimeRate : MonoBehaviour
{
    void Update()
    {
        Academy.Instance.DisableAutomaticStepping();
        Academy.Instance.EnvironmentStep();
    }
}
