using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    /*
    public List<DeviceInfo> devices;
    public Transform devicePanel;
    public GameObject deviceButtonPrefab;
    public Button eraseButton;
    public Button clearButton;
    public Button startButton;

    private DeviceInfo selectedDevice;
    private GameObject silhouetteInstance;
    private bool eraserMode = false;
    private Vector3 robotStartPosition;

    void Start()
    {
        robotStartPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        // Создаем кнопки для каждого устройства
        for (int i = devices.Count - 1; i >= 0; i--)
        {
            if (devices[i].deviceCount > 0)
            {
                GameObject newButton = Instantiate(deviceButtonPrefab, devicePanel);
                newButton.GetComponentInChildren<Image>().sprite = devices[i].deviceIcon;
                newButton.GetComponentInChildren<Text>().text = devices[i].deviceCount.ToString();

                int index = i;
                newButton.GetComponent<Button>().onClick.AddListener(() => SelectDevice(index));
            }
            else
            {
                devices.RemoveAt(i);
            }
        }

        // Привязываем функции к кнопкам
        eraseButton.onClick.AddListener(ActivateEraser);
        clearButton.onClick.AddListener(ClearAllDevices);
        startButton.onClick.AddListener(ToggleGameMode);
    }

    void Update()
    {
        if (selectedDevice != null)
        {
            UpdateSilhouettePosition();

            if (Input.GetMouseButtonDown(0))
            {
                PlaceDevice();
            }
        }

        if (eraserMode && Input.GetMouseButtonDown(0))
        {
            EraseDevice();
        }
    }

    void SelectDevice(int index)
    {
        eraserMode = false;
        selectedDevice = devices[index];

        if (silhouetteInstance != null)
        {
            Destroy(silhouetteInstance);
        }

        silhouetteInstance = Instantiate(selectedDevice.deviceSilhouette);
        silhouetteInstance.SetActive(true);
    }

    void ActivateEraser()
    {
        selectedDevice = null;
        eraserMode = true;

        if (silhouetteInstance != null)
        {
            Destroy(silhouetteInstance);
        }
    }

    void ClearAllDevices()
    {
        GameObject[] placedDevices = GameObject.FindGameObjectsWithTag("PlacedDevice");

        foreach (GameObject device in placedDevices)
        {
            Destroy(device);
        }
    }

    void ToggleGameMode()
    {
        bool isInEditMode = Time.timeScale == 0;

        if (isInEditMode)
        {
            Time.timeScale = 1;
            startButton.GetComponentInChildren<Text>().text = "Pause";
        }
        else
        {
            Time.timeScale = 0;
            GameObject.FindGameObjectWithTag("Player").transform.position = robotStartPosition;
            startButton.GetComponentInChildren<Text>().text = "Start";
        }
    }

    void UpdateSilhouettePosition()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 alignedPosition = new Vector3(Mathf.Round(mouseWorldPosition.x), Mathf.Round(mouseWorldPosition.y), 0);

        if (CanPlaceDevice(alignedPosition))
        {
            silhouetteInstance.transform.position = alignedPosition;
            silhouetteInstance.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            silhouetteInstance.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    bool CanPlaceDevice(Vector3 position)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(position, 0.1f);

        if (hitCollider != null)
        {
            return false;
        }

        // Если устройство имеет особые правила (например, Jump, Springboard)
        if (selectedDevice.deviceName == "Jump" || selectedDevice.deviceName == "Springboard" || selectedDevice.deviceName == "GravChange")
        {
            RaycastHit2D hitGround = Physics2D.Raycast(position, Vector2.down, 1f, LayerMask.GetMask("Ground"));

            if (!hitGround)
            {
                return false;
            }
        }

        return true;
    }

    void PlaceDevice()
    {
        if (CanPlaceDevice(silhouetteInstance.transform.position))
        {
            Instantiate(selectedDevice.devicePrefab, silhouetteInstance.transform.position, Quaternion.identity);
            selectedDevice.deviceCount--;

            if (selectedDevice.deviceCount <= 0)
            {
                Destroy(devicePanel.GetChild(devices.IndexOf(selectedDevice)).gameObject);
                devices.Remove(selectedDevice);
                selectedDevice = null;

                Destroy(silhouetteInstance);
            }
            else
            {
                devicePanel.GetChild(devices.IndexOf(selectedDevice)).GetComponentInChildren<Text>().text = selectedDevice.deviceCount.ToString();
            }
        }
    }

    void EraseDevice()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 alignedPosition = new Vector3(Mathf.Round(mouseWorldPosition.x), Mathf.Round(mouseWorldPosition.y), 0);

        Collider2D hitCollider = Physics2D.OverlapCircle(alignedPosition, 0.1f);

        if (hitCollider != null && hitCollider.CompareTag("PlacedDevice"))
        {
            Destroy(hitCollider.gameObject);

            // Найти соответствующий DeviceInfo и увеличить его количество
            DeviceInfo deviceInfo = devices.Find(d => d.devicePrefab == hitCollider.gameObject);
            if (deviceInfo != null)
            {
                deviceInfo.deviceCount++;
                devicePanel.GetChild(devices.IndexOf(deviceInfo)).GetComponentInChildren<Text>().text = deviceInfo.deviceCount.ToString();
            }
        }
    }
    */
}
