using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevicePanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] deviceButtons;              // Массив кнопок для устройств (6 кнопок)
    public Sprite[] deviceSprites;              // Массив спрайтов для устройств
    public TextMeshProUGUI[] deviceCountTexts;  // Массив TextMeshPro для отображения количества устройств

    public Button eraseButton;                  // Кнопка ластика
    public Button clearButton;                  // Кнопка очистки
    public Sprite selectedButtonSprite;         // Спрайт для выделенной кнопки

    [Header("Device Counts")]
    public int[] deviceCounts;                  // Массив для хранения количества каждого устройства

    private Button selectedButton;              // Текущая выделенная кнопка
    private Sprite[] originalButtonSprites;     // Массив для хранения оригинальных спрайтов кнопок

    private Color normalColor = new Color32(0x3A, 0x62, 0x73, 0xFF); // Обычный цвет
    private Color selectedColor = new Color32(0x4C, 0x8D, 0xA8, 0xFF); // Цвет выделения

    void Start()
    {
        originalButtonSprites = new Sprite[deviceButtons.Length];
        SetupDeviceButtons();
    }

    // Метод для настройки кнопок устройств в зависимости от количества
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
                        iconImage.enabled = true; // Включаем отображение иконки
                        iconImage.color = normalColor; // Устанавливаем стандартный цвет
                    }
                }

                originalButtonSprites[activeButtonIndex] = deviceButtons[activeButtonIndex].image.sprite;
                deviceCountTexts[activeButtonIndex].text = deviceCounts[i].ToString();
                deviceCountTexts[activeButtonIndex].color = normalColor; // Устанавливаем стандартный цвет текста

                int index = activeButtonIndex;
                deviceButtons[activeButtonIndex].onClick.AddListener(() => OnDeviceButtonClicked(deviceButtons[index], i));

                activeButtonIndex++;
            }
        }

        for (int j = activeButtonIndex; j < deviceButtons.Length; j++)
        {
            deviceButtons[j].interactable = false; // Делаем кнопку неинтерактивной
            Transform iconTransform = deviceButtons[j].transform.Find("UI_DeviceButton_Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = null;   // Убираем иконку устройства
                    iconImage.enabled = false; // Скрываем Image
                }
            }
            deviceCountTexts[j].text = ""; // Очищаем текст количества устройств
        }

        // Настраиваем кнопку ластика
        eraseButton.onClick.AddListener(() => OnEraseButtonClicked());
        clearButton.onClick.AddListener(() => OnClearButtonClicked());
    }

    // Обработчик нажатия на кнопку устройства
    void OnDeviceButtonClicked(Button button, int deviceIndex)
    {
        SelectButton(button);

        Debug.Log("Selected device index: " + deviceIndex);
    }

    // Обработчик нажатия на кнопку ластика
    void OnEraseButtonClicked()
    {
        SelectButton(eraseButton);
        Debug.Log("Erase mode selected");
    }

    // Обработчик нажатия на кнопку очистки
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
                        prevIconImage.color = normalColor; // Восстанавливаем цвет иконки
                    }
                }
                deviceCountTexts[prevIndex].color = normalColor; // Восстанавливаем цвет текста
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
                    iconImage.color = selectedColor; // Меняем цвет иконки на выделенный
                }
            }
            deviceCountTexts[selectedIndex].color = selectedColor; // Меняем цвет текста на выделенный
        }
    }

}
