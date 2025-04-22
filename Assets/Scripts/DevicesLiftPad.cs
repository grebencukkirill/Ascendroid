using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevicesLiftPad : MonoBehaviour
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
            animator.Play("LiftPad_Activated");
            audioSource.Play();
        }
    }
}
