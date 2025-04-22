using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelEndUI : MonoBehaviour
{
    public GameObject panel;
    public Image[] capsuleImages;
    public Animator[] capsuleAnimators;

    public float delayBetweenCapsules = 0.5f;

    private string currentSceneName;

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void ShowEndScreen(int capsuleCount)
    {
        panel.SetActive(true);

        StartCoroutine(PlayCapsuleSequence(capsuleCount));
    }

    private IEnumerator PlayCapsuleSequence(int count)
    {
        for (int i = 0; i < capsuleImages.Length; i++)
        {
            capsuleImages[i].gameObject.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(0.5f);

        for (int i = 0; i < count; i++)
        {
            capsuleImages[i].gameObject.SetActive(true);
            capsuleAnimators[i].SetTrigger("Appear");

            yield return new WaitForSecondsRealtime(delayBetweenCapsules);
        }
    }

    public void RetryLevel()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;

        AudioManager audio = FindObjectOfType<AudioManager>();
        if (audio != null)
            audio.RequestEditMode(force: true);
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1f;
        if (MenuMusicManager.Instance != null)
        {
            MenuMusicManager.Instance.PlayMusic();
        }
        SceneManager.LoadScene("LevelSelect");
    }

    public void GoToNextLevel()
    {
        Time.timeScale = 1f;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            SceneManager.LoadScene("LevelSelect");
            if (MenuMusicManager.Instance != null)
            {
                MenuMusicManager.Instance.PlayMusic();
            }
        }
    }
}
