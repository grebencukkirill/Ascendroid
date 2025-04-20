using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevicesGravFlip : MonoBehaviour
{
    public Animator animator;
    private AudioSource audioSource;
    private Collider2D gravFlipButtonCollider;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gravFlipButtonCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            // Запуск анимации и звука
            animator.Play("GravFlipButton_Activated");
            audioSource.Play();

            // Отключение коллайдера
            StartCoroutine(DisableColliderTemporarily());
        }
    }

    private IEnumerator DisableColliderTemporarily()
    {
        // Отключаем коллайдер
        gravFlipButtonCollider.enabled = false;

        // Ждем окончания анимации
        yield return new WaitForSeconds(0.5f);

        // Включаем коллайдер обратно
        gravFlipButtonCollider.enabled = true;
    }
}
