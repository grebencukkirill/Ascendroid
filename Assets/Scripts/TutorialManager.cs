using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Screens")]
    public GameObject[] tutorialSteps;

    private int currentStepIndex = 0;

    void Start()
    {
        // Деактивируем все шаги, кроме первого
        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            tutorialSteps[i].SetActive(i == 0);
        }
    }

    // Вызывается кнопкой "Далее"
    public void NextStep()
    {
        if (tutorialSteps.Length == 0)
            return;

        // Деактивируем текущее окно
        tutorialSteps[currentStepIndex].SetActive(false);

        currentStepIndex++;

        // Если это не последний шаг — активируем следующий
        if (currentStepIndex < tutorialSteps.Length)
        {
            tutorialSteps[currentStepIndex].SetActive(true);
        }
        else
        {
            // Все шаги пройдены — отключаем панель туториала
            gameObject.SetActive(false);
        }
    }
}
