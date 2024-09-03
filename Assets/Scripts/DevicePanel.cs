using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevicePanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] deviceButtons;           // ������ ������ ��� ��������� (6 ������)
    public Sprite[] deviceSprites;           // ������ �������� ��� ���������
    public TextMeshProUGUI[] deviceCountTexts; // ������ TextMeshPro ��� ����������� ���������� ���������

    public Button eraseButton;               // ������ �������
    public Button clearButton;               // ������ �������

    [Header("Device Counts")]
    public int[] deviceCounts;               // ������ ��� �������� ���������� ������� ����������

    private Button selectedButton;           // ������� ���������� ������
    private Outline selectedOutline;         // ������� ���������� ������ Outline

    void Start()
    {
        SetupDeviceButtons();
    }

    // ����� ��� ��������� ������ ��������� � ����������� �� ����������
    void SetupDeviceButtons()
    {
        int activeButtonIndex = 0;

        // �������� �� ������ ������ ���������� � ����������� �
        for (int i = 0; i < deviceCounts.Length; i++)
        {
            if (deviceCounts[i] > 0)
            {
                // ���������� ������ � ������ � �������������
                deviceButtons[activeButtonIndex].gameObject.SetActive(true);
                deviceButtons[activeButtonIndex].interactable = true;

                // ������� �������� ������ UI_DeviceButton_Icon � ������ ��� ������
                Transform iconTransform = deviceButtons[activeButtonIndex].transform.Find("UI_DeviceButton_Icon");
                if (iconTransform != null)
                {
                    Image iconImage = iconTransform.GetComponent<Image>();
                    if (iconImage != null)
                    {
                        iconImage.sprite = deviceSprites[i];
                        iconImage.enabled = true; // �������� ����������� ������
                    }
                }

                // ������������� ���������� ��������� � TextMeshPro
                deviceCountTexts[activeButtonIndex].text = deviceCounts[i].ToString();

                // ������������ ���� �� ������ ����������
                int index = activeButtonIndex;  // ���������� ������ ������, � �� ����������
                deviceButtons[activeButtonIndex].onClick.AddListener(() => OnDeviceButtonClicked(deviceButtons[index], i));

                activeButtonIndex++;
            }
        }

        // ������������ � ������ ���������������� ���������� ������
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

        // ����������� ������ �������
        clearButton.onClick.AddListener(() => OnClearButtonClicked());
    }

    // ���������� ������� �� ������ ����������
    void OnDeviceButtonClicked(Button button, int deviceIndex)
    {
        SelectButton(button);

        // �������� ���������� � ������ ������ (��������, ��������� ����������)
        Debug.Log("Selected device index: " + deviceIndex);
    }

    // ���������� ������� �� ������ �������
    void OnEraseButtonClicked()
    {
        SelectButton(eraseButton);

        // �������� ���������� � ���, ��� ������� ������� �������
        Debug.Log("Erase mode selected");
    }

    // ���������� ������� �� ������ �������
    void OnClearButtonClicked()
    {
        // �������� ������� ������� �� ������� �������
        Debug.Log("Clear all devices");

        // �������� �������� ������ �������
        Outline outline = clearButton.GetComponent<Outline>();
        outline.enabled = true;
        Invoke(nameof(DisableClearOutline), 0.2f);  // ������� ��������� ����� 0.2 �������
    }

    // ������� ��������� � ������ �������
    void DisableClearOutline()
    {
        clearButton.GetComponent<Outline>().enabled = false;
    }

    // ����� ��� ��������� ��������� ������
    void SelectButton(Button button)
    {
        // ������� ��������� � ���������� ������
        if (selectedOutline != null)
        {
            selectedOutline.enabled = false;
        }

        // ������������� ��������� �� ����� ������
        selectedButton = button;
        selectedOutline = button.GetComponent<Outline>();

        if (selectedOutline != null)
        {
            selectedOutline.enabled = true;
        }
    }
}
