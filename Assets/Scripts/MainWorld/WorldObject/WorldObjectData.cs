using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class WorldVector
{
    public string location;
    public int position;
    public string name = "???";


    public WorldVector(World location, int position)
    {
        this.location = location.ToString();
        this.position = position;
    }
    
    
    public World GetLocation()
    {
        return Enum.Parse<World>(location);
    }
}

[Serializable]
public class NPCData
{
    public string name;
    public List<WorldVector> positions;
    public List<(string chat, bool onAwake)> chatData;  // 대사 정보
    public Vector2 anchor;  // 위치
    public float size;    // 크기
}

[Serializable]
public class BGMData
{
    public string location;
    public int code;
}