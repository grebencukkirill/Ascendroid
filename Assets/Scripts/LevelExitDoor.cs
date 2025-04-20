using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitDoor : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            animator.SetTrigger("Open");
            audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            animator.SetTrigger("Close");
            audioSource.Play();
        }
    }
}
