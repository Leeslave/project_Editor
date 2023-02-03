using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat : MonoBehaviour
{
    [Header("목표 CSV 파일 이름")]
    public string fileName;

    [Header("다음 라인 호출 대기 시간")]
    public float delay;

    [Header("바로 시작 여부")]
    public bool PlayOnAwake;

    //CSV 데이터
    private List<Dictionary<string, object>> data;

    //바로 전에 흐름 출력 중이었나?_흐름 출력 종료 타이밍 확인용
    private bool isLastNowFlowText;
    //다음 라인 호출 가능 여부
    private bool isAbleToMoveNextLine;
    //현재 라인
    private int currentLine;

    //선택지 버튼 오브젝트
    private GameObject choice_0;
    private GameObject choice_1;
    private GameObject choice_2;

    //발화자 표시 텍스트메시프로UGUI
    private TextFieldProUGUI speaker;
    //대화내용 표시 텍스트메시프로UGUI
    private TextFieldProUGUI content;
    //대화내용 꾸밈 그림자 오브젝트
    private GameObject shadow;

    private void Start()
    {
        //CSV 데이터 로드
        data = CSVReader.Read("Assets/Workspace_LeeJungWoo/ChatCSV/" + fileName + ".csv");

        choice_0 = GameObject.Find("Choice_0");
        choice_1 = GameObject.Find("Choice_1");
        choice_2 = GameObject.Find("Choice_2");

        speaker = transform.Find("Speaker").GetComponent<TextFieldProUGUI>();
        content = transform.Find("Content").GetComponent<TextFieldProUGUI>();
        shadow = GameObject.Find("Shadow");

        UnvisibleDown();
        UnvisibleMiddleAll();

        if(PlayOnAwake)//바로 대화창을 열고 첫 라인을 로드합니다
        {
            currentLine = 1;
            LoadLine(currentLine);
        }
    }

    private void Update()
    {
        if (isLastNowFlowText == true && content.GetIsNowFlowText() == false)//흐름 출력이 종료되었다면
        {
            //delay 시간 후에 막혀있던 다음 라인 출력이 가능해진다
            Invoke("SetTrueIsAbleToNextLine", delay);
        }
        isLastNowFlowText = content.GetIsNowFlowText();

        if (Input.GetKeyDown(KeyCode.Space))//스페이스가 눌렸을 때
        {
            if (content.GetIsNowFlowText())//흐름 출력 중이었다면 흐름 출력을 중단하고 즉시 출력한다
            {
                if (data[currentLine - 1]["ChatType"].ToString() == "TD")
                {
                    content.StopCoroutineFlowTextWithDelay();
                    content.SetText(data[currentLine - 1]["Content"].ToString());
                }
            }
            else
            {
                if ((data[currentLine - 1]["ChatType"].ToString() == "TD") || (data[currentLine - 1]["ChatType"].ToString() == "TE"))//조건을 만족할 때 다음으로 넘어 갈 수 있다
                    if (isAbleToMoveNextLine)
                        LoadLine(++currentLine);
            }
        }
    }

    private void VisibleMiddleOne()//중간 선택지 하나 활성화
    {
        choice_0.transform.localPosition = new Vector3(0, 150);
    }

    private void VisibleMiddleTwo()//중간 선택지 2개 활성화
    {
        choice_0.transform.localPosition = new Vector3(0, 150);
        choice_1.transform.localPosition = new Vector3(0, 0);
    }

    private void VisibleMiddleThree()//중간 선택지 3개 활성화
    {
        choice_0.transform.localPosition = new Vector3(0, 150);
        choice_1.transform.localPosition = new Vector3(0, 0);
        choice_2.transform.localPosition = new Vector3(0, -150);
    }

    private void UnvisibleMiddleAll()//중간 선택지 전부 비활성화
    {
        choice_0.transform.localPosition = new Vector3(2000, 150);
        choice_1.transform.localPosition = new Vector3(2000, 0);
        choice_2.transform.localPosition = new Vector3(2000, -150);
    }

    private void VisibleDown()//밑 활성화
    {
        content.transform.localPosition = new Vector3(0, 0);
        speaker.transform.localPosition = new Vector3(0, 0);
        shadow.transform.localPosition = new Vector3(0, 0);
    }

    private void UnvisibleDown()//밑 비활성화
    {
        content.transform.localPosition = new Vector3(2000, 0);
        speaker.transform.localPosition = new Vector3(2000, 0);
        shadow.transform.localPosition = new Vector3(2000, 0);
    }

    private void SetTrueIsAbleToNextLine()//다음 라인 출력 가능, Invoke용
    {
        isAbleToMoveNextLine = true;
    }

    private void LoadLine(int line)//지정된 라인을 로드
    {
        Debug.Log("Line : " + line);
        Dictionary<string, object> currentLineData = data[line - 1];

        string chatType = currentLineData["ChatType"].ToString();
        if (chatType == "TD")
        {
            UnvisibleMiddleAll();
            VisibleDown();
            speaker.SetText(currentLineData["Speaker"].ToString());
            float delay = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 0.03f;
            content.FlowTextWithDelay(currentLineData["Content"].ToString(), delay);
        }
        else if(chatType == "TE")
        {
            VisibleDown();
            speaker.SetText(currentLineData["Speaker"].ToString());
            float endTime = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 3.0f;
            content.FlowTextWithEndTime(currentLineData["Content"].ToString(), endTime);
        }
        else if(chatType == "C1")
        {
            UnvisibleDown();
            VisibleMiddleOne();
            choice_0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
        }
        else if (chatType == "C2")
        {
            UnvisibleDown();
            VisibleMiddleTwo();
            choice_0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
            choice_1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
        }
        else if (chatType == "C3")
        {
            UnvisibleDown();
            VisibleMiddleThree();
            choice_0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
            choice_1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
            choice_2.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_2"].ToString();
        }
        else if (chatType == "J")
        {
            LoadLine(int.Parse(currentLineData["Jump_0"].ToString()));
        }
        else if(chatType == "E")
        {
            UnvisibleDown();
            UnvisibleMiddleAll();
            SetLayerDefault();
        }

        isAbleToMoveNextLine = false;
    }

    protected virtual void SetLayerDefault()
    {

    }

    public void OnChoiceDown(int choice)//선택지가 클릭
    {
        currentLine = int.Parse(data[currentLine - 1]["Jump_" + choice.ToString()].ToString());
        LoadLine(currentLine);
    }

    public void OnCharOrObjDown(int line)//캐릭터나 오브젝트가 클릭
    {
        currentLine = line;
        LoadLine(currentLine);
    }
}
