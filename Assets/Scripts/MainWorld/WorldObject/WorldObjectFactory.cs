using GameService;
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
    
    private IDayService _dayService;

    
    #region routine
    private new void Awake()
    {
        base.Awake();
        
        for (int i = 0; i < Enum.GetValues(typeof(World)).Length; i++)
        {
            _objectList.Add(new List<WorldObject>());
        }
    }

    private void Start()
    {
        ServiceProvider.Get<IDayService>(service =>
        {
            _dayService = service;
            _dayService.OnTimeChanged += Init;
            
            if (_dayService.Data != null)
            {
                Init();
            }
        });
    }

    private void OnDestroy()
    {
        _dayService.OnTimeChanged -= Init;
    }

    private void Init(int data)
    {
        Init();
    }

    public void Init()
    {
        var data = _dayService.TimeData;
        Clear();
        
        // Chat Object 생성
        foreach (var item in data.npc)
        {
            CreateNPC(item);
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
                Destroy(obj.gameObject);
            }
            list.Clear();
        }
    }

    #endregion

    #region Create
    /// <summary>
    ///  NPCData를 가지고 NPC 생성
    ///  </summary>
    ///  <param name="objData">월드오브젝트 데이터</param>
    ///  <param name="location">Location 정보</param>
    /// <param name="position">Location 오브젝트의 transform</param>
    public void CreateNPC(ChatObjectData objData, int pos = 0)
    {
        WorldVector targetPos = objData.positions[pos];
        
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
        GameObject newObject = Instantiate(prefab, transform);
        
        // 객체 데이터 설정
        if (string.IsNullOrEmpty(objData.name))
        {
            objData.name = objData.objectType;
        }
        newObject.name = objData.name;
        
        // Chat 데이터 입력
        ChatObject obj = newObject.GetComponent<ChatObject>();
        obj.positions = objData.positions.Zip(objData.anchor, (wv, anchor) =>  (wv, anchor)).ToList();
        obj.chatAssets = objData.chat.Zip(objData.onAwake, (c, a) => (c, a)).ToList();
        
        // 객체 리스트에 추가
        if (_objectList.Count == 0)
        {
            for (int i = 0; i < Enum.GetValues(typeof(World)).Length; i++)
            {
                _objectList.Add(new List<WorldObject>());
            }
        }
        _objectList[(int)targetPos.location].Add(obj);
        
        // 오브젝트 시작
        obj.Init();
    }
    
    
    /// <summary>
    ///  ActionData를 가지고 ActionObject 생성
    ///  </summary>
    public void CreateAction(ActionObjectData objData, int pos = 0)
    {
        //위치 지정
        WorldVector targetPos = objData.positions[pos];
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
        _objectList[(int)targetPos.location].Add(obj);
        
        // 오브젝트 시작
        obj.Init();
    }
    
    #endregion

    #region Manage
    
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
    #endregion
}
