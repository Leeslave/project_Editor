using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectData
{
    public int time;    // 시간대
    public string name;    // 오브젝트 이름
    public ObjectType objType = ObjectType.none;  // 오브젝트 타입
    [HideInInspector]
    public World location;  // 장소
    [HideInInspector]
    public int position;    // 장소 내 위치
    public int count = 0;   // 상호작용 가능 횟수 (0 : 무제한)
    [HideInInspector]
    public string awakeParam = "";   // Active 함수 매개변수
}
