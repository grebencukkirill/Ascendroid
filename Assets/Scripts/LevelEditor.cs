using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    public RobotController robotController; // ������ �� RobotController
    public Transform robot;               // ������ �� ������ ������
    public Vector3 startPosition;         // ��������� ������� ��� ������
    public Button editorToggleButton;     // ������ ��� ���������/���������� ������ ���������
    public Sprite editorOnSprite;         // ������ ��� ������ � ������ ���������
    public Sprite editorOffSprite;        // ������ ��� ������ ��� ������ ���������

    private bool isEditorMode = true;     // ���� ������ ���������

    void Start()
    {
        // ������ ���� �� ����� ��� ������ �����
        Debug.Log("Entering Editor Mode at Start");
        EnterEditorMode();

        // ��������� ��������� ��������� ������
        robot.position = startPosition;
    }

    public void ToggleEditorMode()
    {
        Debug.Log("Toggle Editor Mode called");
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

    // ���� � ����� ���������
    void EnterEditorMode()
    {
        isEditorMode = true;
        Time.timeScale = 0f;  // ������ ���� �� �����
        robot.position = startPosition;  // ���������� ������ �� ��������� �������

        robotController.ResetGravity();
        robotController.ResetDirection();

        // ������ ������ ������ �� ����� ����������� ���������
        editorToggleButton.image.sprite = editorOnSprite;

        Debug.Log("Editor Mode is now ON");
    }

    // ����� �� ������ ���������
    void ExitEditorMode()
    {
        isEditorMode = false;
        Time.timeScale = 1f;  // ������� �����

        // ������ ������ ������ �� ����� ������������ ���������
        editorToggleButton.image.sprite = editorOffSprite;

        Debug.Log("Editor Mode is now OFF");
    }
}
