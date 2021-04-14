using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Weapon
{
    int Magazine { get; set; }
    int BulletsLeft { get; set; }

    float Damage { get; set; }

    //Return true, when shoot was performed.
    bool Shoot(Vector3 from, Vector3 direction);

    void Reload();

    bool ResupplyBullets();

    void OnChoose();

    void OnHide();
}
