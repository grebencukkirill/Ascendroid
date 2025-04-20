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
            // Запуск анимации и звука
            animator.Play("LiftPad_Activated");
            audioSource.Play();

            // Отключение коллайдера
            StartCoroutine(DisableColliderTemporarily());
        }
    }

    private IEnumerator DisableColliderTemporarily()
    {
        // Отключаем коллайдер
        liftPadCollider.enabled = false;

        // Ждем окончания анимации
        yield return new WaitForSeconds(0.5f);

        // Включаем коллайдер обратно
        liftPadCollider.enabled = true;
    }
}
