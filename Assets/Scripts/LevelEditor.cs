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

    // Параметры сетки
    public float gridSize = 1.0f; // Размер клетки сетки
    private Color allowedColor = new Color(0f, 1f, 0f, 0.5f); // Зелёный для доступного размещения
    private Color deniedColor = new Color(1f, 0f, 0f, 0.5f);  // Красный для недоступного размещения

    void Start()
    {
        EnterEditorMode();
        robot.position = startPosition;
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

        if (deviceSilhouette != null)
        {
            Destroy(deviceSilhouette);
        }
    }

    public void SetSelectedDevice(GameObject devicePrefab)
    {
        selectedDevicePrefab = devicePrefab;
        CreateDeviceSilhouette();
    }

    void CreateDeviceSilhouette()
    {
        if (deviceSilhouette != null)
        {
            Destroy(deviceSilhouette);
        }

        if (selectedDevicePrefab != null)
        {
            deviceSilhouette = Instantiate(selectedDevicePrefab);
            SetSilhouetteColor(deniedColor); // Начальный цвет
            deviceSilhouette.layer = LayerMask.NameToLayer("UI"); // Слой, чтобы избежать коллизий
        }
    }

    void Update()
    {
        if (isEditorMode && selectedDevicePrefab != null)
        {
            UpdateSilhouettePosition();

            if (Input.GetMouseButtonDown(0))
            {
                if (CanPlaceDevice())
                {
                    PlaceDevice();
                }
            }
        }
    }

    void UpdateSilhouettePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 gridPosition = new Vector3(
            Mathf.Floor(mousePosition.x / gridSize) * gridSize + (gridSize / 2),
            Mathf.Floor(mousePosition.y / gridSize) * gridSize + (gridSize / 2),
            0
        );

        if (deviceSilhouette != null)
        {
            deviceSilhouette.transform.position = gridPosition;
            AdjustSilhouetteRotation();
            SetSilhouetteColor(CanPlaceDevice() ? allowedColor : deniedColor);
        }
    }

    bool CanPlaceDevice()
    {
        if (deviceSilhouette == null) return false;

        Vector2 checkPosition = deviceSilhouette.transform.position;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(checkPosition, new Vector2(gridSize, gridSize), 0f);

        bool hasGroundBelow = false;
        bool hasGroundAbove = false;

        foreach (var col in colliders)
        {
            if (col.gameObject == deviceSilhouette) continue;

            if (col.tag == "Ground")
            {
                float colCenterY = col.bounds.center.y;
                hasGroundBelow |= colCenterY < checkPosition.y;
                hasGroundAbove |= colCenterY > checkPosition.y;
            }
            else
            {
                return false; // Клетка занята другим объектом
            }
        }

        if (selectedDevicePrefab.name == "Jump" || selectedDevicePrefab.name == "GravChange" || selectedDevicePrefab.name == "Springboard")
        {
            if (!hasGroundBelow && !hasGroundAbove) return false;

            float mouseY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            deviceSilhouette.transform.localScale = new Vector3(1, mouseY < checkPosition.y ? 1 : -1, 1);
        }

        return true;
    }

    void AdjustSilhouetteRotation()
    {
        Vector2 checkPosition = deviceSilhouette.transform.position;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(checkPosition, new Vector2(gridSize, gridSize), 0f);

        bool hasGroundBelow = false;
        bool hasGroundAbove = false;

        foreach (var col in colliders)
        {
            if (col.tag == "Ground")
            {
                float colCenterY = col.bounds.center.y;
                hasGroundBelow |= colCenterY < checkPosition.y;
                hasGroundAbove |= colCenterY > checkPosition.y;
            }
        }

        if (selectedDevicePrefab.name == "Jump" || selectedDevicePrefab.name == "GravChange" || selectedDevicePrefab.name == "Springboard")
        {
            float mouseY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            deviceSilhouette.transform.localScale = new Vector3(1, mouseY < checkPosition.y && hasGroundBelow ? 1 : -1, 1);
        }
    }

    void PlaceDevice()
    {
        Vector3 devicePosition = deviceSilhouette.transform.position;
        GameObject placedDevice = Instantiate(selectedDevicePrefab, devicePosition, Quaternion.identity);
        placedDevice.transform.localScale = deviceSilhouette.transform.localScale;
        Debug.Log("Device placed at: " + devicePosition);
    }

    void SetSilhouetteColor(Color color)
    {
        if (deviceSilhouette == null) return;

        SpriteRenderer[] spriteRenderers = deviceSilhouette.GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = color;
        }
    }
}
