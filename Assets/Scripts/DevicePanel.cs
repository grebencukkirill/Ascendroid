using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

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
    public GameObject tooltipObject;
    public TextMeshProUGUI tooltipText;
    public LocalizedString[] deviceTooltips;

    private Dictionary<int, int> deviceIndexToButtonIndex = new Dictionary<int, int>();
    private Dictionary<int, int> buttonIndexToDeviceIndex = new Dictionary<int, int>();

    private int[] initialDeviceCounts;

    public static DevicePanel Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        initialDeviceCounts = (int[])deviceCounts.Clone();
        originalButtonSprites = new Sprite[deviceButtons.Length];
        SetupDeviceButtons();
        SetupEraseAndClearTooltips();
        tooltipObject.SetActive(false);
    }

    void SetupDeviceButtons()
    {
        deviceIndexToButtonIndex.Clear();
        buttonIndexToDeviceIndex.Clear();

        int activeButtonIndex = 0;

        for (int i = 0; i < deviceCounts.Length; i++)
        {
            if (deviceCounts[i] > 0)
            {
                int buttonIndex = activeButtonIndex;
                int deviceIndex = i;

                deviceIndexToButtonIndex[deviceIndex] = buttonIndex;
                buttonIndexToDeviceIndex[buttonIndex] = deviceIndex;

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

                EventTrigger trigger = deviceButtons[buttonIndex].gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry pointerEnter = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter
                };
                pointerEnter.callback.AddListener((eventData) => ShowTooltip(deviceIndex, deviceButtons[buttonIndex]));
                trigger.triggers.Add(pointerEnter);

                EventTrigger.Entry pointerExit = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerExit
                };
                pointerExit.callback.AddListener((eventData) => HideTooltip());
                trigger.triggers.Add(pointerExit);

                activeButtonIndex++;
            }
        }

        for (int j = activeButtonIndex; j < deviceButtons.Length; j++)
        {

            deviceButtons[j].gameObject.SetActive(false);
        }

        eraseButton.onClick.AddListener(() => OnEraseButtonClicked());
        clearButton.onClick.AddListener(() => OnClearButtonClicked());
    }

    void SetupEraseAndClearTooltips()
    {
        if (deviceTooltips.Length < 2)
        {
            return;
        }

        LocalizedString eraseTooltip = deviceTooltips[deviceTooltips.Length - 2];
        LocalizedString clearTooltip = deviceTooltips[deviceTooltips.Length - 1];

        // Ластик
        EventTrigger eraseTrigger = eraseButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry eraseEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        eraseEnter.callback.AddListener((eventData) => ShowTooltip(eraseTooltip, eraseButton));
        eraseTrigger.triggers.Add(eraseEnter);

        EventTrigger.Entry eraseExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        eraseExit.callback.AddListener((eventData) => HideTooltip());
        eraseTrigger.triggers.Add(eraseExit);

        // Очистка
        EventTrigger clearTrigger = clearButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry clearEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        clearEnter.callback.AddListener((eventData) => ShowTooltip(clearTooltip, clearButton));
        clearTrigger.triggers.Add(clearEnter);

        EventTrigger.Entry clearExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        clearExit.callback.AddListener((eventData) => HideTooltip());
        clearTrigger.triggers.Add(clearExit);
    }

    public void UpdateDeviceCount(int deviceIndex, int delta)
    {
        if (deviceIndex >= 0 && deviceIndex < deviceCounts.Length)
        {
            deviceCounts[deviceIndex] += delta;

            if (deviceIndexToButtonIndex.TryGetValue(deviceIndex, out int buttonIndex))
            {
                if (deviceCounts[deviceIndex] > 0)
                {
                    deviceCountTexts[buttonIndex].text = deviceCounts[deviceIndex].ToString();
                    deviceButtons[buttonIndex].interactable = true;
                }
                else
                {
                    SetupDeviceButtons();
                }
            }
            else if (deviceCounts[deviceIndex] > 0)
            {
                SetupDeviceButtons();
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
        }
    }

    void OnEraseButtonClicked()
    {
        SelectButton(eraseButton);
        levelEditor.SetEraseMode(true);
    }

    void OnClearButtonClicked()
    {
        levelEditor.ClearAllDevices();
        if (initialDeviceCounts == null) return;

        for (int i = 0; i < deviceCounts.Length; i++)
        {
            deviceCounts[i] = initialDeviceCounts[i];
        }

        SetupDeviceButtons();
    }

    void SelectButton(Button button)
    {
        if (selectedButton != null)
        {
            int prevButtonIndex = System.Array.IndexOf(deviceButtons, selectedButton);
            if (prevButtonIndex >= 0)
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
                deviceCountTexts[prevButtonIndex].color = normalColor;
            }
        }

        selectedButton = button;

        int newButtonIndex = System.Array.IndexOf(deviceButtons, button);
        if (newButtonIndex >= 0)
        {
            Transform newIconTransform = button.transform.Find("UI_DeviceButton_Icon");
            if (newIconTransform != null)
            {
                Image newIconImage = newIconTransform.GetComponent<Image>();
                if (newIconImage != null)
                {
                    newIconImage.color = selectedColor;
                }
            }
            deviceCountTexts[newButtonIndex].color = selectedColor;
        }
    }


    public void ShowTooltip(int deviceIndex, Button button)
    {
        if (deviceIndex >= 0 && deviceIndex < deviceTooltips.Length)
        {
            deviceTooltips[deviceIndex].GetLocalizedStringAsync().Completed += handle =>
            {
                tooltipText.text = handle.Result;
                tooltipObject.SetActive(true);

                Vector3 buttonPosition = button.transform.position;
                tooltipObject.transform.position = new Vector3(buttonPosition.x, tooltipObject.transform.position.y, buttonPosition.z);
            };
        }
    }

    public void ShowTooltip(LocalizedString localizedTooltip, Button button)
    {
        localizedTooltip.GetLocalizedStringAsync().Completed += handle =>
        {
            tooltipText.text = handle.Result;
            tooltipObject.SetActive(true);

            Vector3 buttonPosition = button.transform.position;
            tooltipObject.transform.position = new Vector3(buttonPosition.x, tooltipObject.transform.position.y, buttonPosition.z);
        };
    }

    public void HideTooltip()
    {
        tooltipObject.SetActive(false);
    }
} 