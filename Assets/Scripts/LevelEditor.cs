using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    public RobotController robotController;  // ������ �� RobotController
    public Transform robot;                  // ������ �� ������ ������
    public Vector3 startPosition;            // ��������� ������� ��� ������
    public Button editorToggleButton;        // ������ ��� ���������/���������� ������ ���������
    public Sprite editorOnSprite;            // ������ ��� ������ � ������ ���������
    public Sprite editorOffSprite;           // ������ ��� ������ ��� ������ ���������
    public DevicePanel devicePanel;          // ������ �� ������ ���������
    public GameObject[] devicePrefabs;       // ������ �������� ��������� ��� ���������
    public GameObject deviceGhostPrefab;     // ������ ��� ����������� ������� ����������

    private bool isEditorMode = true;        // ���� ������ ���������
    private GameObject deviceGhost;          // ������ ����������
    private int selectedDeviceIndex = -1;    // ������ ���������� ����������
    private SpriteRenderer ghostRenderer;    // SpriteRenderer ��� ����������� �������

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

                // �������� �� ��������� ���������� ��� ������� ������ ������� ����
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

        // �������� � ����� (������ ������ 1x1, �������� ��� �������������)
        Vector3 gridPos = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), 0);
        deviceGhost.transform.position = gridPos;

        // �������� �� ����������� ����������
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
