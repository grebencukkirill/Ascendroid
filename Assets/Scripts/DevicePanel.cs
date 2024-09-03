using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevicePanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] deviceButtons;           // Массив кнопок для устройств (6 кнопок)
    public Sprite[] deviceSprites;           // Массив спрайтов для устройств
    public TextMeshProUGUI[] deviceCountTexts; // Массив TextMeshPro для отображения количества устройств

    public Button eraseButton;               // Кнопка ластика
    public Button clearButton;               // Кнопка очистки

    [Header("Device Counts")]
    public int[] deviceCounts;               // Массив для хранения количества каждого устройства

    private Button selectedButton;           // Текущая выделенная кнопка
    private Outline selectedOutline;         // Текущий выделенный объект Outline

    void Start()
    {
        SetupDeviceButtons();
    }

    // Метод для настройки кнопок устройств в зависимости от количества
    void SetupDeviceButtons()
    {
        int activeButtonIndex = 0;

        // Проходим по каждой кнопке устройства и настраиваем её
        for (int i = 0; i < deviceCounts.Length; i++)
        {
            if (deviceCounts[i] > 0)
            {
                // Активируем кнопку и делаем её интерактивной
                deviceButtons[activeButtonIndex].gameObject.SetActive(true);
                deviceButtons[activeButtonIndex].interactable = true;

                // Находим дочерний объект UI_DeviceButton_Icon и задаем ему спрайт
                Transform iconTransform = deviceButtons[activeButtonIndex].transform.Find("UI_DeviceButton_Icon");
                if (iconTransform != null)
                {
                    Image iconImage = iconTransform.GetComponent<Image>();
                    if (iconImage != null)
                    {
                        iconImage.sprite = deviceSprites[i];
                        iconImage.enabled = true; // Включаем отображение иконки
                    }
                }

                // Устанавливаем количество устройств в TextMeshPro
                deviceCountTexts[activeButtonIndex].text = deviceCounts[i].ToString();

                // Обрабатываем клик по кнопке устройства
                int index = activeButtonIndex;  // Используем индекс кнопки, а не устройства
                deviceButtons[activeButtonIndex].onClick.AddListener(() => OnDeviceButtonClicked(deviceButtons[index], i));

                activeButtonIndex++;
            }
        }

        // Деактивируем и делаем неинтерактивными оставшиеся кнопки
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

        // Настраиваем кнопку очистки
        clearButton.onClick.AddListener(() => OnClearButtonClicked());
    }

    // Обработчик нажатия на кнопку устройства
    void OnDeviceButtonClicked(Button button, int deviceIndex)
    {
        SelectButton(button);

        // Передаем информацию в другой скрипт (например, выбранное устройство)
        Debug.Log("Selected device index: " + deviceIndex);
    }

    // Обработчик нажатия на кнопку ластика
    void OnEraseButtonClicked()
    {
        SelectButton(eraseButton);

        // Передаем информацию о том, что выбрана функция ластика
        Debug.Log("Erase mode selected");
    }

    // Обработчик нажатия на кнопку очистки
    void OnClearButtonClicked()
    {
        // Вызываем функцию очистки из другого скрипта
        Debug.Log("Clear all devices");

        // Временно выделяем кнопку очистки
        Outline outline = clearButton.GetComponent<Outline>();
        outline.enabled = true;
        Invoke(nameof(DisableClearOutline), 0.2f);  // Убираем выделение через 0.2 секунды
    }

    // Убираем выделение с кнопки очистки
    void DisableClearOutline()
    {
        clearButton.GetComponent<Outline>().enabled = false;
    }

    // Метод для выделения выбранной кнопки
    void SelectButton(Button button)
    {
        // Снимаем выделение с предыдущей кнопки
        if (selectedOutline != null)
        {
            selectedOutline.enabled = false;
        }

        // Устанавливаем выделение на новой кнопке
        selectedButton = button;
        selectedOutline = button.GetComponent<Outline>();

        if (selectedOutline != null)
        {
            selectedOutline.enabled = true;
        }
    }
}
