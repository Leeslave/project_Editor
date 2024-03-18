using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object Data", menuName = "Scriptable Object/World Object Data", order = int.MaxValue)]
public class WorldObjectData : ScriptableObject
{
    public GameObject prefab;   // 오브젝트 프리팹
    public string objName;  // 오브젝트 이름
    [HideInInspector]
    public World location;  // 장소
    [HideInInspector]
    public int position;    // 장소 내 위치
    public int count = 0;   // 상호작용 가능 횟수 (0 : 무제한)
    [HideInInspector]
    public string awakeParam = "";   // Active 함수 매개변수
}
