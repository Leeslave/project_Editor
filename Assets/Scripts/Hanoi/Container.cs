using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Container:MonoBehaviour
{
    // ���̶���Ʈ ���� ��, ���� �ʾ��� ���� Sprite
    public Sprite NonClicked;
    public Sprite Clicked;
    // HanoiManager�� �����Լ��� ������ Object
    public HanoiManager HM;
    // ���� Container�� ���� �ڽ����� ����Ʈ
    public List<GameObject> Boxes = new List<GameObject>();
    [HideInInspector]
    public int BoxSize = 0;
    // ���� Container�� ���� Index
    public int Ind;
    // Image Component�� ��µ� ���� ����
    Image sr;

    private void Awake()
    {
        sr = GetComponent<Image>();
        EventTrigger ET = GetComponent<EventTrigger>();
        AddEvent(ET,EventTriggerType.PointerEnter,OnPoint);
        AddEvent(ET,EventTriggerType.PointerExit, OutPoint);
        AddEvent(ET,EventTriggerType.PointerClick, Click);
    }

    // �����Ͱ� �������� �� ȣ��Ǵ� �Լ�
    // Input : ȣ��� ���¿��� Pointer�� ���¸� ���� Class(�ش� �Լ������� �̻��)
    void OnPoint(PointerEventData data)
    {
        // ���� ���ڸ� ���� ���°�, �ش� ���ڸ� ���� Container�� �ű� �� �ִٸ�, ���̶���Ʈ�Ѵ�.
        if (HM.IsPick) { if (CompareTop(HM.PickedBox)) sr.sprite = Clicked; }
        // ���� ���ڸ� ���� ���� ���¸� ���̶���Ʈ�Ѵ�.
        else
        {
            if (Boxes.Count != 0) 
            {
                if (!Boxes[Boxes.Count - 1].GetComponent<Box>().CanPick()) sr.sprite = NonClicked;
                else sr.sprite = Clicked;
            }
            else sr.sprite = NonClicked;
        }
    }
    // �����Ͱ� ����� �� ȣ��Ǵ� �Լ�
    // �����Ͱ� ����� ���̶���Ʈ�� �����Ѵ�.
    // Input : ȣ��� ���¿��� Pointer�� ���¸� ���� Class(�ش� �Լ������� �̻��)
    public void OutPoint(PointerEventData data)
    {
        sr.sprite = NonClicked;
    }
    // �����ͷ� �ش� Object�� Ŭ������ �� ȣ��Ǵ� �Լ�.
    // Container���� Box�� �������� ����Ѵ�.
    // Input : ȣ��� ���¿��� Pointer�� ���¸� ���� Class(�ش� �Լ������� �̻��)
    void Click(PointerEventData data)
    {
        // ���� ���ڸ� ���� ������ ���, Container�� �ش� ���ڰ��� ���¸� ���ϰ� �߰��� ������ ���Ѵ�.
        // HanoiManager.cs ����
        if (HM.IsPick)
        {
            HM.AddEvent(Ind);
            HM._Init();
        }
        else
        {
            // ���� ���ڸ� ���� ���� ���
            // ����ִ� Container�� ������ ���� �ƴ϶�� Container�� �ֻ�� ���ڸ� ��������, ���ڸ� ���� ���·� �ٲ۴�.
            HM.PickedBox = ReturnTop(false);
            if (HM.PickedBox != null)
            {
                HM.PickedBox.transform.SetAsLastSibling();
                HM.IsPick = true;
                HM.CurCon = Ind;
            }
        }
        OnPoint(null);
    }

    // �Ű� ������ ���� ���ڿ�, ���� Container���� ���¸� ���Ѵ�.
    // ����ְų�, �Ű������� ���� ���ڰ� ���� Container �ֻ���� ���ں��� �����ٸ� true�� ��ȯ�ϸ� �׷��� ������ false�� ��ȯ�Ѵ�.
    // Input : ���� Object
    // Output : Input���� ���� ���ڰ� �ش� Container�� �߰��� �� �ִ���
    public bool CompareTop(GameObject A)
    {
        if (A == null) return true;
        if (Boxes.Count == 0) return true;
        if (A.GetComponent<Box>().CanAdd(Boxes[Boxes.Count - 1].GetComponent<Box>())) return true;
        else return false;
    }

    // ���� Container�� �ֻ���� ���ڸ� ��ȯ�Ѵ�.
    // ����ִٸ� null�� ��ȯ�Ѵ�.
    // Output : ���� Container �ֻ���� ����.(����ִٸ� null)
    public GameObject ReturnTop(bool IsBack)
    {
        if(Boxes.Count == 0) return null;
        else
        {
            GameObject cnt = Boxes[Boxes.Count - 1];
            if (IsBack) cnt.GetComponent<Box>().DuraChange(1);
            if (!cnt.GetComponent<Box>().CanPick()) return null;
            Boxes.RemoveAt(Boxes.Count - 1);
            BoxSize--;
            return cnt;
        }
    }

    // ���� Container�� �Ű������� ���� ���� Object�� �״´�.
    // Input : �߰��� Box Object
    public void AddTop(GameObject cnt)
    {
        Vector3 tmp;
        // ���� Container�� ��������� Ư�� ��ġ�� Box�� �̵���Ų��.
        if (Boxes.Count == 0)
        {
            tmp = gameObject.transform.position;
            tmp.y = -190;
        }
        // Container�� ������� ������, �ֻ�ܿ� �ִ� ���� �ٷ� ���� ��ġ��Ų��.
        else
        {
            tmp = Boxes[Boxes.Count - 1].transform.position;
            tmp.y += Boxes[Boxes.Count - 1].transform.lossyScale.y;
        }
        // Container�� Box List�� �߰��Ѵ�.
        Boxes.Add(cnt);
        BoxSize++;
        cnt.transform.position = tmp;
    }

    // Ư�� EventTriggerType�� Event �Լ��� ���ӽ�Ų��.
    // Input : ���� ��ų Object�� EventTrigger, EventTrigger�� �߻���ų EventType, ���� ��ų �Լ�

    void AddEvent(EventTrigger eventTrigger, EventTriggerType Type, Action<PointerEventData> Event)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = Type;
        entry.callback.AddListener((data) => { Event((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }
}
