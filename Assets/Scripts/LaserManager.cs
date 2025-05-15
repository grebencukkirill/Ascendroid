using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
    public static LaserManager Instance;

    private float startTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        startTime = Time.time;
    }

    // Возвращает прошедшее время с момента старта лазерного цикла
    public float GetCycleTime()
    {
        return Time.time - startTime;
    }

    // Сбросить лазерный таймер — обычно вызывается при запуске уровня
    public void ResetTimer()
    {
        startTime = Time.time;
    }
}
