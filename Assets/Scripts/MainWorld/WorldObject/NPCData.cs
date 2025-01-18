using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NPCData : WorldObjectData
{
    public List<(string chat, bool onAwake)> chatData;  // 대사 정보
    public Vector2 anchor;  // 위치
    public float size;    // 크기
}
