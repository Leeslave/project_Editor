using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttatchFile_F : MonoBehaviour
{
    [SerializeField] ClearManager_N CN;
    [SerializeField] AttatchFile_N AN;
    [SerializeField] List<GameObject> AttatchFields;
    [SerializeField] List<TMP_Text> AttatchFieldsName;
    [SerializeField] List<Image> AttatchFieldsImage;
    [SerializeField] GameObject Goal;
    [SerializeField] GameObject Border;
    [SerializeField] GameObject Error;
    [SerializeField] TMP_Text ErrorMessage;
    [SerializeField] TMP_Text ErrorReason;
    RectTransform MyRect;
    int AttatchNum = 0;
    List<int> AttatchInd = new List<int>(5);
    bool[] GoalAttatched = new bool[5];

    private void Awake()
    {
        for (int i = 0; i < 5; i++) AttatchInd.Add(i);
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

    void TestTT()
    {
        string z = "";
        foreach(var a in AttatchInd)
        {
            z += a.ToString() + ",";
        }
        print(z);
    }
    public void Attatching(Sprite image, string name, GameObject Goal)
    {
        if(AttatchNum == 4)
        {
            AttatchFail("Ã·ºÎ ½ÇÆÐ","Ã·ºÎ »óÇÑ ÃÊ°ú");
            return;
        }
        if (AttatchNum == 0) AttatchFields[0].SetActive(false);
        ++AttatchNum;
        int s = AttatchInd[AttatchNum];
        if (this.Goal == Goal)
        {
            CN.IsGoalAttatched++;
            GoalAttatched[s] = true;
        }
        AttatchFields[s].SetActive(true);
        AttatchFieldsName[s].text = name;
        AttatchFieldsImage[s].sprite = image;
        AN.IsAttatched = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(MyRect);
    }

    public void AttatchCancle(int num)
    {
        if(AttatchNum == 1) AttatchFields[0].SetActive(true);
        AttatchFields[num].SetActive(false);
        if (num < AttatchNum)
        {
            AttatchFields[num].transform.SetSiblingIndex(AttatchNum);
            AttatchInd.Remove(num);
            AttatchInd.Insert(AttatchNum,num);
            if (GoalAttatched[num] == true)
            {
                GoalAttatched[num] = false;
                CN.IsGoalAttatched--;
            }
        }
        AttatchNum--;
        LayoutRebuilder.ForceRebuildLayoutImmediate(MyRect);
    }
    
    public void AttatchFail(string reason,string message)
    {
        Border.SetActive(true);
        Error.SetActive(true);
        AN.IsAttatched = false;
        ErrorReason.text = reason;
        ErrorMessage.text = message;
    }
}
