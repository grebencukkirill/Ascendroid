using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevicesRedirect : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            audioSource.Play();
        }
    }
}
