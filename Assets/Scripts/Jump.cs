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
            // Запуск анимации Springboard_Activated
            animator.Play("Jump_Activated");

            // Отключение коллайдера
            StartCoroutine(DisableColliderTemporarily());
        }
    }

    private IEnumerator DisableColliderTemporarily()
    {
        // Отключаем коллайдер
        jumpCollider.enabled = false;

        // Ждем окончания анимации
        yield return new WaitForSeconds(0.5f);

        // Включаем коллайдер обратно
        jumpCollider.enabled = true;
    }
}
