using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager
{
    private static ScenesManager instance;

    public static ScenesManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ScenesManager();

            return instance;
        }
    }

    public enum Scene { Menu, LoadingScreen, FirstLevel }

    private Action onSManagerCallback;

    public void Load(Scene scene)
    {
        // Set the SManager callback action to load the target scene
        onSManagerCallback = () =>
        {
            SceneManager.LoadScene(scene.ToString());
        };

        SceneManager.LoadScene(Scene.LoadingScreen.ToString());
    }

    public void SManagerCallback()
    {
        if(onSManagerCallback != null)
        {
            onSManagerCallback();
            onSManagerCallback = null;
        }
    }
}
