using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimingImage : MonoBehaviour
{
    protected Image image;

    public virtual void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetImageScale(float scale)
    {
        image.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetImage(Sprite newImage)
    {
        image.sprite = newImage;
    }

}
