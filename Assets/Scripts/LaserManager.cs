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

    public float GetCycleTime()
    {
        return Time.time - startTime;
    }

    public void ResetTimer()
    {
        startTime = Time.time;
    }
}
