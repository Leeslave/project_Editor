using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldObjectData
{
    public string name;
    public List<WorldVector> positions;
    public List<(string chat, bool onAwake)> chatData;  // 대사 정보
    public List<Anchor> anchor;  // 위치
}

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
public class Anchor
{
    public float x;
    public float y;
    public float size;

    public Vector2 GetVector()
    {
        return new Vector2(x, y);
    }
}

[Serializable]
public class BGMData
{
    public string location;
    public int code;
}