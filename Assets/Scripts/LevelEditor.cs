using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    public RobotController robotController;
    public Transform robot;
    public Vector3 startPosition;
    public Button editorToggleButton;
    public Sprite editorOnSprite;
    public Sprite editorOffSprite;

    private bool isEditorMode = true;
    private GameObject selectedDevicePrefab; // Префаб выбранного устройства
    private GameObject deviceSilhouette;     // Силуэт устройства, который следует за мышкой
    private int selectedDeviceIndex;         // Индекс выбранного устройства

    // Параметры сетки
    public float gridSize = 1.0f; // Размер клетки сетки

    void Start()
    {
        Debug.Log("Entering Editor Mode at Start");
        EnterEditorMode();
        robot.position = startPosition;
    }

    public void ToggleEditorMode()
    {
        if (isEditorMode)
        {
            Debug.Log("Exiting Editor Mode");
            ExitEditorMode();
        }
        else
        {
            Debug.Log("Entering Editor Mode");
            EnterEditorMode();
        }
    }

    void EnterEditorMode()
    {
        isEditorMode = true;
        Time.timeScale = 0f; // Ставим игру на паузу
        robot.position = startPosition;

        robotController.ResetGravity();
        robotController.ResetDirection();
        robotController.ResetPhysicsState();
        robotController.PlayAnimation("Walk");

        editorToggleButton.image.sprite = editorOnSprite;
    }

    void ExitEditorMode()
    {
        isEditorMode = false;
        Time.timeScale = 1f; // Снимаем паузу
        editorToggleButton.image.sprite = editorOffSprite;

        // Удаляем силуэт, если он существует
        if (deviceSilhouette != null)
        {
            Destroy(deviceSilhouette);
        }
    }

    public void SetSelectedDevice(GameObject devicePrefab)
    {
        selectedDevicePrefab = devicePrefab;
        Debug.Log("Selected device prefab set in LevelEditor: " + devicePrefab.name);

        // Создаем силуэт устройства
        CreateDeviceSilhouette();
    }

    void CreateDeviceSilhouette()
    {
        // Удаляем старый силуэт, если он существует
        if (deviceSilhouette != null)
        {
            Destroy(deviceSilhouette);
        }

        // Создаем новый силуэт устройства
        if (selectedDevicePrefab != null)
        {
            deviceSilhouette = Instantiate(selectedDevicePrefab);
            deviceSilhouette.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f); // Прозрачность
            deviceSilhouette.layer = LayerMask.NameToLayer("UI"); // Установите слой, чтобы избежать коллизий
        }
    }

    void Update()
    {
        if (isEditorMode && selectedDevicePrefab != null)
        {
            UpdateSilhouettePosition(); // Обновляем позицию силуэта

            // Проверка нажатия левой кнопки мыши для размещения устройства
            if (Input.GetMouseButtonDown(0))
            {
                PlaceDevice();
            }
        }
    }

    void UpdateSilhouettePosition()
    {
        // Получаем положение мыши в мировых координатах
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Привязываем силуэт к сетке с точной привязкой к центру клетки, на которую указывает курсор
        Vector3 gridPosition = new Vector3(
            Mathf.Floor(mousePosition.x / gridSize) * gridSize + (gridSize / 2),
            Mathf.Floor(mousePosition.y / gridSize) * gridSize + (gridSize / 2),
            0
        );

        // Обновляем позицию силуэта
        if (deviceSilhouette != null)
        {
            deviceSilhouette.transform.position = gridPosition;
        }
    }

    void PlaceDevice()
    {
        // Получаем текущее положение силуэта (которое уже привязано к сетке)
        Vector3 devicePosition = deviceSilhouette.transform.position;

        // Создаем экземпляр префаба на позиции силуэта
        Instantiate(selectedDevicePrefab, devicePosition, Quaternion.identity);
        Debug.Log("Device placed at: " + devicePosition);

        // Уменьшаем количество устройств, если требуется, или убираем силуэт
        // Пример: Destroy(deviceSilhouette);
    }
}
