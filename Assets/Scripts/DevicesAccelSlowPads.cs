using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevicesAccelSlowPads : MonoBehaviour
{
    public AudioSource accelSource;
    public AudioSource slowSource;

    private static int accelCount = 0;
    private static int slowCount = 0;

    void Start()
    {
        if (accelSource == null)
        {
            GameObject accelObj = GameObject.Find("AccelAudio");
            if (accelObj != null)
                accelSource = accelObj.GetComponent<AudioSource>();
        }

        if (slowSource == null)
        {
            GameObject slowObj = GameObject.Find("SlowAudio");
            if (slowObj != null)
                slowSource = slowObj.GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Robot")) return;

        string tag = gameObject.tag;

        if (tag == "AccelPad")
        {
            accelCount++;
            if (accelCount == 1 && accelSource != null)
                accelSource.Play();
        }
        else if (tag == "SlowPad")
        {
            slowCount++;
            if (slowCount == 1 && slowSource != null)
                slowSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Robot")) return;

        string tag = gameObject.tag;

        if (tag == "AccelPad")
        {
            accelCount = Mathf.Max(0, accelCount - 1);
            if (accelCount == 0 && accelSource != null)
                accelSource.Stop();
        }
        else if (tag == "SlowPad")
        {
            slowCount = Mathf.Max(0, slowCount - 1);
            if (slowCount == 0 && slowSource != null)
                slowSource.Stop();
        }
    }

    public static void ForceStopAllSounds()
    {
        accelCount = 0;
        slowCount = 0;

        // Останавливаем звуки на всех активных объектах
        foreach (var pad in FindObjectsOfType<DevicesAccelSlowPads>())
        {
            if (pad.accelSource != null)
                pad.accelSource.Stop();
            if (pad.slowSource != null)
                pad.slowSource.Stop();
        }
    }
}
