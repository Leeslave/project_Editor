using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ChatObjectData
{
    public string name = null;
    public string objectType;
    public List<WorldVector> positions;
    public List<Anchor> anchor;  // 위치
    public List<string> chat;  // 대사 정보
    public List<bool> onAwake;  // 대사 시작 여부
}

[Serializable]
public class ActionObjectData
{
    public List<WorldVector> positions;
    public List<Anchor> anchor;  // 위치
    public string actionName;  // 액션 정보
    public string actionParam;  // 대사 시작 여부
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
    
    // == 연산자 오버로딩
    public static bool operator ==(WorldVector v1, WorldVector v2)
    {
        // null 체크 및 비교
        if (ReferenceEquals(v1, null) && ReferenceEquals(v2, null)) return true;
        if (ReferenceEquals(v1, null) || ReferenceEquals(v2, null)) return false;

        return v1.location == v2.location && v1.position == v2.position;
    }

    // != 연산자 오버로딩
    public static bool operator !=(WorldVector v1, WorldVector v2)
    {
        return !(v1 == v2);
    }

    // Equals 메서드 오버라이딩
    public override bool Equals(object obj)
    {
        if (obj is WorldVector other)
        {
            return this == other; // == 연산자를 이용하여 비교
        }
        return false;
    }

    // GetHashCode 메서드 오버라이딩
    public override int GetHashCode()
    {
        // 위치와 포지션을 기준으로 해시 코드 생성
        return HashCode.Combine(location, position);
    }
}

[Serializable]
public class Anchor
{
    public float x;
    public float y;
    public float size = 1.0f;

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