using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCountPower : EnemyCountCalculation
{
    public float power;
    public int offset;

    public override int Calculate(int wave)
    {
        return (int)Mathf.Pow(wave, power) + offset;
    }
}
