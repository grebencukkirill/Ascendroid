using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    public RobotController robotController; // Ссылка на RobotController
    public Transform robot;               // Ссылка на объект робота
    public Vector3 startPosition;         // Начальная позиция для робота
    public Button editorToggleButton;     // Кнопка для включения/выключения режима редактора
    public Sprite editorOnSprite;         // Спрайт для кнопки в режиме редактора
    public Sprite editorOffSprite;        // Спрайт для кнопки вне режима редактора

    private bool isEditorMode = true;     // Флаг режима редактора

    void Start()
    {
        // Ставим игру на паузу при старте сцены
        Debug.Log("Entering Editor Mode at Start");
        EnterEditorMode();

        // Назначаем начальное положение робота
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

    // Вход в режим редактора
    void EnterEditorMode()
    {
        isEditorMode = true;
        Time.timeScale = 0f;  // Ставим игру на паузу
        robot.position = startPosition;  // Возвращаем робота на начальную позицию

        robotController.ResetGravity();
        robotController.ResetDirection();

        // Меняем спрайт кнопки на режим включенного редактора
        editorToggleButton.image.sprite = editorOnSprite;

        Debug.Log("Editor Mode is now ON");
    }

    // Выход из режима редактора
    void ExitEditorMode()
    {
        isEditorMode = false;
        Time.timeScale = 1f;  // Снимаем паузу

        // Меняем спрайт кнопки на режим выключенного редактора
        editorToggleButton.image.sprite = editorOffSprite;

        Debug.Log("Editor Mode is now OFF");
    }
}
