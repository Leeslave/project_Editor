using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldObjectFactory : Singleton<WorldObjectFactory>
{
    /**
     * 월드 오브젝트 생성
     * - NPCData를 가지고 해당하는 객체 생성
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

    private new void Awake()
    {
        base.Awake();
        
        for (int i = 0; i < Enum.GetValues(typeof(World)).Length; i++)
        {
            _objectList.Add(new List<WorldObject>());
        }
    }


    /// <summary>
    ///  NPCData를 가지고 NPC 생성
    ///  </summary>
    ///  <param name="objData">월드오브젝트 데이터</param>
    ///  <param name="location">Location 정보</param>
    /// <param name="position">Location 오브젝트의 transform</param>
    public void CreateNPC(ChatObjectData objData, World location, Transform position)
    {
        // 해당하는 프리팹 로드
        GameObject prefab;
        if (Enum.TryParse(objData.objectType, out WorldObjectType npcType))
        {
            prefab = prefabs[(int)npcType];
        }
        else
        {
            throw new Exception($"Invalid WorldObject Name : {objData.objectType}");
        }
        
        // 월드 오브젝트 생성
        GameObject newObject = Instantiate(prefab, position);
        
        // 데이터 입력
        if (objData.name == null)
        {
            objData.name = objData.objectType;
        }
        newObject.name = objData.name;
        ChatObject obj = newObject.GetComponent<ChatObject>();
        obj.positions = objData.positions.Zip(objData.anchor, (wv, anchor) =>  (wv, anchor)).ToList();
        obj.chatAssets = objData.chat.Zip(objData.onAwake, (c, a) => (c, a)).ToList();
        obj.Triggers = new();
        
        // 객체 리스트에 추가
        if (_objectList.Count == 0)
        {
            for (int i = 0; i < Enum.GetValues(typeof(World)).Length; i++)
            {
                _objectList.Add(new List<WorldObject>());
            }
        }
        _objectList[(int)location].Add(obj);
        
        // 오브젝트 시작
        obj.OnAwake();
    }
    
    
    /// <summary>
    ///  ActionData를 가지고 ActionObject 생성
    ///  </summary>
    public void CreateAction(ActionObjectData objData, World location, Transform position)
    {
        GameObject newObject = new();
        Debug.Log($"New Object: {newObject.name}");
        
        // 데이터 입력
        newObject.name = objData.actionName;
        ActionObject obj = newObject.AddComponent<ActionObject>();
        obj.positions = objData.positions.Zip(objData.anchor, (wv, anchor) =>  (wv, anchor)).ToList();
        obj.actionName = objData.actionName;
        obj.actionParam = objData.actionParam;
        
        // 객체 리스트에 추가
        if (_objectList.Count == 0)
        {
            for (int i = 0; i < Enum.GetValues(typeof(World)).Length; i++)
            {
                _objectList.Add(new List<WorldObject>());
            }
        }
        _objectList[(int)location].Add(obj);
        
        // 오브젝트 시작
        obj.OnAwake();
    }


    /// <summary>
    /// 지역 내 특정 오브젝트 반환
    /// </summary>
    /// <param name="objName">오브젝트명</param>
    /// <returns></returns>
    public WorldObject FindObject(string objName)
    {
        foreach (var objList in _objectList)
        {
            var obj = objList.Find(x => x.name == objName);
            if (obj is not null)
            {
                return obj;
            }
        }
        return null;
    }


    /// <summary>
    /// 지역 내 특정 오브젝트 삭제
    /// </summary>
    /// <param name="objName">오브젝트명</param>
    public void RemoveObject(string objName)
    {
        foreach (var objList in _objectList)
        {
            var obj = objList.Find(x => x.name == objName);
            if (obj is not null)
            {
                objList.Remove(obj);
                Destroy(obj.gameObject);
                return;
            }
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
