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
    [SerializeField] TMP_Text ErrorMessage;
    [SerializeField] TMP_Text ErrorReason;
    RectTransform MyRect;
    // 현재 첨부 된 파일의 수
    int AttatchNum = 0;
    // 첨부 취소 시 기존 자식의 순서가 변환되어
    // 활성화 되었을 때 기준 몇번 자식이 현재 어떤 위치에 있는지 기록에 사용.
    List<int> AttatchInd = new List<int>(5);
    // 해당 Index에 목표가 첨부되었는지 여부(True면 첨부된 것)
    bool[] GoalAttatched = new bool[5];
    // 목표의 첨부 여부
    public int IsGoalAttatched;

    private void Awake()
    {
        for (int i = 0; i < 5; i++) AttatchInd.Add(i);
        MyRect = GetComponent<RectTransform>();
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerEnter, OnPoint);
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerExit, OutPoint);
    }

    /// <summary>
    /// 비활성화 시 초기화 시킨다.
    /// </summary>
    private void OnDisable()
    {
        AttatchNum = 0;
        for (int i = 0; i < 5; i++) GoalAttatched[i] = false;
        for (int i = 0; i < 5; i++) AttatchInd[i] = i;
        for (int i = 0; i < 5; i++) AttatchFields[i].SetActive(false);
    }

    // 첨부 여부 판단(드래그)
    private void OutPoint(PointerEventData Data)
    {
        if (AN.IsDragged) AN.IsAttatched = false;
    }

    private void OnPoint(PointerEventData Data)
    {
        if (AN.IsDragged) AN.IsAttatched = true;
    }

    /// <summary>
    /// 필드에 Icon을 첨부시킨다.
    /// 특정 목표의 첨부 여부를 판단 가능하다.
    /// </summary>
    /// <param name="image"> 첨부된 Icon의 이미지 </param>
    /// <param name="name"> 첨부된 Icon의 이름 </param>
    /// <param name="Goal"> 특정 Object의 첨부 여부 판단을 위해 사용(첨부된 Icon) </param>
    public void Attatching(Sprite image, string name, GameObject Goal)
    {
        if(AttatchNum == 4)
        {
            AttatchFail("첨부 실패","첨부 상한 초과");
            return;
        }
        if (AttatchNum == 0) AttatchFields[0].SetActive(false);
        ++AttatchNum;
        int s = AttatchInd[AttatchNum];
        if (this.Goal == Goal)
        {
            IsGoalAttatched++;
            GoalAttatched[s] = true;
        }
        AttatchFields[s].SetActive(true);
        AttatchFieldsName[s].text = name;
        AttatchFieldsImage[s].sprite = image;
        AN.IsAttatched = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(MyRect);
    }

    /// <summary>
    /// 첨부를 취소시킨다.
    /// </summary>
    /// <param name="num"> 취소시킨 파일이 필드상에서 몇번 째 파일인지 </param>
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
                IsGoalAttatched--;
            }
        }
        AttatchNum--;
        LayoutRebuilder.ForceRebuildLayoutImmediate(MyRect);
    }
    
    /// <summary>
    /// 특정 이유로 첨부에 실패한 경우 에러 메세지를 띄운다.
    /// </summary>
    /// <param name="reason"> 첨부 실패 이유(간략) </param>
    /// <param name="message"> 첨부 실패 이유(자세히) </param>
    public void AttatchFail(string reason,string message)
    {
        Border.SetActive(true);
        Error.SetActive(true);
        AN.IsAttatched = false;
        ErrorReason.text = reason;
        ErrorMessage.text = message;
    }
}
