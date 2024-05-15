using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDatabase : SingletonObject<ObjectDatabase>
{
    public List<GameObject> prefabs;       // 데이터 리스트

    public static List<List<WorldObjectData>> ObjectList = new();        // 해당 날자의 WorldObject들
    public static List<MessageData> MessageList = new();        // 해당 날짜의 메시지들
    public string dataPath;


    /// <summary>
    /// 오브젝트 데이터들을 불러오기
    /// </summary>
    public void Read()
    {
        var dataList = FileReader.ReadCSV($"{dataPath}_{GameSystem.Instance.gameData.date}");
        if (dataList == null)
        {
            return;
        }
        
        ClearList();

        foreach(var obj in dataList)
        {
            switch(obj[(int)DataColumn.type])
            {
                // NPC 데이터 생성
                case "npc":
                    NPCData newNPC = new()
                    {
                        time = int.Parse(obj[(int)DataColumn.time]),
                        name = obj[(int)DataColumn.name],
                        objType = Enum.Parse<ObjectType>(obj[(int)DataColumn.objType]),

                        location = Enum.Parse<World>(obj[(int)DataColumn.location]),
                        position = int.Parse(obj[(int)DataColumn.position]),

                        anchor = new Vector2(float.Parse(obj[(int)DataColumn.posX]), float.Parse(obj[(int)DataColumn.posY])),
                        size = float.Parse(obj[(int)DataColumn.size]),

                        awakeParam = obj[(int)DataColumn.OnAwake],
                        clickParam = obj[(int)DataColumn.OnClick]
                    };

                    ObjectList[(int)newNPC.location].Add(newNPC);
                    break;
                // 이펙트 데이터 생성
                case "effect":
                    EffectData newEffect = new()
                    {
                        time = int.Parse(obj[(int)DataColumn.time]),
                        name = obj[(int)DataColumn.name],
                        objType = Enum.Parse<ObjectType>(obj[(int)DataColumn.objType]),
                        location = Enum.Parse<World>(obj[(int)DataColumn.location])
                    };

                    ObjectList[(int)newEffect.location].Add(newEffect);
                    break;
                // 메시지 데이터 생성
                case "message":
                    MessageData newMessage = new()
                    {
                        time = int.Parse(obj[(int)DataColumn.time]),
                        name = obj[(int)DataColumn.name],
                        objType = Enum.Parse<ObjectType>(obj[(int)DataColumn.objType]),
                        awakeParam = obj[(int)DataColumn.OnAwake]
                    };

                    MessageList.Add(newMessage);
                    break;
            }
        }
    }


    private void ClearList()
    {
        // 리스트 초기화
        ObjectList = new();
        for(int i = 0; i < (int)World.MAX; i++)
        {
            ObjectList.Add(new());
        }
    }

    public void DebugList()
    {
        Debug.Log($"Total location Count : {ObjectList.Count}");
        foreach(var iter in ObjectList)
        {
            Debug.Log($"NPC Count : {iter.Count}");
        }
    }
}

public enum ObjectType {
    /**
    월드 내 생성 오브젝트 목록
    */
    none,
    Klayton,
    Clover,
    Henderson,
    Walter,
    King,
    Rex,
    Kennedy,
    Price,
    Monk,
    Mechanic,
    Reporter,
    Nametag,
}

internal enum DataColumn {
    time,
    type,
    name,
    objType,
    location,
    position,
    OnAwake,
    OnClick,
    posX,
    posY,
    size,
}