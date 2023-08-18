using System;
using UnityEngine;


[System.Serializable]
public class WorldObject
{
    public string imageName;
    public string chatName = null;
    public ChatTriggerType chatType;

    
}

[Serializable]
[SerializeField]
public enum ChatTriggerType {   
    OnClick,    // 버튼 누를 시
    OnStart,    // 활성화시 자동 1회
    EveryStart, // 매번 활성화마다
}