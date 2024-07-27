using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TutorialSetting.TutoList;

public class TutorialSetting : MonoBehaviour
{
    public bool Test;

    public static TutorialSetting instance;
    public GameObject ForTutoCond;
    [SerializeField] GameObject CircleTuto;
    [SerializeField] GameObject BoxTuto;
    TutorialBorder CircleBorder;
    TutorialBorder BoxBorder;
    [SerializeField] RectTransform TextBox;

    public TutorialBorder CurActive = null;

    static Vector2 TargetPos = Vector2.zero;
    static float MaxLength = 0;


    [System.Serializable]
    public class TutoList
    {
        [Header("---- 조명할 Object ----")]
        public GameObject TargetObj;
        // 조명할 Object가 Instantiate등의 이유로 사전에 넣을 수 없을 경우
        // 부모 Object를 대신 넣어 지정함
        public List<int> ChildInd = new List<int>();   
        
        [Header("---- 설명 문구 ----")]
        [Multiline(3)]
        public string Text;

        [Header("---- 마우스 이벤트 ----")]
        public List<EventData> Events;

        [Header("---- Object 관련 ----")]
        public List<GameObject> DisableObjects;
        public List<GameObject> AbleObjects;

        // 클리어를 해당 Object의 활성화로 판단
        public GameObject ActiveTarget;
        public List<int> ActiveChild;

        // 클리어를 해당 Object의 비활성화로 판단
        public GameObject InActiveTarget;
        public List<int> InActiveChild;

        [Header("---- 기타 설정 ----")]
        
        public bool IsRect;
        public float TimeScale = 1;
        public bool GoNextTuto;
        public bool IsHighlight;
        public bool KeepEvent = false;
        public bool IsRemove = true;

        [Serializable]
        public class Event : UnityEvent<BaseEventData> { }

        [Serializable]
        public class EventData
        {
            public EventTriggerType TriggerType;
            public bool ReactAnywhere;
            public int ClickCount = 1;
            public Event TutoEvent;
            public bool IsEndPoint;
            public bool AddTargetAction = false;
        }
    }

    public List<TutoList> TutorialList;


    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
        BoxBorder = BoxTuto.GetComponent<TutorialBorder>(); CircleBorder = CircleTuto.GetComponent<TutorialBorder>();
        if (Test) ActiveTutorial();
    }

    public bool GoNextTutorial()
    {
        var cnt = TutorialList[0]; if(cnt.IsRemove) TutorialList.RemoveAt(0);
        if ((cnt.GoNextTuto || !gameObject.activeSelf) && TutorialList.Count!=0) ActiveTutorial();
        else gameObject.SetActive(false);

        return true;
    }

    public void ActiveTutorial()
    {
        Time.timeScale = TutorialList[0].TimeScale;

        foreach (var k in TutorialList[0].DisableObjects) k.SetActive(false);
        foreach (var k in TutorialList[0].AbleObjects) k.SetActive(true);

        foreach (var k in TutorialList[0].ChildInd)
            TutorialList[0].TargetObj = TutorialList[0].TargetObj.transform.GetChild(k).gameObject;


        if (TutorialList[0].TargetObj != null) { Vector2 Pos = Camera.main.WorldToScreenPoint(TutorialList[0].TargetObj.transform.position); TargetPos = Pos; }
        gameObject.SetActive(true);

        if (TutorialList[0].ActiveTarget)
        {
            foreach (var k in TutorialList[0].ActiveChild) TutorialList[0].ActiveTarget = TutorialList[0].ActiveTarget.transform.GetChild(k).gameObject;
            TutorialList[0].ActiveTarget.AddComponent<Tutorial_ActiveType>();
        }

        if (TutorialList[0].InActiveTarget)
        {
            foreach (var k in TutorialList[0].InActiveChild) TutorialList[0].InActiveTarget = TutorialList[0].InActiveTarget.transform.GetChild(k).gameObject;
            TutorialList[0].InActiveTarget.AddComponent<Tutorial_Inactive>();
        }

        if (TutorialList[0].IsRect)
        {
            MaxLength = BoxBorder.Init(TutorialList[0].TargetObj, TutorialList[0].Text, TutorialList[0].IsHighlight, TutorialList[0].KeepEvent);
            CircleTuto.SetActive(false);
        }
        else
        {
            MaxLength = CircleBorder.Init(TutorialList[0].TargetObj, TutorialList[0].Text, TutorialList[0].IsHighlight, TutorialList[0].KeepEvent);
            BoxTuto.SetActive(false);
        }
    }

    public void ChangeTarget(GameObject Target)
    {
        CurActive.ChangeTarget(Target);
    }

    public void ResetTarget()
    {
        CurActive.ChangeTarget(TutorialList[0].TargetObj);
    }

    
    public void OnPointer()
    {
        CurActive.InActiveRay();
    }

    public void OutPointer()
    {
        CurActive.ActiveRay();
    }

    public class EventActions
    {
        EventTriggerType Type;
        List<EventTrigger.TriggerEvent> Actions = new List<EventTrigger.TriggerEvent>();
        public EventActions(EventTrigger TargetTrigger, EventTrigger trigger, EventData Data, bool AddExtra, Func<bool> act)
        {
            try {
                Type = Data.TriggerType;
                if (TargetTrigger != null && AddExtra) foreach (var k in TargetTrigger.triggers)
                    {
                        if (k.eventID == Type) Actions.Add(k.callback);
                    }

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = Type;

                entry.callback.AddListener((data) =>
                {
                    var PointerData = (PointerEventData)data;
                    if (PointerData.clickCount >= Data.ClickCount)
                    {
                        if (Data.ReactAnywhere || Vector2.Distance(TargetPos, PointerData.pressPosition) < MaxLength)
                        {
                            foreach (var j in Actions) j.Invoke(data);
                            Data.TutoEvent.Invoke(data);
                            if (Data.IsEndPoint) act();
                        }
                    }
                });
                trigger.triggers.Add(entry);
            }
            catch(System.Exception e)
            {
                print($"{e} At Add");
            }
        }
    }
    
    public void SetEvent(EventTrigger trigger)
    {
        if (TutorialList[0].TargetObj == null) return;
        EventTrigger cnt = TutorialList[0].TargetObj.GetComponent<EventTrigger>();
        foreach (var i in TutorialList[0].Events)
        {
             new EventActions(cnt,trigger,i,i.AddTargetAction,GoNextTutorial);
        }
    }
}
