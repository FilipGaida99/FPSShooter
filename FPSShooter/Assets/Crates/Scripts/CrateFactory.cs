using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CrateFactory : MonoBehaviour
{
    public abstract SupplyCrate CreateCrate();
}
