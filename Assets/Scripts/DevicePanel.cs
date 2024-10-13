using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevicePanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] deviceButtons;              // ������ ������ ��� ��������� (6 ������)
    public Sprite[] deviceSprites;              // ������ �������� ��� ���������
    public TextMeshProUGUI[] deviceCountTexts;  // ������ TextMeshPro ��� ����������� ���������� ���������

    public Button eraseButton;                  // ������ �������
    public Button clearButton;                  // ������ �������
    public Sprite selectedButtonSprite;         // ������ ��� ���������� ������

    [Header("Device Counts")]
    public int[] deviceCounts;                  // ������ ��� �������� ���������� ������� ����������

    private Button selectedButton;              // ������� ���������� ������
    private Sprite[] originalButtonSprites;     // ������ ��� �������� ������������ �������� ������

    private Color normalColor = new Color32(0x3A, 0x62, 0x73, 0xFF); // ������� ����
    private Color selectedColor = new Color32(0x4C, 0x8D, 0xA8, 0xFF); // ���� ���������

    void Start()
    {
        originalButtonSprites = new Sprite[deviceButtons.Length];
        SetupDeviceButtons();
    }

    // ����� ��� ��������� ������ ��������� � ����������� �� ����������
    void SetupDeviceButtons()
    {
        int activeButtonIndex = 0;

        for (int i = 0; i < deviceCounts.Length; i++)
        {
            if (deviceCounts[i] > 0)
            {
                deviceButtons[activeButtonIndex].gameObject.SetActive(true);
                deviceButtons[activeButtonIndex].interactable = true;

                Transform iconTransform = deviceButtons[activeButtonIndex].transform.Find("UI_DeviceButton_Icon");
                if (iconTransform != null)
                {
                    Image iconImage = iconTransform.GetComponent<Image>();
                    if (iconImage != null)
                    {
                        iconImage.sprite = deviceSprites[i];
                        iconImage.enabled = true; // �������� ����������� ������
                        iconImage.color = normalColor; // ������������� ����������� ����
                    }
                }

                originalButtonSprites[activeButtonIndex] = deviceButtons[activeButtonIndex].image.sprite;
                deviceCountTexts[activeButtonIndex].text = deviceCounts[i].ToString();
                deviceCountTexts[activeButtonIndex].color = normalColor; // ������������� ����������� ���� ������

                int index = activeButtonIndex;
                deviceButtons[activeButtonIndex].onClick.AddListener(() => OnDeviceButtonClicked(deviceButtons[index], i));

                activeButtonIndex++;
            }
        }

        for (int j = activeButtonIndex; j < deviceButtons.Length; j++)
        {
            deviceButtons[j].interactable = false; // ������ ������ ���������������
            Transform iconTransform = deviceButtons[j].transform.Find("UI_DeviceButton_Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = null;   // ������� ������ ����������
                    iconImage.enabled = false; // �������� Image
                }
            }
            deviceCountTexts[j].text = ""; // ������� ����� ���������� ���������
        }

        // ����������� ������ �������
        eraseButton.onClick.AddListener(() => OnEraseButtonClicked());
        clearButton.onClick.AddListener(() => OnClearButtonClicked());
    }

    // ���������� ������� �� ������ ����������
    void OnDeviceButtonClicked(Button button, int deviceIndex)
    {
        SelectButton(button);

        Debug.Log("Selected device index: " + deviceIndex);
    }

    // ���������� ������� �� ������ �������
    void OnEraseButtonClicked()
    {
        SelectButton(eraseButton);
        Debug.Log("Erase mode selected");
    }

    // ���������� ������� �� ������ �������
    void OnClearButtonClicked()
    {
        Debug.Log("Clear all devices");
    }

    void SelectButton(Button button)
    {
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
                        prevIconImage.color = normalColor; // ��������������� ���� ������
                    }
                }
                deviceCountTexts[prevIndex].color = normalColor; // ��������������� ���� ������
            }
        }

        selectedButton = button;
        int selectedIndex = System.Array.IndexOf(deviceButtons, selectedButton);

        if (selectedIndex >= 0)
        {
            Transform iconTransform = selectedButton.transform.Find("UI_DeviceButton_Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.color = selectedColor; // ������ ���� ������ �� ����������
                }
            }
            deviceCountTexts[selectedIndex].color = selectedColor; // ������ ���� ������ �� ����������
        }
    }

}
