using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HighScore
{
    public int wave;
    public float time;

    public HighScore(int _wave, float _time)
    {
        wave = _wave;
        time = _time;
    }
}
