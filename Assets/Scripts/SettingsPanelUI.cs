using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SettingsPanelUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Text Switchers")]
    public TextMeshProUGUI resolutionText;
    public TextMeshProUGUI screenModeText;
    public TextMeshProUGUI framerateText;
    public TextMeshProUGUI languageText;

    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject pausePanel;
    public GameObject mainButtonGroup;

    public bool isInPauseMenu = true;

    [Header("Audio")]
    public AudioMixer audioMixer;

    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;
    private int currentScreenModeIndex = 0;
    private int currentFramerateIndex = 0;
    private int currentLanguageIndex = 0;

    private readonly LocalizedString[] screenModes = new LocalizedString[]
    {
        new LocalizedString { TableReference = "UI_Texts", TableEntryReference = "Settings_Fullscreen" },
        new LocalizedString { TableReference = "UI_Texts", TableEntryReference = "Settings_Windowed" },
        new LocalizedString { TableReference = "UI_Texts", TableEntryReference = "Settings_Borderless" }
    };
    private readonly int[] framerates = { 30, 60, 75, 120, 144 };
    private readonly string[] languages = { "EN", "RU" };

    void Start()
    {
        resolutions = new Resolution[]
        {
            new Resolution { width = 1280, height = 720 },
            new Resolution { width = 1366, height = 768 },
            new Resolution { width = 1600, height = 900 },
            new Resolution { width = 1920, height = 1080 },
            new Resolution { width = 2560, height = 1440 },
            new Resolution { width = 3840, height = 2160 },
        };

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        LoadSettings();
        UpdateAllTexts();
    }

    public void NextResolution() { currentResolutionIndex = (currentResolutionIndex + 1) % resolutions.Length; UpdateAllTexts(); }
    public void PrevResolution() { currentResolutionIndex = (currentResolutionIndex - 1 + resolutions.Length) % resolutions.Length; UpdateAllTexts(); }

    public void NextScreenMode() { currentScreenModeIndex = (currentScreenModeIndex + 1) % screenModes.Length; UpdateAllTexts(); }
    public void PrevScreenMode() { currentScreenModeIndex = (currentScreenModeIndex - 1 + screenModes.Length) % screenModes.Length; UpdateAllTexts(); }

    public void NextFramerate() { currentFramerateIndex = (currentFramerateIndex + 1) % framerates.Length; UpdateAllTexts(); }
    public void PrevFramerate() { currentFramerateIndex = (currentFramerateIndex - 1 + framerates.Length) % framerates.Length; UpdateAllTexts(); }

    public void NextLanguage() { currentLanguageIndex = (currentLanguageIndex + 1) % languages.Length; UpdateAllTexts(); }
    public void PrevLanguage() { currentLanguageIndex = (currentLanguageIndex - 1 + languages.Length) % languages.Length; UpdateAllTexts(); }

    private void UpdateLanguage()
    {
        StartCoroutine(SetLocaleCoroutine(languages[currentLanguageIndex]));
    }

    private IEnumerator SetLocaleCoroutine(string localeCode)
    {
        yield return LocalizationSettings.InitializationOperation;

        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code == localeCode.ToLower())
            {
                LocalizationSettings.SelectedLocale = locale;
                break;
            }
        }
    }

    private void UpdateAllTexts()
    {
        resolutionText.text = $"{resolutions[currentResolutionIndex].width}x{resolutions[currentResolutionIndex].height}";
        framerateText.text = framerates[currentFramerateIndex].ToString();
        languageText.text = languages[currentLanguageIndex];

        StartCoroutine(UpdateScreenModeText());
    }

    private IEnumerator UpdateScreenModeText()
    {
        var op = screenModes[currentScreenModeIndex].GetLocalizedStringAsync();
        yield return op;
        screenModeText.text = op.Result;
    }

    public void SaveSettings()
    {
        // ���������
        PlayerPrefs.SetFloat("Music", musicSlider.value);
        PlayerPrefs.SetFloat("SFX", sfxSlider.value);

        // ����������
        Resolution res = resolutions[currentResolutionIndex];
        PlayerPrefs.SetInt("ResolutionWidth", res.width);
        PlayerPrefs.SetInt("ResolutionHeight", res.height);

        // ����� ������
        PlayerPrefs.SetInt("ScreenMode", currentScreenModeIndex);

        // ������� ������
        int frameRate = framerates[currentFramerateIndex];
        PlayerPrefs.SetInt("Framerate", frameRate);

        // ����
        PlayerPrefs.SetString("Language", languages[currentLanguageIndex]);
        UpdateLanguage();

        ApplyScreenSettings();
        PlayerPrefs.Save();
    }

    private void ApplyScreenSettings()
    {
        Resolution res = resolutions[currentResolutionIndex];
        FullScreenMode mode = FullScreenMode.FullScreenWindow;

        switch (currentScreenModeIndex)
        {
            case 0: mode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: mode = FullScreenMode.Windowed; break;
            case 2: mode = FullScreenMode.FullScreenWindow; break;
        }

        Screen.SetResolution(res.width, res.height, mode);
        Application.targetFrameRate = framerates[currentFramerateIndex];
    }

    public void OnBackButton()
    {
        settingsPanel.SetActive(false);

        if (isInPauseMenu && pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        else
        {
            mainButtonGroup.SetActive(true);
        }
    }

    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("Music", 0.8f);
        float sfxVolume = PlayerPrefs.GetFloat("SFX", 0.8f);
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        SetVolume("Volume_Music", musicVolume);
        SetVolume("Volume_SFX", sfxVolume);

        int width = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height);

        bool found = false;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == width && resolutions[i].height == height)
            {
                currentResolutionIndex = i;
                found = true;
                break;
            }
        }

        if (!found)
        {
            // ��������� ������� ���������� � ������
            var resList = new List<Resolution>(resolutions);
            Resolution currentRes = new Resolution { width = width, height = height };
            resList.Insert(0, currentRes);
            resolutions = resList.ToArray();
            currentResolutionIndex = 0;
        }

        currentScreenModeIndex = PlayerPrefs.GetInt("ScreenMode", 0);

        int savedFramerate = PlayerPrefs.GetInt("Framerate", 60);
        for (int i = 0; i < framerates.Length; i++)
        {
            if (framerates[i] == savedFramerate)
            {
                currentFramerateIndex = i;
                break;
            }
        }

        string lang = PlayerPrefs.GetString("Language", "EN");
        for (int i = 0; i < languages.Length; i++)
        {
            if (languages[i] == lang)
            {
                currentLanguageIndex = i;
                break;
            }
        }
        UpdateLanguage();
    }

    public void OnMusicVolumeChanged(float value)
    {
        SetVolume("Volume_Music", value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        SetVolume("Volume_SFX", value);
    }

    private void SetVolume(string exposedParam, float value)
    {
        audioMixer.SetFloat(exposedParam, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale newLocale)
    {
        UpdateAllTexts();
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }
}
