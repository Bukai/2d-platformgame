using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        if (Application.isMobilePlatform)
            QualitySettings.vSyncCount = 0;
    }
}
