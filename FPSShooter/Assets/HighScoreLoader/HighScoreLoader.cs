using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class HighScoreLoader
{
    const string fileName = "HighScore.dat";
    static readonly string filePath = Application.persistentDataPath + "/" + fileName;

    public static void SaveHighScore(HighScore score)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);
        bf.Serialize(file, score);
        file.Close();
    }

    public static HighScore LoadHighScore()
    {
        HighScore score = new HighScore(0, 0);

        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            score = (HighScore)bf.Deserialize(file);
            file.Close();
        }

        return score;
    }

    public static void ResetHighScore()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
