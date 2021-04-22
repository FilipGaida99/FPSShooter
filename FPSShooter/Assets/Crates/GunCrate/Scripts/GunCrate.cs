using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCrate : SupplyCrate
{
    public GameObject weaponPrefab;

    public override void Awake()
    {
        base.Awake();
        GetComponentInChildren<SpriteRenderer>().sprite = weaponPrefab.GetComponent<Weapon>().Icon;
    }

    public override void Resupply(Player player)
    {
        if (player.AddWeapon(weaponPrefab))
        {
            MarkAsUsed();
        }
    }

}
