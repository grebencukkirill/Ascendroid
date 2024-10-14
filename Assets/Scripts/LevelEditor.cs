using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    public RobotController robotController;  // Ссылка на RobotController
    public Transform robot;                  // Ссылка на объект робота
    public Vector3 startPosition;            // Начальная позиция для робота
    public Button editorToggleButton;        // Кнопка для включения/выключения режима редактора
    public Sprite editorOnSprite;            // Спрайт для кнопки в режиме редактора
    public Sprite editorOffSprite;           // Спрайт для кнопки вне режима редактора
    public DevicePanel devicePanel;          // Ссылка на панель устройств
    public GameObject[] devicePrefabs;       // Массив префабов устройств для установки
    public GameObject deviceGhostPrefab;     // Префаб для отображения силуэта устройства

    private bool isEditorMode = true;        // Флаг режима редактора
    private GameObject deviceGhost;          // Силуэт устройства
    private int selectedDeviceIndex = -1;    // Индекс выбранного устройства
    private SpriteRenderer ghostRenderer;    // SpriteRenderer для отображения силуэта

    void Start()
    {
        EnterEditorMode();
        robot.position = startPosition;
    }

    void Update()
    {
        if (isEditorMode)
        {
            UpdateSelectedDevice();
            if (selectedDeviceIndex >= 0)
            {
                FollowMouse();

                // Проверка на установку устройства при нажатии правой кнопкой мыши
                if (Input.GetMouseButtonDown(1))
                {
                    TryPlaceDevice();
                }
            }
        }
    }

    public void ToggleEditorMode()
    {
        if (isEditorMode)
        {
            ExitEditorMode();
        }
        else
        {
            EnterEditorMode();
        }
    }

    void EnterEditorMode()
    {
        isEditorMode = true;
        Time.timeScale = 0f;
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
        Time.timeScale = 1f;

        editorToggleButton.image.sprite = editorOffSprite;

        if (deviceGhost != null)
        {
            Destroy(deviceGhost);
        }
    }

    void UpdateSelectedDevice()
    {
        int newDeviceIndex = devicePanel.GetSelectedDeviceIndex();
        if (newDeviceIndex != selectedDeviceIndex)
        {
            selectedDeviceIndex = newDeviceIndex;
            CreateGhostDevice();
        }
    }

    void CreateGhostDevice()
    {
        if (deviceGhost != null)
        {
            Destroy(deviceGhost);
        }

        if (selectedDeviceIndex >= 0 && selectedDeviceIndex < devicePrefabs.Length)
        {
            deviceGhost = Instantiate(deviceGhostPrefab);
            ghostRenderer = deviceGhost.GetComponent<SpriteRenderer>();
            ghostRenderer.sprite = devicePrefabs[selectedDeviceIndex].GetComponent<SpriteRenderer>().sprite;
        }
    }

    void FollowMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Привязка к сетке (размер ячейки 1x1, измените при необходимости)
        Vector3 gridPos = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), 0);
        deviceGhost.transform.position = gridPos;

        // Проверка на возможность размещения
        bool canPlace = CanPlaceDevice(gridPos);
        ghostRenderer.color = canPlace ? Color.green : Color.red;
    }

    bool CanPlaceDevice(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.4f);
        if (hit != null) return false;

        return true;
    }

    void TryPlaceDevice()
    {
        Vector3 placementPos = deviceGhost.transform.position;

        if (CanPlaceDevice(placementPos))
        {
            Instantiate(devicePrefabs[selectedDeviceIndex], placementPos, Quaternion.identity);
        }
    }
}
