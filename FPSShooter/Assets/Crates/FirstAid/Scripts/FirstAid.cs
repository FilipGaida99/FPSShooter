using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAid : SupplyCrate
{
    public float healthSupply = 20;
    public override void Resupply(Player player)
    {
        if (player.Heal(healthSupply))
        {
            MarkAsUsed();
        }

    }
}
