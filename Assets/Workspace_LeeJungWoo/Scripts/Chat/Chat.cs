using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    [Header("CSV ���� �̸�")]
    public string FileName;

    [Header("���� ���� ȣ�� ��� �ð�")]
    public float Delay;

    [Header("�ٷ� ���� ����")]
    public bool PlayOnAwake;

    [Header("�ٽú��� ��")]
    public GameObject pastLine;

    [Header("�ٽú��� �ܶ�")]
    public GameObject pastParagraph;

    //����� �ҽ�
    private AudioSource audioSource;

    //CSV ������
    protected List<Dictionary<string, object>> data;

    private bool isLastNowFlowText;
    private bool isAbleToMoveNextLine;
    private int currentLine;

    private GameObject button_Choice0;
    private GameObject button_Choice1;
    private GameObject button_Choice2;
    private GameObject button_System;
    private GameObject button_Skip;
    private GameObject button_Remind;
    private GameObject panel_Dialog;
    private GameObject panel_Remind;
    private GameObject panel_Skip;

    private bool isRemindOpened;
    private bool isSkipOpened;

    private TextFieldProUGUI belong;
    private TextFieldProUGUI speaker;
    private TextFieldProUGUI dialog;

    private void Start()
    {
        //CSV ������ �ε�
        data = CSVReader.Read("Assets/Workspace_LeeJungWoo/ChatCSV/" + FileName + ".csv");

        button_Choice0 = GameObject.Find("Button_Choice0");
        button_Choice1 = GameObject.Find("Button_Choice1");
        button_Choice2 = GameObject.Find("Button_Choice2");
        button_System = GameObject.Find("Button_System");
        button_Remind = GameObject.Find("Button_Remind");
        button_Skip = GameObject.Find("Button_Skip");
        panel_Dialog = GameObject.Find("Panel_Dialog");
        panel_Remind = GameObject.Find("Panel_Remind");
        panel_Skip = GameObject.Find("Panel_Skip");

        audioSource = transform.Find("AudioSource").GetComponent<AudioSource>();
        belong = GameObject.Find("Belong").GetComponent<TextFieldProUGUI>();
        speaker = GameObject.Find("Speaker").GetComponent<TextFieldProUGUI>();
        dialog = GameObject.Find("Dialog").GetComponent<TextFieldProUGUI>();

        if (PlayOnAwake)//�ٷ� ��ȭâ�� ���� ù ������ �ε�
        {
            currentLine = 1;
            LoadLine(currentLine);
        }
        else
        {
            UnvisibleDialogPanel();
            UnvisibleMiddleAll();
        }
    }

    private void Update()
    {
        ControlInput();
    }

    private void ControlInput()//�Է� ����
    {
        if (isLastNowFlowText == true && dialog.GetIsNowFlowText() == false)//�帧 ����� ����Ǿ��ٸ� delay �ð� �Ŀ� �����ִ� ���� ���� ����� ����������
            Invoke("SetTrueIsAbleToNextLine", Delay);
        isLastNowFlowText = dialog.GetIsNowFlowText();

        if (Input.GetKeyDown(KeyCode.Space))//�����̽��� ������ ��
        {
            switch (dialog.GetIsNowFlowText())
            {
                case true://�帧 ��� ���̾���
                    if (data[currentLine - 1]["ChatType"].ToString() == "TD")
                    {
                        dialog.StopCoroutineFlowTextWithDelay();
                        dialog.SetText(data[currentLine - 1]["Dialog"].ToString());
                    }
                    break;
                case false://�帧 ��� ���� �ƴϾ���
                    bool condition_0 = (data[currentLine - 1]["ChatType"].ToString() == "TD") || (data[currentLine - 1]["ChatType"].ToString() == "TE");
                    bool condition_1 = isAbleToMoveNextLine;
                    bool condition_2 = !isRemindOpened && !isSkipOpened;
                    if (condition_0 && condition_1 && condition_2)
                        LoadLine(++currentLine);
                    break;
            }
        }
    }

    private void VisibleSystemButton()//�ý��� ��ư Ȱ��ȭ
    {
        button_System.transform.localPosition = new Vector3(850, 450);
    }

    private void UnvisibleSystemButton()//�ý��� ��ư ��Ȱ��ȭ
    {
        button_System.transform.localPosition = new Vector3(2000, 450);
    }

    private void VisibleSkipButton()//��ŵ ��ư Ȱ��ȭ
    {
        button_Skip.transform.localPosition = new Vector3(850, 325);
    }

    private void UnvisibleSkipButton()//��ŵ ��ư ��Ȱ��ȭ
    {
        button_Skip.transform.localPosition = new Vector3(2000, 325);
    }

    private void VisibleRemindButton()//�ٽú��� ��ư Ȱ��ȭ
    {
        button_Remind.transform.localPosition = new Vector3(850, 200);
    }

    private void UnvisibleRemindButton()//�ٽú��� ��ư ��Ȱ��ȭ
    {
        button_Remind.transform.localPosition = new Vector3(2000, 200);
    }

    private void VisibleRemindPanel()//�ٽú��� �г� Ȱ��ȭ
    {
        UnvisibleDialogPanel();
        UnvisibleSystemButton();
        UnvisibleSkipButton();
        UnvisibleRemindButton();

        panel_Remind.transform.localPosition = new Vector3(0, 0);
        isRemindOpened = true;

        if (panel_Remind.transform.Find("ScrollbarVertical") != null)//��ũ�ѹٿ� Content�� ������ ��ũ�ѹٰ� �������� ���� ���ɼ�
            panel_Remind.transform.Find("ScrollbarVertical").GetComponent<Scrollbar>().value = 0f;  //���� �ֱ��� ��ȭ�� �ٷ� �̵�
    }

    private void UnvisibleRemindPanel()//�ٽú��� �г� ��Ȱ��ȭ
    {
        if (data[currentLine - 1]["ChatType"].ToString() == "E")
        {
            VisibleSystemButton();
            button_Remind.transform.localPosition = new Vector3(850, 325);
        }
        else
        {
            VisibleDialogPanel();
            VisibleSystemButton();
            VisibleSkipButton();
            VisibleRemindButton();
        }

        panel_Remind.transform.localPosition = new Vector3(2000, 0);
        isRemindOpened = false;
    }

    private void VisibleSkipPanel()//��ŵ �г� Ȱ��ȭ
    {
        panel_Skip.transform.localPosition = new Vector3(0, 0);
        isSkipOpened = true;
    }

    private void UnvisibleSkipPanel()//��ŵ �г� ��Ȱ��ȭ
    {
        panel_Skip.transform.localPosition = new Vector3(4000, 0);
        isSkipOpened = false;
    }

    private void AddLineToRemindPanel(string speaker, string belong, string dialog)//�ٽú��⿡ �� �߰�
    {
        GameObject one;
        one = Instantiate(pastLine, panel_Remind.transform.Find("Viewport").transform.Find("Content"));
        one.transform.Find("Speaker").GetComponent<TextMeshProUGUI>().text = speaker;
        one.transform.Find("Belong").GetComponent<TextMeshProUGUI>().text = belong;
        one.transform.Find("Dialog").GetComponent<TextMeshProUGUI>().text = dialog;
        Canvas.ForceUpdateCanvases();
        float newPosX = one.transform.Find("Speaker").transform.localPosition.x + one.transform.Find("Speaker").GetComponent<RectTransform>().rect.width + 20;
        float newPosY = one.transform.Find("Belong").transform.localPosition.y;
        one.transform.Find("Belong").transform.localPosition = new Vector3(newPosX, newPosY, 0);
    }

    private void AddParagraphToRemindPanel(string paragraph)//�ٽú��⿡ �ܶ� �߰�
    {
        GameObject one;
        one = Instantiate(pastParagraph, panel_Remind.transform.Find("Viewport").transform.Find("Content"));
        one.transform.Find("Paragraph").GetComponent<TextMeshProUGUI>().text = paragraph;
    }

    private void VisibleMiddleOne()//�߰� ������ �ϳ� Ȱ��ȭ
    {
        button_Choice0.transform.localPosition = new Vector3(0, 150);
    }

    private void VisibleMiddleTwo()//�߰� ������ 2�� Ȱ��ȭ
    {
        button_Choice0.transform.localPosition = new Vector3(0, 75);
        button_Choice1.transform.localPosition = new Vector3(0, -75);
    }

    private void VisibleMiddleThree()//�߰� ������ 3�� Ȱ��ȭ
    {
        button_Choice0.transform.localPosition = new Vector3(0, 150);
        button_Choice1.transform.localPosition = new Vector3(0, 0);
        button_Choice2.transform.localPosition = new Vector3(0, -150);
    }

    private void UnvisibleMiddleAll()//�߰� ������ ���� ��Ȱ��ȭ
    {
        button_Choice0.transform.localPosition = new Vector3(2000, 150);
        button_Choice1.transform.localPosition = new Vector3(2000, 0);
        button_Choice2.transform.localPosition = new Vector3(2000, -150);
    }

    private void VisibleDialogPanel()//�� Ȱ��ȭ
    {
        panel_Dialog.transform.localPosition = new Vector3(0, 0);
    }

    private void UnvisibleDialogPanel()//�� ��Ȱ��ȭ
    {
        panel_Dialog.transform.localPosition = new Vector3(2000, 0);
    }

    private void SetTrueIsAbleToNextLine()//���� ���� ��� ����, Invoke��
    {
        isAbleToMoveNextLine = true;
    }

    public void OnChoiceDown(int choice)//�������� Ŭ��
    {
        AddLineToRemindPanel(data[currentLine - 1]["Speaker"].ToString(), data[currentLine - 1]["Belong"].ToString(), data[currentLine - 1]["Choice_" + choice.ToString()].ToString());
        currentLine = int.Parse(data[currentLine - 1]["Jump_" + choice.ToString()].ToString());
        LoadLine(currentLine);
    }

    public void OnRemindDown()//�ٽú��� Ŭ��
    {
        if (isRemindOpened)
            UnvisibleRemindPanel();
        else
            VisibleRemindPanel();
    }

    public void OnSkipDown()//��ŵ Ŭ��
    {
        if (isSkipOpened)
        {
            VisibleDialogPanel();
            VisibleSystemButton();
            VisibleSkipButton();
            VisibleRemindButton();
            UnvisibleSkipPanel();
        }
        else
        {
            UnvisibleDialogPanel();
            UnvisibleSystemButton();
            UnvisibleSkipButton();
            UnvisibleRemindButton();
            VisibleSkipPanel();
        }
    }

    public void ExecuteSkip()//��ŵ ����
    {
        Debug.Log("ExecuteSkipOrder");

        UnvisibleSkipPanel();

        for(currentLine += 1; ;currentLine++)
        {
            Dictionary<string, object> currentLineData = data[currentLine - 1];
            Dictionary<string, object> nextChatLineData = data[currentLine] != null ? data[currentLine] : null;
            
            bool condition0 = nextChatLineData["ChatType"].ToString() == "C1";
            bool condition1 = nextChatLineData["ChatType"].ToString() == "C2";
            bool condition2 = nextChatLineData["ChatType"].ToString() == "C3";
            bool isBeforeChoiceLine = condition0 || condition1 || condition2;

            if (currentLineData["RemindInstruction"].ToString() != "")
                AddParagraphToRemindPanel(currentLineData["RemindInstruction"].ToString());

            switch (currentLineData["ChatType"].ToString())
            {
                case "TD":
                    if(isBeforeChoiceLine)
                    {
                        LoadLine(currentLine);
                        return;
                    }
                    else
                        AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                    break;
                case "TE":
                    if(isBeforeChoiceLine)
                    {
                        LoadLine(currentLine);
                        return;
                    }
                    else
                        AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                    break;
                case "J":
                    currentLine = int.Parse(currentLineData["Jump_0"].ToString());                     
                    break;
                case "E":
                    UnvisibleDialogPanel();
                    UnvisibleMiddleAll();
                    UnvisibleSkipButton();
                    UnvisibleSkipPanel();
                    VisibleSystemButton();
                    button_Remind.transform.localPosition = new Vector3(850, 325);
                    SetLayerDefault();
                    return;
            }
        } 
    }

    public virtual void LoadLine(int line)//������ ������ �ε�
    {
        Debug.Log("LoadLine : " + line);

        //���� �� ������Ʈ
        currentLine = line;

        //���� ���
        audioSource.Play();

        //�������� �Ѿ�� �ʵ��� ����
        isAbleToMoveNextLine = false;

        //���� ���� ������ �ε�
        Dictionary<string, object> currentLineData = data[line - 1];

        //�ܶ� ������ �ٽú��⿡ �߰�
        if (currentLineData["RemindInstruction"].ToString() != "")
            AddParagraphToRemindPanel(currentLineData["RemindInstruction"].ToString());

        switch (currentLineData["ChatType"].ToString())
        {
            case "TD":
                //Delay�� ���� �ؽ�Ʈ ���
                UnvisibleMiddleAll();
                VisibleDialogPanel();
                VisibleSystemButton();
                VisibleSkipButton();
                VisibleRemindButton();

                //speaker�� belong�� �ؽ�Ʈ�� �����Ѵ�
                speaker.SetText(currentLineData["Speaker"].ToString());
                belong.SetText(currentLineData["Belong"].ToString());
                
                //delay�� Ȯ���ϰ� �帧 ����Ѵ�
                float delay = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 0.03f;
                dialog.FlowTextWithDelay(currentLineData["Dialog"].ToString(), delay);
                
                //belong�� speaker�� 20 �ڿ� �ֵ��� ���ġ
                belong.transform.localPosition = new Vector3(speaker.transform.localPosition.x + speaker.GetWidth() + 20, belong.transform.localPosition.y, 0);

                //�ٽú��⿡ �� �߰�
                AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                break;
            case "TE":
                //EndTime�� ���� �ؽ�Ʈ ���
                UnvisibleMiddleAll();
                VisibleDialogPanel();
                VisibleSystemButton();
                VisibleSkipButton();
                VisibleRemindButton();

                //speaker�� belong�� �ؽ�Ʈ�� �����Ѵ�
                speaker.SetText(currentLineData["Speaker"].ToString());
                belong.SetText(currentLineData["Belong"].ToString());

                //endTime�� Ȯ���ϰ� �帧 ����Ѵ�
                float endTime = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 2.0f;
                dialog.FlowTextWithEndTime(currentLineData["Dialog"].ToString(), endTime);

                //belong�� speaker�� 10 �ڿ� �ֵ��� ���ġ
                belong.transform.localPosition = new Vector3(speaker.transform.localPosition.x + speaker.GetWidth() + 20, belong.transform.localPosition.y, 0);

                //�ٽú��⿡ �� �߰�
                AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                break;
            case "C1":
                //������ 3��
                UnvisibleDialogPanel();
                UnvisibleSystemButton();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleMiddleOne();

                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                break;
            case "C2":
                //������ 3��
                UnvisibleDialogPanel();
                UnvisibleSystemButton();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleMiddleTwo();

                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
                break;
            case "C3":
                //������ 3��
                UnvisibleDialogPanel();
                UnvisibleSystemButton();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleMiddleThree();

                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
                button_Choice2.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_2"].ToString();
                break;
            case "J":
                //�� ����
                currentLine = int.Parse(currentLineData["Jump_0"].ToString());
                LoadLine(int.Parse(currentLineData["Jump_0"].ToString()));
                break;
            case "E":
                //����
                UnvisibleDialogPanel();
                UnvisibleMiddleAll();
                UnvisibleSkipButton();
                button_Remind.transform.localPosition = new Vector3(850, 325);
                SetLayerDefault();
                break;
        }
    }

    protected virtual void SetLayerDefault()//���̾ �������� ȸ��
    {

    }
}
