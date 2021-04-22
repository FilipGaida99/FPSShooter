using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCountText : CountText
{
    public RectTransform normalTransform;
    public RectTransform superTransform;
    public float transitionTime;
    public bool animated = false;

    private Coroutine activeAnimation;
    private RectTransform rectTransform;

    public override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
    }

    public override void SetTextValue(int value)
    {
        base.SetTextValue(value);

        if(activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
        }
        if (animated)
        {
            activeAnimation = StartCoroutine(NewValueAnimation());
        }
    }

    private IEnumerator NewValueAnimation()
    {
        float elapsedTime = 0;

        while (elapsedTime < transitionTime)
        {
            rectTransform.localPosition = Vector3.Lerp(superTransform.localPosition,
                normalTransform.localPosition,
                elapsedTime / transitionTime);
            rectTransform.localScale = Vector3.Lerp(superTransform.localScale,
                normalTransform.localScale,
                elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = normalTransform.localPosition;
        rectTransform.localScale = normalTransform.localScale;
    }
}
