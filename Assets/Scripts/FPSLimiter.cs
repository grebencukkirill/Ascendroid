using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public int targetFPS = 60;

    void Start()
    {
        // Установка целевой частоты кадров
        Application.targetFrameRate = targetFPS;
    }
}
