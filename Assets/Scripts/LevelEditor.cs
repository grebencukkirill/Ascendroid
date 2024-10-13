using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    public DevicePanel devicePanel;          // Ссылка на панель устройств
    public GameObject[] devicePrefabs;       // Массив префабов для каждого устройства
    public LayerMask groundLayer;            // Слой для проверки Ground
    public LayerMask placementLayer;         // Слой для проверки, где нельзя размещать устройства

    private GameObject devicePreview;        // Объект для предварительного просмотра устройства
    private int selectedDeviceIndex = -1;    // Индекс выбранного устройства из панели устройств

    private Color validColor = Color.green;  // Цвет, когда устройство можно поставить
    private Color invalidColor = Color.red;  // Цвет, когда устройство нельзя поставить

    void Start()
    {
        // При запуске игры активируем режим редактора и ставим игру на паузу
        Time.timeScale = 0f;
        InitializeEditor();
    }

    void Update()
    {
        if (selectedDeviceIndex >= 0)
        {
            MoveDevicePreview();
            if (Input.GetMouseButtonDown(0))
            {
                PlaceDevice();
            }
        }
    }

    // Инициализация редактора уровня
    void InitializeEditor()
    {
        selectedDeviceIndex = -1;
        devicePreview = null;
    }

    // Метод для установки превью устройства за курсором
    void MoveDevicePreview()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 gridPosition = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), 0);

        // Перемещение устройства за курсором
        if (devicePreview != null)
        {
            devicePreview.transform.position = gridPosition;
            UpdateDevicePreviewColor(gridPosition);
        }
    }

    // Обновление цвета устройства для визуальной индикации возможности размещения
    void UpdateDevicePreviewColor(Vector3 position)
    {
        SpriteRenderer spriteRenderer = devicePreview.GetComponent<SpriteRenderer>();
        if (CanPlaceDevice(position))
        {
            spriteRenderer.color = validColor;
        }
        else
        {
            spriteRenderer.color = invalidColor;
        }
    }

    // Проверка возможности размещения устройства на выбранной позиции
    bool CanPlaceDevice(Vector3 position)
    {
        Collider2D collision = Physics2D.OverlapCircle(position, 0.4f, placementLayer);

        if (collision != null)
        {
            // Если что-то уже есть на позиции, не можем ставить
            return false;
        }

        // Проверяем специальные устройства
        if (selectedDeviceIndex == 1 || selectedDeviceIndex == 2 || selectedDeviceIndex == 3) // Индексы GravChange, Jump и Springboard
        {
            RaycastHit2D hitBelow = Physics2D.Raycast(position, Vector2.down, 1f, groundLayer);
            RaycastHit2D hitAbove = Physics2D.Raycast(position, Vector2.up, 1f, groundLayer);

            if (!hitBelow && !hitAbove)
            {
                return false; // Устройство должно иметь Ground сверху или снизу
            }
        }

        return true;
    }

    // Установка устройства на выбранной позиции
    void PlaceDevice()
    {
        Vector3 position = devicePreview.transform.position;

        if (CanPlaceDevice(position))
        {
            Instantiate(devicePrefabs[selectedDeviceIndex], position, Quaternion.identity);
        }
    }

    // Метод для обновления выбранного устройства
    public void UpdateSelectedDevice(int deviceIndex)
    {
        if (devicePreview != null)
        {
            Destroy(devicePreview);
        }

        selectedDeviceIndex = deviceIndex;

        // Создаём силуэт устройства
        devicePreview = Instantiate(devicePrefabs[selectedDeviceIndex]);
        devicePreview.GetComponent<SpriteRenderer>().color = invalidColor; // Устанавливаем цвет в качестве начального
    }
}
