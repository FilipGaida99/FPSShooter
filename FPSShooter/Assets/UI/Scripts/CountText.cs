using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountText : MonoBehaviour
{
    public int NumbersCount = 3;

    protected TextMeshProUGUI textUI;
    protected int lastValue = int.MaxValue;

    private int maxValue;
    private string formatingString;
    
    public virtual void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        maxValue = (int)System.Math.Pow(10, NumbersCount);
        formatingString = "";
        for(int i=0;i< NumbersCount; i++)
        {
            formatingString += '0';
        }
    }

    public virtual void Start()
    {
        SetTextValue(0);
    }

    public virtual void SetTextValue(int value)
    {
        if (value != lastValue)
        {
            if (value < maxValue && value >= 0)
            {
                textUI.text = value.ToString(formatingString);
            }
            else
            {
                textUI.text = "\u221E";
            }
            lastValue = value;
        }
    }

    public virtual void SetTextValue(string customString)
    {
        textUI.text = customString;
        lastValue = 0;
    }

}
