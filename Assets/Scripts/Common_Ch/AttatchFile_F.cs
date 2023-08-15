using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttatchFile_F : MonoBehaviour
{
    [SerializeField] AttatchFile_N AN;
    [SerializeField] List<GameObject> AttatchFields;
    [SerializeField] List<TMP_Text> AttatchFieldsName;
    [SerializeField] List<Image> AttatchFieldsImage;
    [SerializeField] GameObject Goal;
    [SerializeField] GameObject Border;
    [SerializeField] GameObject Error;
    RectTransform MyRect;
    int AttatchNum = 0;
    bool IsGoalAttatched = false;

    private void Awake()
    {
        MyRect = GetComponent<RectTransform>();
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerEnter, OnPoint);
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerExit, OutPoint);
    }

    private void OutPoint(PointerEventData Data)
    {
        if (AN.IsDragged) AN.IsAttatched = false;
    }

    private void OnPoint(PointerEventData Data)
    {
        if (AN.IsDragged) AN.IsAttatched = true;
    }

    public void Attatching(Sprite image, string name, GameObject Goal)
    {
        if (this.Goal == Goal) IsGoalAttatched = true;
        if (AttatchNum == 0) AttatchFields[0].SetActive(false);
        AttatchFields[++AttatchNum].SetActive(true);
        AttatchFieldsName[AttatchNum].text = name;
        AttatchFieldsImage[AttatchNum].sprite = image;
        AN.IsAttatched = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(MyRect);
    }

    public void AttatchCancle(int num)
    {
        if(AttatchNum == 1) AttatchFields[0].SetActive(true);
        AttatchFields[AttatchNum--].SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(MyRect);
    }
    
    public void AttatchFail(string reason)
    {
        Border.SetActive(true);
        Error.SetActive(true);
        AN.IsAttatched = false;
    }


}
