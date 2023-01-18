using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat : MonoBehaviour
{
    [Header("목표 CSV 파일 경로")]
    public string filePath;

    [Header("다음 라인 대기 시간")]
    public float delay;

    private List<Dictionary<string, object>> data;

    private bool isLastNowFlowText;
    private bool isAbleToNextLine;

    private GameObject choice_0;
    private GameObject choice_1;
    private GameObject choice_2;

    private TextFieldProUGUI speaker;
    private TextFieldProUGUI content;
    private GameObject shadow;

    private int currentLine;

    private void Start()
    {
        data = CSVReader.Read(filePath);

        choice_0 = GameObject.Find("Choice_0");
        choice_1 = GameObject.Find("Choice_1");
        choice_2 = GameObject.Find("Choice_2");

        speaker = transform.Find("Speaker").GetComponent<TextFieldProUGUI>();
        content = transform.Find("Content").GetComponent<TextFieldProUGUI>();
        shadow = GameObject.Find("Shadow");

        UnvisibleDown();
        UnvisibleMiddleAll();

        currentLine = 1;
        LoadLine(currentLine);
    }

    private void Update()
    {
        if(isLastNowFlowText == true && content.GetIsNowFlowText() == false)
        {
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
                    if(isAbleToNextLine)
                        LoadLine(++currentLine);
            }
        }
    }

    private void VisibleMiddleOne()//중간 선택지 하나 활성화
    {
        choice_0.transform.localPosition = new Vector3(0, 125);
    }

    private void VisibleMiddleTwo()//중간 선택지 2개 활성화
    {
        choice_0.transform.localPosition = new Vector3(0, 125);
        choice_1.transform.localPosition = new Vector3(0, 0);
    }

    private void VisibleMiddleThree()//중간 선택지 3개 활성화
    {
        choice_0.transform.localPosition = new Vector3(0, 125);
        choice_1.transform.localPosition = new Vector3(0, 0);
        choice_2.transform.localPosition = new Vector3(0, -125);
    }

    private void UnvisibleMiddleAll()//중간 선택지 전부 비활성화
    {
        choice_0.transform.localPosition = new Vector3(1500, 125);
        choice_1.transform.localPosition = new Vector3(1500, 0);
        choice_2.transform.localPosition = new Vector3(1500, -125);
    }

    private void ClearMiddleAll()//중간 선택지 전부 텍스트 비움
    {
        choice_0.GetComponentInChildren<TextMeshProUGUI>().text = "";
        choice_1.GetComponentInChildren<TextMeshProUGUI>().text = "";
        choice_2.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }

    private void VisibleDown()//밑 활성화
    {
        content.transform.localPosition = new Vector3(0, 0);
        speaker.transform.localPosition = new Vector3(0, 0);
        shadow.transform.localPosition = new Vector3(0, 0);
    }

    private void UnvisibleDown()//밑 비활성화
    {
        content.transform.localPosition = new Vector3(1500, 0);
        speaker.transform.localPosition = new Vector3(1500, 0);
        shadow.transform.localPosition = new Vector3(1500, 0);
    }

    private void ClearDown()//밑 텍스트 비움
    {
        speaker.SetText("");
        content.SetText("");
    }

    private void SetTrueIsAbleToNextLine()
    {
        isAbleToNextLine = true;
    }

    private void LoadLine(int line)
    {
        Debug.Log("Line : " + line);

        string chatType = data[line - 1]["ChatType"].ToString();

        Dictionary<string, object> currentLineData = data[line - 1];

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
        }

        isAbleToNextLine = false;
    }

    public void OnChoiceDown(int choice)
    {
        currentLine = int.Parse(data[currentLine - 1]["Jump_" + choice.ToString()].ToString());
        LoadLine(currentLine);
    }

    public void OnCharOrObjDown(int line)
    {
        currentLine = line;
        LoadLine(currentLine);
    }
}
