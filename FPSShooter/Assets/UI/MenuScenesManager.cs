using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScenesManager
{
    private static MenuScenesManager instance;

    public static MenuScenesManager Instance
    {
        get
        {
            if (instance == null)
                instance = new MenuScenesManager();

            return instance;
        }
    }

    public enum Scene { Menu = 0, LoadingScreen = 1, ScoreMenu = 2, StatsMenu = 3}

    private Action onSManagerCallback;

    public void LoadWithSplash(Scene scene)
    {
        LoadWithSplash((int)scene);
    }

    public void LoadWithSplash(int sceneBuildIndex)
    {
        // Set the SManager callback action to load the target scene
        onSManagerCallback = () =>
        {
            SceneManager.LoadScene(sceneBuildIndex);
        };

        SceneManager.LoadScene((int)Scene.LoadingScreen);
    }

    public void Load(Scene scene)
    {
        SceneManager.LoadScene((int)scene);
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
