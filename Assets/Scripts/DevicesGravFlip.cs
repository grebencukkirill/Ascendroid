using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevicesGravFlip : MonoBehaviour
{
    public Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            animator.Play("GravFlipButton_Activated");
            audioSource.Play();
        }
    }
}
