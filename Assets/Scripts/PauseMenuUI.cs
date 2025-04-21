using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject confirmPanel;
    public GameObject settingsPanel;
    public Button confirmButton;

    private float previousTimeScale = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pausePanel.activeSelf && !settingsPanel.activeSelf && !confirmPanel.activeSelf)
                PauseGame();
            else if (pausePanel.activeSelf)
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        // Сохраняем текущее состояние времени
        previousTimeScale = Time.timeScale;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        confirmPanel.SetActive(false);
        settingsPanel.SetActive(false);

        // Возвращаем предыдущее состояние
        Time.timeScale = previousTimeScale;
    }

    public void OnSettingsButton()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnLevelSelectButton()
    {
        pausePanel.SetActive(false);
        confirmPanel.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirmLevelSelect);
    }

    public void OnExitButton()
    {
        pausePanel.SetActive(false);
        confirmPanel.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirmExit);
    }

    public void OnConfirmLevelSelect()
    {
        Time.timeScale = 1f;
        if (MenuMusicManager.Instance != null)
        {
            MenuMusicManager.Instance.PlayMusic();
        }
        SceneManager.LoadScene("LevelSelect");
    }

    public void OnConfirmExit()
    {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnCancelConfirm()
    {
        confirmPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}
