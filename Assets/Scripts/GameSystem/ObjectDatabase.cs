using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectDatabase
{
    public static List<List<WorldObject>> List;


    /// <summary>
    /// 오브젝트 데이터들을 불러오기
    /// </summary>
    public static void Read()
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
                case "npc":
                    NPC newNPC = new(Enum.Parse<World>(obj[(int)DataColumn.location]), int.Parse(obj[(int)DataColumn.position]), obj[(int)DataColumn.OnAwake], obj[(int)DataColumn.OnClick]);
                    newNPC.image = obj[(int)DataColumn.image];
                    newNPC.pos = new Vector2(float.Parse(obj[(int)DataColumn.posX]), float.Parse(obj[(int)DataColumn.posY]));
                    newNPC.size = new Vector2(float.Parse(obj[(int)DataColumn.sizeX]), float.Parse(obj[(int)DataColumn.sizeY]));

                    List[(int)newNPC.location].Add(newNPC);
                    break;
                case "effect":
                    WorldEffect newEffect = new(Enum.Parse<World>(obj[(int)DataColumn.location]), int.Parse(obj[(int)DataColumn.position]));

                    List[(int)newEffect.location].Add(newEffect);
                    break;
            }
        }
    }
}

internal enum DataColumn {
    type,
    location,
    position,
    OnAwake,
    OnClick,
    image,
    posX,
    posY,
    sizeX,
    sizeY,
}