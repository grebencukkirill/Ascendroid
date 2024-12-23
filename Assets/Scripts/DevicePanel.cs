using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DevicePanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] deviceButtons;
    public Sprite[] deviceSprites;
    public TextMeshProUGUI[] deviceCountTexts;

    public Button eraseButton;
    public Button clearButton;
    public Sprite selectedButtonSprite;

    [Header("Device Counts")]
    public int[] deviceCounts;

    public string[] deviceTags;

    [Header("Device Prefabs")]
    public GameObject[] devicePrefabs;
    public LevelEditor levelEditor;

    public Button selectedButton;
    private Sprite[] originalButtonSprites;

    private Color normalColor = new Color32(0x3A, 0x62, 0x73, 0xFF);
    private Color selectedColor = new Color32(0x4C, 0x8D, 0xA8, 0xFF);

    [Header("Tooltip Elements")]
    public GameObject tooltipObject;                  // Tooltip GameObject
    public TextMeshProUGUI tooltipText;               // Text inside tooltip
    public string[] deviceTooltips;                   // Array of tooltip descriptions for each device

    void Start()
    {
        originalButtonSprites = new Sprite[deviceButtons.Length];
        SetupDeviceButtons();
        SetupEraseAndClearTooltips();
        tooltipObject.SetActive(false); // Hide tooltip at start
    }

    void SetupDeviceButtons()
    {
        int activeButtonIndex = 0;

        for (int i = 0; i < deviceCounts.Length; i++)
        {
            if (deviceCounts[i] > 0)
            {
                int buttonIndex = activeButtonIndex;
                int deviceIndex = i;

                deviceButtons[buttonIndex].gameObject.SetActive(true);
                deviceButtons[buttonIndex].interactable = true;

                Transform iconTransform = deviceButtons[buttonIndex].transform.Find("UI_DeviceButton_Icon");
                if (iconTransform != null)
                {
                    Image iconImage = iconTransform.GetComponent<Image>();
                    if (iconImage != null)
                    {
                        iconImage.sprite = deviceSprites[deviceIndex];
                        iconImage.enabled = true;
                        iconImage.color = normalColor;
                    }
                }

                originalButtonSprites[buttonIndex] = deviceButtons[buttonIndex].image.sprite;
                deviceCountTexts[buttonIndex].text = deviceCounts[deviceIndex].ToString();
                deviceCountTexts[buttonIndex].color = normalColor;

                deviceButtons[buttonIndex].onClick.AddListener(() => OnDeviceButtonClicked(deviceButtons[buttonIndex], deviceIndex));

                // Add tooltip listeners
                EventTrigger trigger = deviceButtons[buttonIndex].gameObject.AddComponent<EventTrigger>();

                // PointerEnter event
                EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
                pointerEnter.eventID = EventTriggerType.PointerEnter;
                pointerEnter.callback.AddListener((eventData) => ShowTooltip(deviceIndex, deviceButtons[buttonIndex]));
                trigger.triggers.Add(pointerEnter);

                // PointerExit event
                EventTrigger.Entry pointerExit = new EventTrigger.Entry();
                pointerExit.eventID = EventTriggerType.PointerExit;
                pointerExit.callback.AddListener((eventData) => HideTooltip());
                trigger.triggers.Add(pointerExit);

                activeButtonIndex++;
            }
        }

        // Отключаем оставшиеся кнопки
        for (int j = activeButtonIndex; j < deviceButtons.Length; j++)
        {
            deviceButtons[j].interactable = false;
            Transform iconTransform = deviceButtons[j].transform.Find("UI_DeviceButton_Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = null;
                    iconImage.enabled = false;
                }
            }
            deviceCountTexts[j].text = "";
        }

        eraseButton.onClick.AddListener(() => OnEraseButtonClicked());
        clearButton.onClick.AddListener(() => OnClearButtonClicked());
    }

    void SetupEraseAndClearTooltips()
    {
        // Проверяем, что массив подсказок имеет достаточное количество элементов
        if (deviceTooltips.Length < 2)
        {
            Debug.LogError("Device tooltips array must have at least two elements for erase and clear buttons.");
            return;
        }

        // Получаем подсказки для ластика и очистки
        string eraseTooltip = deviceTooltips[deviceTooltips.Length - 2];
        string clearTooltip = deviceTooltips[deviceTooltips.Length - 1];

        // Ластик
        EventTrigger eraseTrigger = eraseButton.gameObject.AddComponent<EventTrigger>();

        // PointerEnter для ластика
        EventTrigger.Entry eraseEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        eraseEnter.callback.AddListener((eventData) => ShowTooltip(eraseTooltip, eraseButton));
        eraseTrigger.triggers.Add(eraseEnter);

        // PointerExit для ластика
        EventTrigger.Entry eraseExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        eraseExit.callback.AddListener((eventData) => HideTooltip());
        eraseTrigger.triggers.Add(eraseExit);

        // Очистка
        EventTrigger clearTrigger = clearButton.gameObject.AddComponent<EventTrigger>();

        // PointerEnter для очистки
        EventTrigger.Entry clearEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        clearEnter.callback.AddListener((eventData) => ShowTooltip(clearTooltip, clearButton));
        clearTrigger.triggers.Add(clearEnter);

        // PointerExit для очистки
        EventTrigger.Entry clearExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        clearExit.callback.AddListener((eventData) => HideTooltip());
        clearTrigger.triggers.Add(clearExit);
    }

    public void UpdateDeviceCount(int deviceIndex, int delta)
    {
        if (deviceIndex >= 0 && deviceIndex < deviceCounts.Length)
        {
            deviceCounts[deviceIndex] += delta;

            // Обновляем текстовое представление количества
            if (deviceCounts[deviceIndex] > 0)
            {
                deviceCountTexts[deviceIndex].text = deviceCounts[deviceIndex].ToString();
                deviceButtons[deviceIndex].interactable = true;
            }
            else
            {
                deviceCountTexts[deviceIndex].text = "0";
                deviceButtons[deviceIndex].interactable = false; // Отключаем кнопку, если устройств больше нет
            }
        }
    }


    void OnDeviceButtonClicked(Button button, int deviceIndex)
    {
        levelEditor.SetEraseMode(false);
        SelectButton(button);

        if (deviceIndex >= 0 && deviceIndex < devicePrefabs.Length && deviceCounts[deviceIndex] > 0)
        {
            levelEditor.SetSelectedDevice(devicePrefabs[deviceIndex]);
            Debug.Log("Selected device index: " + deviceIndex);
        }
        else
        {
            Debug.LogWarning("Device index out of bounds or no devices left.");
        }
    }

    void OnEraseButtonClicked()
    {
        SelectButton(eraseButton);
        levelEditor.SetEraseMode(true);
        Debug.Log("Erase mode selected");
    }

    void OnClearButtonClicked()
    {
        levelEditor.ClearAllDevices();
        Debug.Log("Clear all devices");
    }

    void SelectButton(Button button)
    {
        // Сбрасываем выделение с предыдущей кнопки
        if (selectedButton != null)
        {
            int prevIndex = System.Array.IndexOf(deviceButtons, selectedButton);
            if (prevIndex >= 0)
            {
                Transform prevIconTransform = selectedButton.transform.Find("UI_DeviceButton_Icon");
                if (prevIconTransform != null)
                {
                    Image prevIconImage = prevIconTransform.GetComponent<Image>();
                    if (prevIconImage != null)
                    {
                        prevIconImage.color = normalColor;
                    }
                }
                deviceCountTexts[prevIndex].color = normalColor;
            }
        }

        selectedButton = button;
    }

    // Tooltip display methods
    public void ShowTooltip(int deviceIndex, Button button)
    {
        if (deviceIndex >= 0 && deviceIndex < deviceTooltips.Length)
        {
            tooltipText.text = deviceTooltips[deviceIndex];
            tooltipObject.SetActive(true);

            Vector3 buttonPosition = button.transform.position;
            tooltipObject.transform.position = new Vector3(buttonPosition.x, buttonPosition.y + 180, buttonPosition.z);
        }
    }

    public void ShowTooltip(string tooltipMessage, Button button)
    {
        tooltipText.text = tooltipMessage;
        tooltipObject.SetActive(true);

        Vector3 buttonPosition = button.transform.position;
        tooltipObject.transform.position = new Vector3(buttonPosition.x, buttonPosition.y + 180, buttonPosition.z);
    }

    public void HideTooltip()
    {
        tooltipObject.SetActive(false);
    }
} 