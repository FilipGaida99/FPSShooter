using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreMenu : MonoBehaviour
{
    public TextMeshProUGUI miscText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timeText;

    public string waveLabel;
    public string timeLabel;
    public string timeFormat = "{0:00}:{1:00}:{2:00}";
    public string highScoreCongratulations;
    public List<string> randomCongratulations;

    private int wave;
    private float time;

    public void Start()
    {
        wave = GameManager.Instance.Wave;
        time = GameManager.Instance.ElapsedTime;

        SetScoreText();

        if (isNewHighScore)
        {
            miscText.text = highScoreCongratulations;
            HighScoreLoader.SaveHighScore(new HighScore(wave, time));
        }
        else
        {
            miscText.text = GetRandomCongratulations();
        }
    }

    public void GoToMainMenu()
    {
        MenuScenesManager.Instance.Load(MenuScenesManager.Scene.Menu);
    }

    private void SetScoreText()
    {
        waveText.text = waveLabel + wave;

        int hours = TimeSpan.FromSeconds(time).Hours;
        int minutes = TimeSpan.FromSeconds(time).Minutes;
        int seconds = TimeSpan.FromSeconds(time).Seconds;
        timeText.text = timeLabel + string.Format(timeFormat, hours, minutes, seconds);

    }

    private string GetRandomCongratulations()
    {
        if(randomCongratulations.Count < 1)
        {
            return "Nice!!!";
        }
        return randomCongratulations[UnityEngine.Random.Range(0, randomCongratulations.Count)];
    }

    private bool isNewHighScore { 
        get
        {
            HighScore previousHighScore = HighScoreLoader.LoadHighScore();
            return wave > previousHighScore.wave;
        }
    }
}
