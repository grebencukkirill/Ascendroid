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
        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            tutorialSteps[i].SetActive(i == 0);
        }
    }

    public void NextStep()
    {
        if (tutorialSteps.Length == 0)
            return;

        tutorialSteps[currentStepIndex].SetActive(false);

        currentStepIndex++;

        if (currentStepIndex < tutorialSteps.Length)
        {
            tutorialSteps[currentStepIndex].SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
