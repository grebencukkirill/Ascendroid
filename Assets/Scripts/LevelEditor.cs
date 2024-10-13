using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    public DevicePanel devicePanel;          // ������ �� ������ ���������
    public GameObject[] devicePrefabs;       // ������ �������� ��� ������� ����������
    public LayerMask groundLayer;            // ���� ��� �������� Ground
    public LayerMask placementLayer;         // ���� ��� ��������, ��� ������ ��������� ����������

    private GameObject devicePreview;        // ������ ��� ���������������� ��������� ����������
    private int selectedDeviceIndex = -1;    // ������ ���������� ���������� �� ������ ���������

    private Color validColor = Color.green;  // ����, ����� ���������� ����� ���������
    private Color invalidColor = Color.red;  // ����, ����� ���������� ������ ���������

    void Start()
    {
        // ��� ������� ���� ���������� ����� ��������� � ������ ���� �� �����
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

    // ������������� ��������� ������
    void InitializeEditor()
    {
        selectedDeviceIndex = -1;
        devicePreview = null;
    }

    // ����� ��� ��������� ������ ���������� �� ��������
    void MoveDevicePreview()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 gridPosition = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), 0);

        // ����������� ���������� �� ��������
        if (devicePreview != null)
        {
            devicePreview.transform.position = gridPosition;
            UpdateDevicePreviewColor(gridPosition);
        }
    }

    // ���������� ����� ���������� ��� ���������� ��������� ����������� ����������
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

    // �������� ����������� ���������� ���������� �� ��������� �������
    bool CanPlaceDevice(Vector3 position)
    {
        Collider2D collision = Physics2D.OverlapCircle(position, 0.4f, placementLayer);

        if (collision != null)
        {
            // ���� ���-�� ��� ���� �� �������, �� ����� �������
            return false;
        }

        // ��������� ����������� ����������
        if (selectedDeviceIndex == 1 || selectedDeviceIndex == 2 || selectedDeviceIndex == 3) // ������� GravChange, Jump � Springboard
        {
            RaycastHit2D hitBelow = Physics2D.Raycast(position, Vector2.down, 1f, groundLayer);
            RaycastHit2D hitAbove = Physics2D.Raycast(position, Vector2.up, 1f, groundLayer);

            if (!hitBelow && !hitAbove)
            {
                return false; // ���������� ������ ����� Ground ������ ��� �����
            }
        }

        return true;
    }

    // ��������� ���������� �� ��������� �������
    void PlaceDevice()
    {
        Vector3 position = devicePreview.transform.position;

        if (CanPlaceDevice(position))
        {
            Instantiate(devicePrefabs[selectedDeviceIndex], position, Quaternion.identity);
        }
    }

    // ����� ��� ���������� ���������� ����������
    public void UpdateSelectedDevice(int deviceIndex)
    {
        if (devicePreview != null)
        {
            Destroy(devicePreview);
        }

        selectedDeviceIndex = deviceIndex;

        // ������ ������ ����������
        devicePreview = Instantiate(devicePrefabs[selectedDeviceIndex]);
        devicePreview.GetComponent<SpriteRenderer>().color = invalidColor; // ������������� ���� � �������� ����������
    }
}
