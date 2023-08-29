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
    // ���� ÷�� �� ������ ��
    int AttatchNum = 0;
    // ÷�� ��� �� ���� �ڽ��� ������ ��ȯ�Ǿ�
    // Ȱ��ȭ �Ǿ��� �� ���� ��� �ڽ��� ���� � ��ġ�� �ִ��� ��Ͽ� ���.
    List<int> AttatchInd = new List<int>(5);
    // �ش� Index�� ��ǥ�� ÷�εǾ����� ����(True�� ÷�ε� ��)
    bool[] GoalAttatched = new bool[5];
    // ��ǥ�� ÷�� ����
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
    /// ��Ȱ��ȭ �� �ʱ�ȭ ��Ų��.
    /// </summary>
    private void OnDisable()
    {
        AttatchNum = 0;
        for (int i = 0; i < 5; i++) GoalAttatched[i] = false;
        for (int i = 0; i < 5; i++) AttatchInd[i] = i;
        for (int i = 0; i < 5; i++) AttatchFields[i].SetActive(false);
    }

    // ÷�� ���� �Ǵ�(�巡��)
    private void OutPoint(PointerEventData Data)
    {
        if (AN.IsDragged) AN.IsAttatched = false;
    }

    private void OnPoint(PointerEventData Data)
    {
        if (AN.IsDragged) AN.IsAttatched = true;
    }

    /// <summary>
    /// �ʵ忡 Icon�� ÷�ν�Ų��.
    /// Ư�� ��ǥ�� ÷�� ���θ� �Ǵ� �����ϴ�.
    /// </summary>
    /// <param name="image"> ÷�ε� Icon�� �̹��� </param>
    /// <param name="name"> ÷�ε� Icon�� �̸� </param>
    /// <param name="Goal"> Ư�� Object�� ÷�� ���� �Ǵ��� ���� ���(÷�ε� Icon) </param>
    public void Attatching(Sprite image, string name, GameObject Goal)
    {
        if(AttatchNum == 4)
        {
            AttatchFail("÷�� ����","÷�� ���� �ʰ�");
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
    /// ÷�θ� ��ҽ�Ų��.
    /// </summary>
    /// <param name="num"> ��ҽ�Ų ������ �ʵ�󿡼� ��� ° �������� </param>
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
    /// Ư�� ������ ÷�ο� ������ ��� ���� �޼����� ����.
    /// </summary>
    /// <param name="reason"> ÷�� ���� ����(����) </param>
    /// <param name="message"> ÷�� ���� ����(�ڼ���) </param>
    public void AttatchFail(string reason,string message)
    {
        Border.SetActive(true);
        Error.SetActive(true);
        AN.IsAttatched = false;
        ErrorReason.text = reason;
        ErrorMessage.text = message;
    }
}
