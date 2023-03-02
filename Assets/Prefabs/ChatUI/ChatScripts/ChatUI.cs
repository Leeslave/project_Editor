using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ChatUI : MonoBehaviour
{
    private GameObject[] choiceButton;          //선택지 버튼 UI
    private GameObject downSpeaker;             //발화자 UI
    private GameObject downText;                //채팅 내용 UI
    private GameObject downShadow;              //채팅 박스 UI

    private FileInfo fileTxt;
    private StreamReader chatTxt;               //이번 채팅 텍스트 파일
    private int currentChatTextLine;            //현재 대화 라인

    private string chatType;                    //채팅 타입
    private string chatSpeaker;                 //채팅 발화자
    private string SpeakerNumber;
    private int LastNumber;
    private string[] chatData;                  //채팅 내용
    private string[] choiceLine;                //선택지 내용

    private bool isOnFlowText;                  //흐름 출력 중
    private bool isAbleNext;                    //다음 채팅 출력 가능 여부

    public Image[] chatSpeakerImage;

    [Header("출력 완료 후 대기 시간")]
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
        if(Input.GetKeyDown(KeyCode.Space))//스페이스가 눌렸을 때
        {
            if(isOnFlowText)//흐름 출력 중이었다면 흐름 출력을 중단하고 즉시 출력한다
                TextImediately();
            else
            {
                if (isAbleNext && chatType == "D")//조건을 만족할 때 다음으로 넘어 갈 수 있다
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



    private void EnableMiddleOne()//중간 선택지 하나 활성화
    {
        choiceButton[0].SetActive(true);
    }

    private void EnableMiddleTwo()//중간 선택지 2개 활성화
    {
        choiceButton[0].SetActive(true);
        choiceButton[1].SetActive(true);
    }

    private void EnableMiddleThree()//중간 선택지 3개 활성화
    {
        choiceButton[0].SetActive(true);
        choiceButton[1].SetActive(true);
        choiceButton[2].SetActive(true);
    }

    private void UnableMiddleAll()//중간 선택지 전부 비활성화
    {
        choiceButton[0].SetActive(false);
        choiceButton[1].SetActive(false);
        choiceButton[2].SetActive(false);
    }

    private void ClearMiddleAll()//중간 선택지 전부 텍스트 비움
    {
        choiceButton[0].GetComponentInChildren<TextMeshProUGUI>().text = "";
        choiceButton[1].GetComponentInChildren<TextMeshProUGUI>().text = "";
        choiceButton[2].GetComponentInChildren<TextMeshProUGUI>().text = "";
    }

    private void EnableDown()//밑 활성화
    {
        downText.SetActive(true);
        downSpeaker.SetActive(true);
        downShadow.SetActive(true);
    }

    private void UnableDown()//밑 비활성화
    {
        downText.SetActive(false);
        downSpeaker.SetActive(false);
        downShadow.SetActive(false);
    }

    private void ClearDown()//밑 텍스트 비움
    {
        downSpeaker.GetComponent<TextMeshProUGUI>().text = "";
        downText.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void GetChatTxtFile(int fileNum)//이번 대화 텍스트 파일을 불러온다
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

    private void LoadNextChatData()//다음 대화를 출력시킨다
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

            if (newLine.Contains(':')) // 나
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
            
            if (newLine.Contains(':')) // 채팅
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
        else if (chatType == "C1")//선택지 하나
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
        else if (chatType == "C2")//선택지 둘
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
        else if(chatType == "C3")//선택지 셋
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

    private void TextImediately()//흐름 출력을 종료하고 즉시 출력시킨다
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

    public void OnChoiceDown(int choiceNum)//선택지 선택에 따른 결과를 반영한다
    {
        if (chatType == "C1")//C1일 때는 선택에 따른 변화가 없고 다음으로 넘긴다
        {
            LoadNextChatData();
            return;
        }

        OnMoveChatLine(int.Parse(choiceLine[choiceNum]));
    }

    public void OnMoveChatLine(int lineNum)//오브젝트 선택에 따른 대화를 출력한다
    {
        GetChatTxtFile(1);
        currentChatTextLine = lineNum - 1;
        for(int i=1;i<lineNum;i++)
        {
            chatTxt.ReadLine();
        }
        LoadNextChatData();
    }
    
    private void FlowTextInEndTime(TextMeshProUGUI target, string value, int idx, float endTime)//제한시간 흐름 출력
    {
        isOnFlowText = true;
        StartCoroutine(FlowTextInEndTimeIEnumerator(target, value, idx, endTime));
    }

    private IEnumerator FlowTextInEndTimeIEnumerator(TextMeshProUGUI target, string value, int idx, float endTime)//제한시간 흐름 출력 재귀
    {
        if (idx >= value.Length)
            yield break;
        target.text += value.Substring(idx, 1);
        yield return new WaitForSeconds(endTime/value.Length);
        StartCoroutine(FlowTextInEndTimeIEnumerator(target, value, idx + 1, endTime));
    }

    private void FlowTextWithDelayTime(TextMeshProUGUI target, string value, int idx, float delayTime)//딜레이 흐름 출력
    {
        isOnFlowText = true;//흐름 출력 중임을 알림
        isAbleNext = false;//다음 대화 출력 불가
        StartCoroutine(FlowtextWithDelayTimeIEnumerator(target, value, idx, delayTime));
    }

    private IEnumerator FlowtextWithDelayTimeIEnumerator(TextMeshProUGUI target, string value, int idx, float delayTime)//딜레이 흐름 출력 재귀
    {
        if (idx >= value.Length)//끝까지 다 출력했음
        {
            isOnFlowText = false;//흐름 출력 종료를 알림
            Invoke("SetTrue_isAbleNext",nextDelaytime);//다음 대화 출력은 0.5초 이후부터 가능
            yield break;
        }
        target.text += value.Substring(idx, 1);
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(FlowtextWithDelayTimeIEnumerator(target, value, idx + 1, delayTime));
    }

    private void SetTrue_isAbleNext()//Invoke용, 다음 대화로 넘어갈 수 있게 해준다
    {
        isAbleNext = true;
    }
}
