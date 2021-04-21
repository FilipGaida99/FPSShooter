using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private static PauseManager _instance;
    public static PauseManager Instance { get { return _instance; } }

    public bool isPaused = false;

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        SetPause(isPaused);
    }

    public void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            TooglePause();
        }
    }

    public void TooglePause()
    {
        isPaused = !isPaused;
        SetToState();
    }

    public void SetPause(bool pause)
    {
        isPaused = pause;
        SetToState();
    }

    private void SetToState()
    {
        AdjustTime();
        ShowHideChilds();
        ShowHideMouse();
    }

    private void AdjustTime()
    {
        Time.timeScale = isPaused ? 0 : 1;
    }

    private void ShowHideChilds()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isPaused);
        }
    }

    private void ShowHideMouse()
    {
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

}
