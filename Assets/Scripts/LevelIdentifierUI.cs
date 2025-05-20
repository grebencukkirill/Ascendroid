using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelIdentifierUI : MonoBehaviour
{
    private TMP_Text levelText;

    void Start()
    {
        levelText = GetComponent<TMP_Text>();

        if (levelText != null)
        {
            UpdateLevelText();
            LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
        }
    }

    private void OnLanguageChanged(Locale newLocale)
    {
        UpdateLevelText();
    }

    private void UpdateLevelText()
    {
        LocalizedString localizedPrefix = new LocalizedString("UI_Texts", "LevelName");
        string sceneName = SceneManager.GetActiveScene().name;
        string levelNumber = System.Text.RegularExpressions.Regex.Match(sceneName, @"\d+$").Value;

        localizedPrefix.GetLocalizedStringAsync().Completed += handle =>
        {
            string prefix = handle.Result;
            levelText.text = $"{prefix} {levelNumber}";
        };
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    }
}
