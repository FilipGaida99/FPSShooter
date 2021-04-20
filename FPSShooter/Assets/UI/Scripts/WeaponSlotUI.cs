using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI textUI;

    public void Awake()
    {
        image = GetComponentInChildren<Image>();
        textUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetWeaponSlot(Sprite sprite, string description)
    {
        image.sprite = sprite;
        textUI.text = description;
    }
}
