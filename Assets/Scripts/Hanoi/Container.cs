using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Container:MonoBehaviour
{
    public Sprite NonClicked;
    public Sprite Clicked;
    public HanoiManager HM;
    public List<GameObject> Boxes = new List<GameObject>();
    public int Ind;
    Image sr;

    private void Awake()
    {
        sr = GetComponent<Image>();
        EventTrigger ET = GetComponent<EventTrigger>();
        AddEvent(ET,EventTriggerType.PointerEnter,OnPoint);
        AddEvent(ET,EventTriggerType.PointerExit, OutPoint);
        AddEvent(ET,EventTriggerType.PointerClick, Click);
    }
    void OnPoint(PointerEventData data)
    {
        if (HM.IsPick) { if (CompareTop(HM.PickedBox)) sr.sprite = Clicked; }
        else sr.sprite = Clicked;
    }
    public void OutPoint(PointerEventData data)
    {
        sr.sprite = NonClicked;
    }
    void Click(PointerEventData data)
    {
        if (HM.IsPick)
        {
            HM.AddEvent(Ind);
            HM._Init();
        }
        else
        {
            HM.PickedBox = ReturnTop();
            if (HM.PickedBox != null)
            {
                HM.PickedBox.GetComponent<SpriteRenderer>().sortingOrder = 2;
                HM.IsPick = true;
                HM.CurCon = Ind;
            }
        }
    }

    public bool CompareTop(GameObject A)
    {
        if (A == null) return true;
        if (Boxes.Count == 0) return true;
        if (A.GetComponent<Box>().Weight <= Boxes[Boxes.Count - 1].GetComponent<Box>().Weight) return true;
        else return false;
    }
    public GameObject ReturnTop()
    {
        if(Boxes.Count == 0) return null;
        else
        {
            GameObject cnt = Boxes[Boxes.Count - 1];
            Boxes.RemoveAt(Boxes.Count - 1);
            return cnt;
        }
    }
    
    public void AddTop(GameObject cnt)
    {
        Vector3 tmp;
        cnt.GetComponent<SpriteRenderer>().sortingOrder = 1;
        if (Boxes.Count == 0)
        {
            tmp = gameObject.transform.position;
            tmp.y = -3;
        }
        else
        {
            tmp = Boxes[Boxes.Count - 1].transform.position;
            tmp.y += Boxes[Boxes.Count - 1].transform.lossyScale.y;
        }
        Boxes.Add(cnt);
        cnt.transform.position = tmp;
    }

    void AddEvent(EventTrigger eventTrigger, EventTriggerType Type, Action<PointerEventData> Event)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = Type;
        entry.callback.AddListener((data) => { Event((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }
}
