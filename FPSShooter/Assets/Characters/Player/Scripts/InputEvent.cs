using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InputEvent
{
    public string inputKey;
    public TypeOfInput type;
    [SerializeField]
    public UnityEvent Function;

    public void Invoke()
    {
        if (DownActive || RepatableHighActive || UpActive || RepatableLowActive)
        {
            Function.Invoke();
        }
    }

    public bool DownActive => Input.GetButtonDown(inputKey) && type == TypeOfInput.Down;

    public bool RepatableHighActive => Input.GetButton(inputKey) && type == TypeOfInput.RepeatableHigh;

    public bool UpActive => Input.GetButtonUp(inputKey) && type == TypeOfInput.Up;

    public bool RepatableLowActive => !Input.GetButton(inputKey) && type == TypeOfInput.RepeatableLow;

    public enum TypeOfInput { Down, RepeatableHigh, Up, RepeatableLow};
}
