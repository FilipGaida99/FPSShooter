using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InputEvent
{
    public string inputKey;
    public bool Repeatable;
    [SerializeField]
    public UnityEvent Function;

    public void Invoke()
    {
        if ((Input.GetButton(inputKey) && Repeatable) || (Input.GetButtonDown(inputKey) && !Repeatable))
        {
            Function.Invoke();
        }
    }
}
