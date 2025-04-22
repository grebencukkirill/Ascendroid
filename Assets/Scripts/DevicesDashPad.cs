using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevicesDashPad : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            animator.Play("DashPad_Activated");
            audioSource.Play();
        }
    }
}
