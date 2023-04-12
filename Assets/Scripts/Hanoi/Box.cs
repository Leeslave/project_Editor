using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 박스의 정보를 저장(아직 무게만 사용)
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
