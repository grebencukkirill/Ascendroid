using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    private Animator animator;
    private Collider2D jumpCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        jumpCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            // ������ �������� Springboard_Activated
            animator.Play("Jump_Activated");

            // ���������� ����������
            StartCoroutine(DisableColliderTemporarily());
        }
    }

    private IEnumerator DisableColliderTemporarily()
    {
        // ��������� ���������
        jumpCollider.enabled = false;

        // ���� ��������� ��������
        yield return new WaitForSeconds(0.5f);

        // �������� ��������� �������
        jumpCollider.enabled = true;
    }
}
