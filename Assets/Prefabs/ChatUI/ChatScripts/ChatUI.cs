using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ChatUI : MonoBehaviour
{
    private GameObject[] choiceButton;          //������ ��ư UI
    private GameObject downSpeaker;             //��ȭ�� UI
    private GameObject downText;                //ä�� ���� UI
    private GameObject downShadow;              //ä�� �ڽ� UI

    private FileInfo fileTxt;
    private StreamReader chatTxt;               //�̹� ä�� �ؽ�Ʈ ����
    private int currentChatTextLine;            //���� ��ȭ ����

    private string chatType;                    //ä�� Ÿ��
    private string chatSpeaker;                 //ä�� ��ȭ��
    private string SpeakerNumber;
    private int LastNumber;
    private string[] chatData;                  //ä�� ����
    private string[] choiceLine;                //������ ����

    private bool isOnFlowText;                  //�帧 ��� ��
    private bool isAbleNext;                    //���� ä�� ��� ���� ����

    public Image[] chatSpeakerImage;

    [Header("��� �Ϸ� �� ��� �ð�")]
    public float nextDelaytime;

    private void Start()
    {
        choiceButton = new GameObject[3];
        choiceButton[0] = GameObject.Find("Choice_0");
        choiceButton[1] = GameObject.Find("Choice_1");
        choiceButton[2] = GameObject.Find("Choice_2");
        LastNumber = 5;
        downSpeaker = GameObject.Find("DownSpeaker");
        downText = GameObject.Find("DownText");
        downShadow = GameObject.Find("DownShadow");

        chatData = new string[3];
        choiceLine = new string[3];

        isOnFlowText = false;
        isAbleNext = true;

        UnableMiddleAll();
        UnableDown();
        ClearDown();
        ImageInit();
        GetChatTxtFile(1);

    }

    public void Start0Day()
    {
        Invoke("LoadNextChatData", 1);

    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))//�����̽��� ������ ��
        {
            if(isOnFlowText)//�帧 ��� ���̾��ٸ� �帧 ����� �ߴ��ϰ� ��� ����Ѵ�
                TextImediately();
            else
            {
                if (isAbleNext && chatType == "D")//������ ������ �� �������� �Ѿ� �� �� �ִ�
                    LoadNextChatData();
            }
        }
    }

    private void ImageInit()
    {
        for (int i = 0; i < chatSpeakerImage.Length; i++)
        {
            Debug.Log(chatSpeakerImage.Length);
            chatSpeakerImage[i].enabled = false;
        }
    }



    private void EnableMiddleOne()//�߰� ������ �ϳ� Ȱ��ȭ
    {
        choiceButton[0].SetActive(true);
    }

    private void EnableMiddleTwo()//�߰� ������ 2�� Ȱ��ȭ
    {
        choiceButton[0].SetActive(true);
        choiceButton[1].SetActive(true);
    }

    private void EnableMiddleThree()//�߰� ������ 3�� Ȱ��ȭ
    {
        choiceButton[0].SetActive(true);
        choiceButton[1].SetActive(true);
        choiceButton[2].SetActive(true);
    }

    private void UnableMiddleAll()//�߰� ������ ���� ��Ȱ��ȭ
    {
        choiceButton[0].SetActive(false);
        choiceButton[1].SetActive(false);
        choiceButton[2].SetActive(false);
    }

    private void ClearMiddleAll()//�߰� ������ ���� �ؽ�Ʈ ���
    {
        choiceButton[0].GetComponentInChildren<TextMeshProUGUI>().text = "";
        choiceButton[1].GetComponentInChildren<TextMeshProUGUI>().text = "";
        choiceButton[2].GetComponentInChildren<TextMeshProUGUI>().text = "";
    }

    private void EnableDown()//�� Ȱ��ȭ
    {
        downText.SetActive(true);
        downSpeaker.SetActive(true);
        downShadow.SetActive(true);
    }

    private void UnableDown()//�� ��Ȱ��ȭ
    {
        downText.SetActive(false);
        downSpeaker.SetActive(false);
        downShadow.SetActive(false);
    }

    private void ClearDown()//�� �ؽ�Ʈ ���
    {
        downSpeaker.GetComponent<TextMeshProUGUI>().text = "";
        downText.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void GetChatTxtFile(int fileNum)//�̹� ��ȭ �ؽ�Ʈ ������ �ҷ��´�
    {
        string filePath = "Assets/Workspace_LeeJungWoo/Prefab/ChatUI/ChatTxt/ChatTxt_" + fileNum.ToString() + ".txt";
        fileTxt = new FileInfo(filePath);

        if (fileTxt.Exists)
        {
            chatTxt = new StreamReader(filePath, System.Text.Encoding.UTF8);
            currentChatTextLine = 0;
        }
        else
            Debug.Log("Unexist filepath!");
    }

    private void LoadNextChatData()//���� ��ȭ�� ��½�Ų��
    {
        string newLine = chatTxt.ReadLine();
        currentChatTextLine++;
        Debug.Log(currentChatTextLine);

        chatType = newLine.Substring(0, newLine.IndexOf(':'));
        newLine = newLine.Substring(newLine.IndexOf(':') + 1);

        if (chatType == "D")
        {
            if (newLine.Contains(':')) // D
            {
                chatSpeaker = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }

            if (newLine.Contains(':')) // ��
            {
                SpeakerNumber = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
                if (LastNumber != int.Parse(SpeakerNumber))
                {
                    if (LastNumber != 0)
                    {
                        chatSpeakerImage[LastNumber].enabled = false;
                    }
                    chatSpeakerImage[int.Parse(SpeakerNumber)].enabled = true;
                }
                LastNumber = int.Parse(SpeakerNumber);
            }
            
            if (newLine.Contains(':')) // ä��
            {
                chatData[0] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }

            EnableDown();
            UnableMiddleAll();
            ClearDown();
            downSpeaker.GetComponent<TextMeshProUGUI>().text = chatSpeaker;
            FlowTextWithDelayTime(downText.GetComponent<TextMeshProUGUI>(), chatData[0], 0, 0.03f);
            return;
        }
        else if (chatType == "C1")//������ �ϳ�
        {
            if (newLine.Contains(':'))
            {
                chatData[0] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }

            EnableMiddleOne();
            UnableDown();
            ClearMiddleAll();
            choiceButton[0].GetComponentInChildren<TextMeshProUGUI>().text = chatData[0];
            choiceButton[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            choiceButton[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            return;
        }
        else if (chatType == "C2")//������ ��
        {
            if (newLine.Contains(':'))
            {
                chatData[0] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }
            if (newLine.Contains(':'))
            {
                chatData[1] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }
            if (newLine.Contains(':'))
            {
                choiceLine[0] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }
            if (newLine.Contains(':'))
            {
                choiceLine[1] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }

            EnableMiddleTwo();
            UnableDown();
            ClearMiddleAll();
            choiceButton[0].GetComponentInChildren<TextMeshProUGUI>().text = chatData[0];
            choiceButton[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 75);
            choiceButton[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 75);
            choiceButton[1].GetComponentInChildren<TextMeshProUGUI>().text = chatData[1];
            choiceButton[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
            choiceButton[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);

        }
        else if(chatType == "C3")//������ ��
        {
            if (newLine.Contains(':'))
            {
                chatData[0] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }
            if (newLine.Contains(':'))
            {
                chatData[1] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }
            if (newLine.Contains(':'))
            {
                chatData[2] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }
            if (newLine.Contains(':'))
            {
                choiceLine[0] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }
            if (newLine.Contains(':'))
            {
                choiceLine[1] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }
            if (newLine.Contains(':'))
            {
                choiceLine[2] = newLine.Substring(0, newLine.IndexOf(':'));
                newLine = newLine.Substring(newLine.IndexOf(':') + 1);
            }

            EnableMiddleThree();
            UnableDown();
            ClearMiddleAll();
            choiceButton[0].GetComponentInChildren<TextMeshProUGUI>().text = chatData[0];
            choiceButton[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
            choiceButton[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
            choiceButton[1].GetComponentInChildren<TextMeshProUGUI>().text = chatData[1];
            choiceButton[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            choiceButton[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            choiceButton[2].GetComponentInChildren<TextMeshProUGUI>().text = chatData[2];
            choiceButton[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150);
            choiceButton[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150);
        }
        else if(chatType == "End")
        {
            ImageInit();
            UnableDown();
            UnableMiddleAll();
        }
    }

    private void TextImediately()//�帧 ����� �����ϰ� ��� ��½�Ų��
    {
        StopAllCoroutines();
        if (chatType == "D")
        {
            ClearDown();
            downSpeaker.GetComponent<TextMeshProUGUI>().text = chatSpeaker;
            downText.GetComponent<TextMeshProUGUI>().text = chatData[0];
        }
        isOnFlowText = false;
        Invoke("SetTrue_isAbleNext", nextDelaytime);
    }

    public void OnChoiceDown(int choiceNum)//������ ���ÿ� ���� ����� �ݿ��Ѵ�
    {
        if (chatType == "C1")//C1�� ���� ���ÿ� ���� ��ȭ�� ���� �������� �ѱ��
        {
            LoadNextChatData();
            return;
        }

        OnMoveChatLine(int.Parse(choiceLine[choiceNum]));
    }

    public void OnMoveChatLine(int lineNum)//������Ʈ ���ÿ� ���� ��ȭ�� ����Ѵ�
    {
        GetChatTxtFile(1);
        currentChatTextLine = lineNum - 1;
        for(int i=1;i<lineNum;i++)
        {
            chatTxt.ReadLine();
        }
        LoadNextChatData();
    }
    
    private void FlowTextInEndTime(TextMeshProUGUI target, string value, int idx, float endTime)//���ѽð� �帧 ���
    {
        isOnFlowText = true;
        StartCoroutine(FlowTextInEndTimeIEnumerator(target, value, idx, endTime));
    }

    private IEnumerator FlowTextInEndTimeIEnumerator(TextMeshProUGUI target, string value, int idx, float endTime)//���ѽð� �帧 ��� ���
    {
        if (idx >= value.Length)
            yield break;
        target.text += value.Substring(idx, 1);
        yield return new WaitForSeconds(endTime/value.Length);
        StartCoroutine(FlowTextInEndTimeIEnumerator(target, value, idx + 1, endTime));
    }

    private void FlowTextWithDelayTime(TextMeshProUGUI target, string value, int idx, float delayTime)//������ �帧 ���
    {
        isOnFlowText = true;//�帧 ��� ������ �˸�
        isAbleNext = false;//���� ��ȭ ��� �Ұ�
        StartCoroutine(FlowtextWithDelayTimeIEnumerator(target, value, idx, delayTime));
    }

    private IEnumerator FlowtextWithDelayTimeIEnumerator(TextMeshProUGUI target, string value, int idx, float delayTime)//������ �帧 ��� ���
    {
        if (idx >= value.Length)//������ �� �������
        {
            isOnFlowText = false;//�帧 ��� ���Ḧ �˸�
            Invoke("SetTrue_isAbleNext",nextDelaytime);//���� ��ȭ ����� 0.5�� ���ĺ��� ����
            yield break;
        }
        target.text += value.Substring(idx, 1);
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(FlowtextWithDelayTimeIEnumerator(target, value, idx + 1, delayTime));
    }

    private void SetTrue_isAbleNext()//Invoke��, ���� ��ȭ�� �Ѿ �� �ְ� ���ش�
    {
        isAbleNext = true;
    }
}
