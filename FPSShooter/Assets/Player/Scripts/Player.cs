using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviour
{
    private Weapon[] weapons = new Weapon[3];
    private int chosedWeapon = 0;

    // Start is called before the first frame update
    void Awake()
    {
        weapons[0] = GameObject.FindGameObjectWithTag("Pistol").GetComponent<Pistol>();
        if(weapons[0] == null)
        {
            Debug.LogError("No pistol found");
        }
    }

    public void Shoot()
    {
        if(weapons[chosedWeapon] != null)
        {
            weapons[chosedWeapon].Shoot(transform.position, transform.forward);
        }
    }

    public void Reload()
    {
        if (weapons[chosedWeapon] != null)
        {
            weapons[chosedWeapon].Reload();
        }
    }
    
}
