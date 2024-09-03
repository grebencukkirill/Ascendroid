using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Springboard : MonoBehaviour
{
    private Animator animator;
    private Collider2D springboardCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        springboardCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            // ������ �������� Springboard_Activated
            animator.Play("Springboard_Activated");

            // ���������� ����������
            StartCoroutine(DisableColliderTemporarily());
        }
    }

    private IEnumerator DisableColliderTemporarily()
    {
        // ��������� ���������
        springboardCollider.enabled = false;

        // ���� ��������� ��������
        yield return new WaitForSeconds(0.5f);

        // �������� ��������� �������
        springboardCollider.enabled = true;
    }
}
