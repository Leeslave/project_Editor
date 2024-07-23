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
    public static TutorialSetting instance;
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

        [Header("---- 기타 설정 ----")]
        
        public bool IsRect;
        public bool IsStopTime;
        public bool GoNextTuto;
        public bool IsHighlight;
        public bool KeepEvent = false;

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
    }

    public bool GoNextTutorial()
    {
        var cnt = TutorialList[0]; TutorialList.RemoveAt(0);
        if (cnt.GoNextTuto && TutorialList.Count!=0) ActiveTutorial();
        else gameObject.SetActive(false);

        return true;
    }

    public void ActiveTutorial()
    {
        if (TutorialList[0].IsStopTime) Time.timeScale = 0;

        foreach (var k in TutorialList[0].DisableObjects) k.SetActive(false);
        foreach (var k in TutorialList[0].AbleObjects) k.SetActive(true);

        Vector2 Pos = Camera.main.WorldToScreenPoint(TutorialList[0].TargetObj.transform.position); TargetPos = Pos;
        gameObject.SetActive(true);
        if (TutorialList[0].ActiveTarget) TutorialList[0].ActiveTarget.AddComponent<Tutorial_ActiveType>();
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
                print("Error At Add!");
            }
        }
    }
    
    public void SetEvent(EventTrigger trigger)
    {
        EventTrigger cnt = TutorialList[0].TargetObj.GetComponent<EventTrigger>();
        foreach (var i in TutorialList[0].Events)
        {
             new EventActions(cnt,trigger,i,i.AddTargetAction,GoNextTutorial);
        }
    }
}
