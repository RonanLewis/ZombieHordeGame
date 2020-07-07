using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameHandler : MonoBehaviour
{
    public static bool cursorVisible = false;
    public static bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = cursorVisible;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("TogglePause"))
        {
            PauseGame();
        }
    }
    public void TogglePauseGame()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            PlayerCameraController.isLocked = true;
            Time.timeScale = 0;
            EventManager.TriggerEvent("openPauseMenu");
        }
        else
        {
            PlayerCameraController.isLocked = false;
            Time.timeScale = 1;
            EventManager.TriggerEvent("closePauseMenu");
        }
        ToggleCursorLock();
    }
    public void UnPauseGame()
    {
        isPaused = false;
        PlayerCameraController.isLocked = false;
        LockCursor();
        Time.timeScale = 1;
        EventManager.TriggerEvent("closePauseMenu");
    }

    public void PauseGame()
    {
        isPaused = true;
        PlayerCameraController.isLocked = true;
        UnlockCursor();
        Time.timeScale = 0;
        EventManager.TriggerEvent("openPauseMenu");
    }
    public static void ToggleCursorLock()
    {
        Cursor.visible = !Cursor.visible;
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Confined;
        } else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public static void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

    }
    public static void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
