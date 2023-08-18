using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttatchFile_F : MonoBehaviour
{
    [SerializeField] protected  AttatchFile_N AN;
    [SerializeField] protected  List<GameObject> AttatchFields;
    [SerializeField] protected  List<TMP_Text> AttatchFieldsName;
    [SerializeField] protected  List<Image> AttatchFieldsImage;
    [SerializeField] protected  GameObject Border;
    [SerializeField] protected  GameObject Error;
    [SerializeField] protected  TMP_Text ErrorMessage;
    [SerializeField] protected  TMP_Text ErrorReason;
    protected RectTransform MyRect;
    protected int AttatchNum = 0;
    protected List<int> AttatchInd = new List<int>(5);
    
    private void Awake()
    {
        for (int i = 0; i < 5; i++) AttatchInd.Add(i);
        MyRect = GetComponent<RectTransform>();
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerEnter, OnPoint);
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerExit, OutPoint);
    }

    protected virtual void OutPoint(PointerEventData Data)
    {
        if (AN.IsDragged) AN.IsAttatched = false;
    }

    protected virtual void OnPoint(PointerEventData Data)
    {
        if (AN.IsDragged) AN.IsAttatched = true;
    }
    public virtual void Attatching(Sprite image, string name, GameObject Goal)
    {
        if(AttatchNum == 4)
        {
            AttatchFail("Ã·ºÎ ½ÇÆÐ","Ã·ºÎ »óÇÑ ÃÊ°ú");
            return;
        }
        if (AttatchNum == 0) AttatchFields[0].SetActive(false);
        ++AttatchNum;
        int s = AttatchInd[AttatchNum];
        AttatchFields[s].SetActive(true);
        AttatchFieldsName[s].text = name;
        AttatchFieldsImage[s].sprite = image;
        AN.IsAttatched = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(MyRect);
    }

    public virtual void AttatchCancle(int num)
    {
        if(AttatchNum == 1) AttatchFields[0].SetActive(true);
        AttatchFields[num].SetActive(false);
        if (num < AttatchNum)
        {
            AttatchFields[num].transform.SetSiblingIndex(AttatchNum);
            AttatchInd.Remove(num);
            AttatchInd.Insert(AttatchNum,num);
        }
        AttatchNum--;
        LayoutRebuilder.ForceRebuildLayoutImmediate(MyRect);
    }
    
    public virtual void AttatchFail(string reason,string message)
    {
        Border.SetActive(true);
        Error.SetActive(true);
        AN.IsAttatched = false;
        ErrorReason.text = reason;
        ErrorMessage.text = message;
    }
}
