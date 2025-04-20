using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject quitConfirmPanel;
    public GameObject mainButtonGroup;
    public GameObject settingsPanel;

    public void OnLevelsPressed()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void OnSettingsPressed()
    {
        settingsPanel.SetActive(true);
        mainButtonGroup.SetActive(false);
    }

    public void OnQuitPressed()
    {
        quitConfirmPanel.SetActive(true);
        mainButtonGroup.SetActive(false);
    }

    public void OnConfirmQuit()
    {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnCancelQuit()
    {
        quitConfirmPanel.SetActive(false);
        mainButtonGroup.SetActive(true);
    }
}
