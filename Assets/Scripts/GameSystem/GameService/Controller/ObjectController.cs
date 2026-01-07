
using GameService;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour, IObjectDataService
{
    /**
     * 데이터 기반 월드 오브젝트 컨트롤러
     * - DataService로부터 객체 로드 및 생성
     * - 
     */
    [SerializeField] private List<ChatObject> npcs = new();
    [SerializeField] private List<ActionObject> actions = new();
    
    public void Init()
    {
        throw new System.NotImplementedException();
    }

    public List<ChatObjectData> GetNPC()
    {
        throw new System.NotImplementedException();
    }

    public List<ActionObjectData> GetActionObject()
    {
        throw new System.NotImplementedException();
    }

    public List<BGMData> GetBGM()
    {
        throw new System.NotImplementedException();
    }
}
