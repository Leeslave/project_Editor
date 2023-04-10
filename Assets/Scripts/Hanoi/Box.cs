using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public int Weight;
    public int MaxDurability;
    public int CurDurability;

    public void InitBox(int _Weight, int _Durability)
    {
        Weight = _Weight;
        MaxDurability = _Durability;
        CurDurability = _Durability;
    }
}
