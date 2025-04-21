using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    public DevicePanel devicePanel;
    public AudioManager audioManager;

    public Animator devicePanelAnimator;

    private string[] deviceTags = { "LiftPad", "DashPad", "Redirect", "GravFlip", "AccelPad", "SlowPad" };

    public RobotController robotController;
    public Transform robot;
    public Vector3 startPosition;
    public Button editorToggleButton;
    public Sprite editorOnSprite;
    public Sprite editorOffSprite;

    private bool isSwitchingMode = false;

    private bool isEditorMode = true;
    private GameObject selectedDevicePrefab;
    private GameObject deviceSilhouette;

    private bool isEraseMode = false;
    private GameObject eraserSilhouette;
    public Sprite eraserSprite;

    public float gridSize = 1.0f;
    private Color allowedColor = new Color(0f, 1f, 0f, 0.5f);
    private Color deniedColor = new Color(1f, 0f, 0f, 0.5f);

    void Start()
    {
        startPosition = robot.position;
        if (Physics2D.gravity.y > 0)
        {
            Physics2D.gravity = new Vector2(Physics2D.gravity.x, -Physics2D.gravity.y);
        }

        devicePanel.deviceTags = deviceTags;

        audioManager.OnPlayModeReady += OnPlayModeStart;
        audioManager.OnEditModeReady += OnEditModeStart;

        EnterEditorMode(); // стартуем в редакторском режиме
    }

    public void ToggleEditorMode()
    {
        if (isSwitchingMode) return;
        isSwitchingMode = true;

        if (isEditorMode)
        {
            devicePanelAnimator.SetBool("isVisible", false);
            audioManager.RequestPlayMode();
        }
        else
        {
            devicePanelAnimator.SetBool("isVisible", true);
            audioManager.RequestEditMode();
        }
    }

    void OnPlayModeStart()
    {
        ExitEditorMode();
        isSwitchingMode = false;
    }

    void OnEditModeStart()
    {
        EnterEditorMode();
        isSwitchingMode = false;
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

        EnergyCapsuleManager.Instance?.ResetCapsules();
        DevicesAccelSlowPads.ForceStopAllSounds();

        editorToggleButton.image.sprite = editorOnSprite;
        devicePanelAnimator.SetBool("isVisible", true);

        foreach (LaserController laser in FindObjectsOfType<LaserController>())
        {
            laser.StopLaser();
        }
    }

    void ExitEditorMode()
    {
        isEditorMode = false;
        Time.timeScale = 1f;

        if (deviceSilhouette != null) Destroy(deviceSilhouette);
        if (eraserSilhouette != null) Destroy(eraserSilhouette);

        editorToggleButton.image.sprite = editorOffSprite;

        foreach (LaserController laser in FindObjectsOfType<LaserController>())
        {
            laser.StartLaser();
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
            SetSilhouetteColor(deniedColor);
            deviceSilhouette.layer = LayerMask.NameToLayer("UI");
        }
    }

    void Update()
    {
        if (isEditorMode)
        {
            if (isEraseMode)
            {
                UpdateEraserPosition();

                if (Input.GetMouseButtonDown(0))
                {
                    EraseDevice();
                }
            }
            else if (selectedDevicePrefab != null)
            {
                UpdateSilhouettePosition();

                if (Input.GetMouseButtonDown(0) && CanPlaceDevice())
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

        Vector2 targetPosition = deviceSilhouette.transform.position;
        Vector2 cellSize = new Vector2(gridSize - 0.1f, gridSize - 0.1f);

        // Проверка занятости целевой клетки
        Collider2D[] collidersAtTarget = Physics2D.OverlapBoxAll(targetPosition, cellSize, 0f);
        foreach (var col in collidersAtTarget)
        {
            if (col.gameObject != deviceSilhouette)
            {
                return false; // Если есть любой объект (включая Ground), целевая клетка занята
            }
        }

        // Проверка для Jump, GravChange и Springboard: наличие Ground сверху или снизу
        if (selectedDevicePrefab.name == "LiftPad" || selectedDevicePrefab.name == "GravFlip" || selectedDevicePrefab.name == "DashPad")
        {
            bool hasGroundBelow = false;
            bool hasGroundAbove = false;

            // Проверка на Ground снизу
            Vector2 belowPosition = targetPosition + Vector2.down * gridSize;
            Collider2D[] collidersBelow = Physics2D.OverlapBoxAll(belowPosition, cellSize, 0f);
            foreach (var col in collidersBelow)
            {
                if (col.tag == "Ground")
                {
                    hasGroundBelow = true;
                    break;
                }
            }

            // Проверка на Ground сверху
            Vector2 abovePosition = targetPosition + Vector2.up * gridSize;
            Collider2D[] collidersAbove = Physics2D.OverlapBoxAll(abovePosition, cellSize, 0f);
            foreach (var col in collidersAbove)
            {
                if (col.tag == "Ground")
                {
                    hasGroundAbove = true;
                    break;
                }
            }

            // Разрешить размещение только если есть Ground снизу или сверху
            if (!hasGroundBelow && !hasGroundAbove) return false;

            
        }

        return true; // Если все условия выполнены, разрешаем установку устройства
    }





    void AdjustSilhouetteRotation()
    {
        Vector2 checkPosition = deviceSilhouette.transform.position;
        Vector2 cellSize = new Vector2(gridSize - 0.1f, gridSize - 0.1f); // Немного меньше, чтобы избежать ложных срабатываний

        // Проверка на Ground снизу
        Vector2 belowPosition = checkPosition + Vector2.down * gridSize;
        Collider2D[] collidersBelow = Physics2D.OverlapBoxAll(belowPosition, cellSize, 0f);
        bool hasGroundBelow = false;
        foreach (var col in collidersBelow)
        {
            if (col.tag == "Ground")
            {
                hasGroundBelow = true;
                break;
            }
        }

        // Проверка на Ground сверху
        Vector2 abovePosition = checkPosition + Vector2.up * gridSize;
        Collider2D[] collidersAbove = Physics2D.OverlapBoxAll(abovePosition, cellSize, 0f);
        bool hasGroundAbove = false;
        foreach (var col in collidersAbove)
        {
            if (col.tag == "Ground")
            {
                hasGroundAbove = true;
                break;
            }
        }

        if (selectedDevicePrefab.name == "LiftPad" || selectedDevicePrefab.name == "GravFlip" || selectedDevicePrefab.name == "DashPad")
        {
            if (hasGroundBelow && !hasGroundAbove)
            {
                deviceSilhouette.transform.localScale = new Vector3(1, 1, 1); // Нормальное положение
            }
            else if (!hasGroundBelow && hasGroundAbove)
            {
                deviceSilhouette.transform.localScale = new Vector3(1, -1, 1); // Перевернутое положение
            }
            else if (hasGroundBelow && hasGroundAbove)
            {
                float mouseY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
                deviceSilhouette.transform.localScale = new Vector3(1, mouseY < checkPosition.y ? 1 : -1, 1);
            }
        }
    }

    void PlaceDevice()
    {
        Vector3 devicePosition = deviceSilhouette.transform.position;

        // Установить устройство, только если есть доступные экземпляры
        int deviceIndex = System.Array.IndexOf(devicePanel.devicePrefabs, selectedDevicePrefab);
        if (deviceIndex >= 0 && devicePanel.deviceCounts[deviceIndex] > 0)
        {
            GameObject placedDevice = Instantiate(selectedDevicePrefab, devicePosition, Quaternion.identity);
            placedDevice.transform.localScale = deviceSilhouette.transform.localScale;

            // Уменьшаем количество доступных устройств
            devicePanel.UpdateDeviceCount(deviceIndex, -1);

            Debug.Log("Device placed at: " + devicePosition);

            if (devicePanel.deviceCounts[deviceIndex] <= 0)
            {
                Destroy(deviceSilhouette);
                deviceSilhouette = null;
                selectedDevicePrefab = null;
            }
        }
        else
        {
            Debug.LogWarning("No devices left to place.");
        }
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

    public void SetEraseMode(bool enabled)
    {
        isEraseMode = enabled;

        if (isEraseMode)
        {
            CreateEraserSilhouette();
        }
        else
        {
            if (eraserSilhouette != null)
            {
                Destroy(eraserSilhouette);
            }
            selectedDevicePrefab = null; // Сбрасываем выбранное устройство
            CreateDeviceSilhouette(); // Удаляем силуэт устройства
        }
    }

    void CreateEraserSilhouette()
    {
        if (eraserSilhouette != null)
        {
            Destroy(eraserSilhouette);
        }

        // Создаем силуэт ластика
        eraserSilhouette = new GameObject("EraserSilhouette");
        SpriteRenderer sr = eraserSilhouette.AddComponent<SpriteRenderer>();
        sr.sprite = eraserSprite;
        sr.color = new Color(1f, 0f, 0f, 0.5f); // Полупрозрачный красный
        eraserSilhouette.layer = LayerMask.NameToLayer("UI");
    }

    void UpdateEraserPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        if (eraserSilhouette != null)
        {
            eraserSilhouette.transform.position = mousePosition;
        }
    }

    void EraseDevice()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 gridPosition = new Vector3(
            Mathf.Floor(mousePosition.x / gridSize) * gridSize + (gridSize / 2),
            Mathf.Floor(mousePosition.y / gridSize) * gridSize + (gridSize / 2),
            0
        );

        Vector2 cellSize = new Vector2(gridSize - 0.1f, gridSize - 0.1f);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(gridPosition, cellSize, 0f);
        foreach (var col in colliders)
        {
            if (IsDeviceTag(col.tag))
            {
                // Увеличиваем количество устройств
                int deviceIndex = System.Array.IndexOf(devicePanel.deviceTags, col.tag);
                if (deviceIndex >= 0)
                {
                    devicePanel.UpdateDeviceCount(deviceIndex, 1);
                }

                Destroy(col.gameObject);
                Debug.Log($"Device with tag {col.tag} erased at grid cell: {gridPosition}");
                break;
            }
        }
    }


    bool IsDeviceTag(string tag)
    {
        foreach (var deviceTag in deviceTags)
        {
            if (tag == deviceTag) return true;
        }
        return false;
    }

    public void ClearAllDevices()
    {
        for (int i = 0; i < deviceTags.Length; i++)
        {
            GameObject[] devices = GameObject.FindGameObjectsWithTag(deviceTags[i]);
            foreach (var device in devices)
            {
                Destroy(device);
            }
        }

        Debug.Log("All devices cleared and counts reset.");
    }

}