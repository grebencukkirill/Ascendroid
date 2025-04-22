using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelIdentifierUI : MonoBehaviour
{
    private TMP_Text levelText; // Текст будет автоматически найден на этом же объекте

    void Start()
    {
        // Получаем компонент TMP_Text на этом объекте
        levelText = GetComponent<TMP_Text>();

        if (levelText != null)
        {
            // Изначально обновляем текст
            UpdateLevelText();

            // Подписываемся на событие изменения языка
            LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
        }
    }

    private void OnLanguageChanged(Locale newLocale)
    {
        // При изменении языка обновляем текст
        UpdateLevelText();
    }

    private void UpdateLevelText()
    {
        // Получаем локализованное слово для "Level" / "Уровень"
        LocalizedString localizedPrefix = new LocalizedString("UI_Texts", "LevelName");

        // Получаем имя текущей сцены
        string sceneName = SceneManager.GetActiveScene().name;

        // Извлекаем номер уровня из имени сцены (например, Level_1 -> 1)
        string levelNumber = System.Text.RegularExpressions.Regex.Match(sceneName, @"\d+$").Value;

        // Получаем локализованный текст и обновляем текст на UI
        localizedPrefix.GetLocalizedStringAsync().Completed += handle =>
        {
            string prefix = handle.Result;
            levelText.text = $"{prefix} {levelNumber}";
        };
    }

    private void OnDestroy()
    {
        // Отписываемся от события изменения языка при уничтожении объекта
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    }
}
