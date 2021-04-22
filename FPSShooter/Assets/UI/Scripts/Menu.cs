using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public List<LevelScene> possibleLevels;

    private int choosedMapIndex;

    public void Awake()
    {
        choosedMapIndex = 0;
    }

    public void Start()
    {
        var levelOptions = new List<TMP_Dropdown.OptionData>();
        foreach(var level in possibleLevels)
        {
            levelOptions.Add(new TMP_Dropdown.OptionData(level.DescriptiveName, level.captionSprite));
        }
        dropdown.AddOptions(levelOptions);
    }

    public void Play()
    {
        if(choosedMapIndex == 0)
        {
            //Get random map.
            choosedMapIndex = Random.Range(0, possibleLevels.Count) + 1;
        }
        choosedMapIndex--;

        MenuScenesManager.Instance.LoadWithSplash(possibleLevels[choosedMapIndex].buildIndex);
    }

    public void Stats()
    {
        MenuScenesManager.Instance.Load(MenuScenesManager.Scene.StatsMenu);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ChangeMap(int newIndex)
    {
        choosedMapIndex = newIndex;
    }

    [System.Serializable]
    public class LevelScene
    {
        public string DescriptiveName;
        public Sprite captionSprite;
        public int buildIndex;
    }

}

