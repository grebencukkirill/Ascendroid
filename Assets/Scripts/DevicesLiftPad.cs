using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevicesLiftPad : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private Collider2D liftPadCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        liftPadCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            // ������ �������� � �����
            animator.Play("LiftPad_Activated");
            audioSource.Play();

            // ���������� ����������
            StartCoroutine(DisableColliderTemporarily());
        }
    }

    private IEnumerator DisableColliderTemporarily()
    {
        // ��������� ���������
        liftPadCollider.enabled = false;

        // ���� ��������� ��������
        yield return new WaitForSeconds(0.5f);

        // �������� ��������� �������
        liftPadCollider.enabled = true;
    }
}
