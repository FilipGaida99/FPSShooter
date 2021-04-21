using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCrateFactory : CrateFactory
{
    public GunCrate cratePrefab;
    public List<GameObject> possibleWeapons;

    public override SupplyCrate CreateCrate()
    {
        cratePrefab.weaponPrefab = GetRandomWeapon();
        return Instantiate(cratePrefab);
    }

    private GameObject GetRandomWeapon()
    {
        return possibleWeapons[Random.Range(0, possibleWeapons.Count)];
    }
}
