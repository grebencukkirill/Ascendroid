using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public int targetFPS = 60;

    void Start()
    {
        int savedFPS = PlayerPrefs.GetInt("Framerate", 60);
        Application.targetFrameRate = savedFPS;
    }
}
