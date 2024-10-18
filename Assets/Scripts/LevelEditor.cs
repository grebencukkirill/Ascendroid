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
    }

    public void SetSelectedDevice(GameObject devicePrefab)
    {
        selectedDevicePrefab = devicePrefab;
        Debug.Log("Selected device prefab set in LevelEditor: " + devicePrefab.name);
    }

    void Update()
    {
        // Проверка нажатия левой кнопки мыши в режиме редактирования
        if (isEditorMode && selectedDevicePrefab != null && Input.GetMouseButtonDown(0))
        {
            PlaceDevice();
        }
    }

    void PlaceDevice()
    {
        // Получаем положение мыши в мировых координатах
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Отключаем z-координату
        mousePosition.z = 0;

        // Привязываем к сетке
        Vector3 gridPosition = new Vector3(
            Mathf.Round(mousePosition.x / gridSize) * gridSize + (gridSize / 2),
            Mathf.Round(mousePosition.y / gridSize) * gridSize + (gridSize / 2),
            0
        );

        // Создаем экземпляр префаба
        Instantiate(selectedDevicePrefab, gridPosition, Quaternion.identity);
        Debug.Log("Device placed at: " + gridPosition);
    }
}
