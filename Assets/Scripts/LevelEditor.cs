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
    private GameObject selectedDevicePrefab; // ������ ���������� ����������
    private int selectedDeviceIndex;         // ������ ���������� ����������

    // ��������� �����
    public float gridSize = 1.0f; // ������ ������ �����

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
        Time.timeScale = 0f; // ������ ���� �� �����
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
        Time.timeScale = 1f; // ������� �����
        editorToggleButton.image.sprite = editorOffSprite;
    }

    public void SetSelectedDevice(GameObject devicePrefab)
    {
        selectedDevicePrefab = devicePrefab;
        Debug.Log("Selected device prefab set in LevelEditor: " + devicePrefab.name);
    }

    void Update()
    {
        // �������� ������� ����� ������ ���� � ������ ��������������
        if (isEditorMode && selectedDevicePrefab != null && Input.GetMouseButtonDown(0))
        {
            PlaceDevice();
        }
    }

    void PlaceDevice()
    {
        // �������� ��������� ���� � ������� �����������
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // ��������� z-����������
        mousePosition.z = 0;

        // ����������� � �����
        Vector3 gridPosition = new Vector3(
            Mathf.Round(mousePosition.x / gridSize) * gridSize + (gridSize / 2),
            Mathf.Round(mousePosition.y / gridSize) * gridSize + (gridSize / 2),
            0
        );

        // ������� ��������� �������
        Instantiate(selectedDevicePrefab, gridPosition, Quaternion.identity);
        Debug.Log("Device placed at: " + gridPosition);
    }
}
