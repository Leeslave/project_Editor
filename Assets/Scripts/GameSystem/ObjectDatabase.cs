using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDatabase : MonoBehaviour
{
    private enum ObjectType {
        /**
        월드 내 생성 오브젝트 목록
        */
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
        none,
    }

    public List<WorldObjectData> objectDatas; 

    public static List<List<WorldObjectData>> List;

    private static ObjectDatabase _instance;
    public static ObjectDatabase Instance { get { return _instance; } }

    void Awake()
    {
        if (!_instance)
            _instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 오브젝트 데이터들을 불러오기
    /// </summary>
    public void Read()
    {
        var dataList = FileReader.ReadCSV($"GameData/World/ObjectData_{GameSystem.Instance.gameData.date}");
        // 데이터 목록 초기화
        if (List is null)
        {
            List = new();
            for(int i = 0; i < Enum.GetValues(typeof(World)).Length; i++)
            {
                List.Add(new());
            }
        }
        else
        {
            foreach(var iter in List)
            {
                iter.Clear();
            }
        }

        // 각 데이터를 종류별로 생성
        foreach(var obj in dataList)
        {
            switch(obj[(int)DataColumn.type])
            {
                // NPC 데이터 생성
                case "npc":
                    NPCData newNPC = objectDatas[(int)Enum.Parse<ObjectType>(obj[(int)DataColumn.name])] as NPCData;
                    newNPC.location = Enum.Parse<World>(obj[(int)DataColumn.location]);
                    newNPC.anchor = new Vector2(float.Parse(obj[(int)DataColumn.posX]), float.Parse(obj[(int)DataColumn.posY]));
                    newNPC.position = int.Parse(obj[(int)DataColumn.position]);

                    newNPC.awakeParam = obj[(int)DataColumn.OnAwake];
                    newNPC.clickParam = obj[(int)DataColumn.OnClick];
                    List[(int)newNPC.location].Add(newNPC);
                    break;
                // 이펙트 데이터 생성
                case "effect":
                    EffectData newEffect = new();
                    newEffect.location = Enum.Parse<World>(obj[(int)DataColumn.location]);
                    List[(int)newEffect.location].Add(newEffect);
                    break;
            }
        }
    }
}

internal enum DataColumn {
    type,
    name,
    location,
    position,
    OnAwake,
    OnClick,
    posX,
    posY,
    size,
}