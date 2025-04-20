using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevicesDashPad : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private Collider2D dashPadCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        dashPadCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            animator.Play("DashPad_Activated");
            audioSource.Play();
            StartCoroutine(DisableColliderTemporarily());
        }
    }

    private IEnumerator DisableColliderTemporarily()
    {
        dashPadCollider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        dashPadCollider.enabled = true;
    }
}
