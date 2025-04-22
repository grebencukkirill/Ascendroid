using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LevelSelectUI : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public Transform contentParent;
    public Button backButton;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("Level_1_unlocked"))
        {
            PlayerPrefs.SetInt("Level_1_unlocked", 1);
            PlayerPrefs.Save();
        }
    }

    private void Start()
    {
        backButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        GenerateLevelButtons();
    }

    void GenerateLevelButtons()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        var localizedPrefix = new LocalizedString("UI_Texts", "LevelName");

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (!sceneName.ToLower().Contains("level")) continue;
            if (sceneName == "LevelSelect") continue;

            GameObject buttonObj = Instantiate(levelButtonPrefab, contentParent);
            Button button = buttonObj.GetComponent<Button>();

            string levelNumber = System.Text.RegularExpressions.Regex.Match(sceneName, @"\d+$").Value;

            localizedPrefix.GetLocalizedStringAsync().Completed += handle =>
            {
                string prefix = handle.Result;
                buttonObj.GetComponentInChildren<TMP_Text>().text = $"{prefix} {levelNumber}";
            };

            int collected = PlayerPrefs.GetInt($"{sceneName}_capsules", 0);
            for (int j = 0; j < 3; j++)
            {
                string iconName = $"UI_LevelButton_Capsule_{j + 1}";
                Image icon = buttonObj.transform.Find(iconName).GetComponent<Image>();
                icon.enabled = j < collected;
            }

            bool unlocked = PlayerPrefs.GetInt($"{sceneName}_unlocked", i == 0 ? 1 : 0) == 1;

            if (unlocked)
            {
                button.interactable = true;
                button.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
            }
            else
            {
                button.interactable = false;
            }
        }
    }
}
