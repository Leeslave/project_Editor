using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldObjectFactory : Singleton<WorldObjectFactory>
{
    /**
     * 월드 오브젝트 생성
     * - NPCData, BGMData를 가지고 해당하는 객체 생성
     * - 지역락 데이터로 해당 지역으로 이동하는 객체 삭제
     */

    private enum WorldObjectType
    {
        none,
        Rex,
        Clover,
        Henderson,
        Kennedy,
        King,
        Klayton,
        Price,
        Walter,
        Mechanic,
        Monk,
        Reporter,
        Nametag,
        
    }

    public List<GameObject> prefabs;
    private readonly List<List<WorldObject>> _objectList = new();


    /// <summary>
    ///  NPCData를 가지고 NPC 생성
    ///  </summary>
    ///  <param name="objData">월드오브젝트 데이터</param>
    ///  <param name="location">Location 정보</param>
    /// <param name="position">Location 오브젝트의 transform</param>
    public void CreateObject(WorldObjectData objData, World location, Transform position)
    {
        // 해당하는 프리팹 로드
        GameObject prefab;
        if (Enum.TryParse(objData.name, out WorldObjectType npcType))
        {
            prefab = prefabs[(int)npcType];
        }
        else
        {
            throw new Exception($"Invalid WorldObject Name : {objData.name}");
        }
        
        // 월드 오브젝트 생성
        GameObject newObject = Instantiate(prefab, position);
        
        // 데이터 입력
        WorldObject worldObject = newObject.GetComponent<WorldObject>();
        worldObject.name = objData.name;
        worldObject.positions = objData.positions.Zip(objData.anchor, (wv, anchor) =>  (wv, anchor)).ToList();
        worldObject.chatAssets = objData.chat;
        Debug.Log(objData.chat);
        
        // 객체 리스트에 추가
        if (_objectList.Count == 0)
        {
            for (int i = 0; i < Enum.GetValues(typeof(World)).Length; i++)
            {
                _objectList.Add(new List<WorldObject>());
            }
        }
        _objectList[(int)location].Add(worldObject);
        
        // 오브젝트 시작
        worldObject.OnAwake();
    }


    /// <summary>
    /// 지역 내 특정 오브젝트 삭제
    /// </summary>
    /// <param name="objName">오브젝트명</param>
    /// <param name="location">해당하는 위치</param>
    public void RemoveObject(string objName, World location)
    {
        var obj = _objectList[(int)location].Find(x => x.name == objName);
        if (obj != null)
        {
            _objectList[(int)location].Remove(obj);
            Destroy(obj);
        }
    }


    /// <summary>
    /// 모든 오브젝트 삭제
    /// </summary>
    public void Clear()
    {
        foreach (var list in _objectList)
        {
            foreach (var obj in list)
            {
                Destroy(obj);
            }
            list.Clear();
        }
    }
}
