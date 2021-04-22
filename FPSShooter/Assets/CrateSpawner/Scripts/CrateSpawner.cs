using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    private CrateFactory[] crateFactories;

    private int lastIndex = -1;

    public void Awake()
    {
        RefreshFactories();
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        foreach(Transform child in transform)
        {
            Gizmos.DrawSphere(child.position, 10);
        }
    }

    public void SpawnCrates()
    {
        foreach(var crateFactory in crateFactories)
        {
            var crateTransform = crateFactory.CreateCrate().transform;
            crateTransform.position = GetRandomPositionFromChild();
        }
    }

    public void RefreshFactories()
    {
        crateFactories = GetComponents<CrateFactory>();
    }

    private Vector3 GetRandomPositionFromChild()
    {
        var childIndex = -1;
        do 
        {
            childIndex = Random.Range(0, transform.childCount);
        } while (childIndex == lastIndex);

        return transform.GetChild(childIndex).position;
    }
}
