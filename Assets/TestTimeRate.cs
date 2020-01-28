using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTimeRate : MonoBehaviour
{
    void Update()
    {
        if (Application.targetFrameRate != 300)
        {
            Application.targetFrameRate = 300;
        }
    }
}
