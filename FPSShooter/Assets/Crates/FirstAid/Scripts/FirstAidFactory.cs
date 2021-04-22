using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidFactory : CrateFactory
{
    public FirstAid cratePrefab;
    public AnimationCurve lifeDistribution;

    public override SupplyCrate CreateCrate()
    {
        var crate = Instantiate(cratePrefab);
        crate.healthSupply = lifeDistribution.Evaluate(Random.Range(0, 1));
        return crate;
    }
}
