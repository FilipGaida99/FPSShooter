using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChoose : MonoBehaviour
{
    public float imageSize = 80;
    public int maxSlots = 4;
    public GameObject weaponSlotPrefab;

    public RectTransform mark;
    public float markTransitionTime = 1;

    private List<WeaponSlotUI> weaponSlots;
    private Coroutine activeMove;

    public void Awake()
    {
        weaponSlots = new List<WeaponSlotUI>();
    }

    public void SetWeaponSlot(int index, Sprite sprite, string description)
    {
        if(index >= maxSlots || index < 0)
        {
            return;
        }
        if(index >= weaponSlots.Count)
        {
            int indexDifference = weaponSlots.Count - index + 1;
            for (int i = 0; i < indexDifference; i++)
            {
                var newSlot = Instantiate(weaponSlotPrefab, transform);
                newSlot.transform.SetAsFirstSibling();
                weaponSlots.Add(newSlot.GetComponent<WeaponSlotUI>());
            }
            RelocateUI();
        }
        weaponSlots[index].SetWeaponSlot(sprite, description);
    }

    public void ChooseWeapon(int index, bool withAnimation = true)
    {
        index = Mathf.Clamp(index, 0, weaponSlots.Count - 1);

        if(activeMove != null)
        {
            StopCoroutine(activeMove);
        }
        var endPosition = weaponSlots[index].GetComponent<RectTransform>().localPosition;
        if (withAnimation)
        {
            activeMove = StartCoroutine(MoveMark(mark.localPosition, endPosition));
        }
        else
        {
            mark.localPosition = endPosition;
        }
    }

    private void RelocateUI()
    {
        int slotCount = weaponSlots.Count;
        float newPositionX = -slotCount * imageSize / 2 + 40;
        for(int i = 0; i < slotCount; i++)
        {
            var rectTransform = weaponSlots[i].GetComponent<RectTransform>();
            var localPosition = rectTransform.localPosition;
            rectTransform.localPosition = new Vector3(newPositionX, localPosition.y, localPosition.z);
            newPositionX += imageSize;
        }
    }

    private IEnumerator MoveMark(Vector3 startPosition, Vector3 endPosition)
    {
        float elapsedTime = 0;
        while (elapsedTime < markTransitionTime)
        {
            mark.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / markTransitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mark.localPosition = endPosition;
    }
}
