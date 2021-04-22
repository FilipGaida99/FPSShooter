using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatsMenu : MonoBehaviour
{
    public int mainMenuBuildIndex;

    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timeText;

    public string waveLabel;
    public string timeLabel;
    public string timeFormat = "{0:00}:{1:00}:{2:00}";


    private void Start()
    {
        SetScore();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuBuildIndex);
    }

    public void ResetScore()
    {
        HighScoreLoader.ResetHighScore();
        SetScore();
    }

    private void SetScore()
    {
        var score = HighScoreLoader.LoadHighScore();
        waveText.text = waveLabel + score.wave;

        float time = score.time;
        int hours = TimeSpan.FromSeconds(time).Hours;
        int minutes = TimeSpan.FromSeconds(time).Minutes;
        int seconds = TimeSpan.FromSeconds(time).Seconds;
        timeText.text = timeLabel + string.Format(timeFormat, hours, minutes, seconds);
    }
}
