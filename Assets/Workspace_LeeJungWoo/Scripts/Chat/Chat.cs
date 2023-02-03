using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat : MonoBehaviour
{
    [Header("��ǥ CSV ���� �̸�")]
    public string fileName;

    [Header("���� ���� ȣ�� ��� �ð�")]
    public float delay;

    [Header("�ٷ� ���� ����")]
    public bool PlayOnAwake;

    //CSV ������
    private List<Dictionary<string, object>> data;

    //�ٷ� ���� �帧 ��� ���̾���?_�帧 ��� ���� Ÿ�̹� Ȯ�ο�
    private bool isLastNowFlowText;
    //���� ���� ȣ�� ���� ����
    private bool isAbleToMoveNextLine;
    //���� ����
    private int currentLine;

    //������ ��ư ������Ʈ
    private GameObject choice_0;
    private GameObject choice_1;
    private GameObject choice_2;

    //��ȭ�� ǥ�� �ؽ�Ʈ�޽�����UGUI
    private TextFieldProUGUI speaker;
    //��ȭ���� ǥ�� �ؽ�Ʈ�޽�����UGUI
    private TextFieldProUGUI content;
    //��ȭ���� �ٹ� �׸��� ������Ʈ
    private GameObject shadow;

    private void Start()
    {
        //CSV ������ �ε�
        data = CSVReader.Read("Assets/Workspace_LeeJungWoo/ChatCSV/" + fileName + ".csv");

        choice_0 = GameObject.Find("Choice_0");
        choice_1 = GameObject.Find("Choice_1");
        choice_2 = GameObject.Find("Choice_2");

        speaker = transform.Find("Speaker").GetComponent<TextFieldProUGUI>();
        content = transform.Find("Content").GetComponent<TextFieldProUGUI>();
        shadow = GameObject.Find("Shadow");

        UnvisibleDown();
        UnvisibleMiddleAll();

        if(PlayOnAwake)//�ٷ� ��ȭâ�� ���� ù ������ �ε��մϴ�
        {
            currentLine = 1;
            LoadLine(currentLine);
        }
    }

    private void Update()
    {
        if (isLastNowFlowText == true && content.GetIsNowFlowText() == false)//�帧 ����� ����Ǿ��ٸ�
        {
            //delay �ð� �Ŀ� �����ִ� ���� ���� ����� ����������
            Invoke("SetTrueIsAbleToNextLine", delay);
        }
        isLastNowFlowText = content.GetIsNowFlowText();

        if (Input.GetKeyDown(KeyCode.Space))//�����̽��� ������ ��
        {
            if (content.GetIsNowFlowText())//�帧 ��� ���̾��ٸ� �帧 ����� �ߴ��ϰ� ��� ����Ѵ�
            {
                if (data[currentLine - 1]["ChatType"].ToString() == "TD")
                {
                    content.StopCoroutineFlowTextWithDelay();
                    content.SetText(data[currentLine - 1]["Content"].ToString());
                }
            }
            else
            {
                if ((data[currentLine - 1]["ChatType"].ToString() == "TD") || (data[currentLine - 1]["ChatType"].ToString() == "TE"))//������ ������ �� �������� �Ѿ� �� �� �ִ�
                    if (isAbleToMoveNextLine)
                        LoadLine(++currentLine);
            }
        }
    }

    private void VisibleMiddleOne()//�߰� ������ �ϳ� Ȱ��ȭ
    {
        choice_0.transform.localPosition = new Vector3(0, 150);
    }

    private void VisibleMiddleTwo()//�߰� ������ 2�� Ȱ��ȭ
    {
        choice_0.transform.localPosition = new Vector3(0, 150);
        choice_1.transform.localPosition = new Vector3(0, 0);
    }

    private void VisibleMiddleThree()//�߰� ������ 3�� Ȱ��ȭ
    {
        choice_0.transform.localPosition = new Vector3(0, 150);
        choice_1.transform.localPosition = new Vector3(0, 0);
        choice_2.transform.localPosition = new Vector3(0, -150);
    }

    private void UnvisibleMiddleAll()//�߰� ������ ���� ��Ȱ��ȭ
    {
        choice_0.transform.localPosition = new Vector3(2000, 150);
        choice_1.transform.localPosition = new Vector3(2000, 0);
        choice_2.transform.localPosition = new Vector3(2000, -150);
    }

    private void VisibleDown()//�� Ȱ��ȭ
    {
        content.transform.localPosition = new Vector3(0, 0);
        speaker.transform.localPosition = new Vector3(0, 0);
        shadow.transform.localPosition = new Vector3(0, 0);
    }

    private void UnvisibleDown()//�� ��Ȱ��ȭ
    {
        content.transform.localPosition = new Vector3(2000, 0);
        speaker.transform.localPosition = new Vector3(2000, 0);
        shadow.transform.localPosition = new Vector3(2000, 0);
    }

    private void SetTrueIsAbleToNextLine()//���� ���� ��� ����, Invoke��
    {
        isAbleToMoveNextLine = true;
    }

    private void LoadLine(int line)//������ ������ �ε�
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

    public void OnChoiceDown(int choice)//�������� Ŭ��
    {
        currentLine = int.Parse(data[currentLine - 1]["Jump_" + choice.ToString()].ToString());
        LoadLine(currentLine);
    }

    public void OnCharOrObjDown(int line)//ĳ���ͳ� ������Ʈ�� Ŭ��
    {
        currentLine = line;
        LoadLine(currentLine);
    }
}
