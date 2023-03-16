using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    private GameObject button_Skip;
    private GameObject button_Remind;
    private GameObject panel_Choice;
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
        data = CSVReader.Read("Assets/Resources/Chats/" + FileName + ".csv");
        Debug.Log(data.ToString());

        button_Choice0 = GameObject.Find("Button_Choice0");
        button_Choice1 = GameObject.Find("Button_Choice1");
        button_Choice2 = GameObject.Find("Button_Choice2");
        button_Remind = GameObject.Find("Button_Remind");
        button_Skip = GameObject.Find("Button_Skip");
        panel_Choice = GameObject.Find("Panel_Choice");
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
            UnvisibleChoiceAll();
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

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && (!isRemindOpened && !isSkipOpened && !EventSystem.current.IsPointerOverGameObject()))
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
                    if (condition_0 && condition_1)
                        LoadLine(++currentLine);
                    break;
            }
        }
    }

    private void VisibleSkipButton()//��ŵ ��ư Ȱ��ȭ
    {
        button_Skip.transform.localPosition = new Vector3(810, 375);
    }

    private void UnvisibleSkipButton()//��ŵ ��ư ��Ȱ��ȭ
    {
        button_Skip.transform.localPosition = new Vector3(2000, 375);
    }

    private void VisibleRemindButton()//�ٽú��� ��ư Ȱ��ȭ
    {
        button_Remind.transform.localPosition = new Vector3(810, 460);
    }

    private void UnvisibleRemindButton()//�ٽú��� ��ư ��Ȱ��ȭ
    {
        button_Remind.transform.localPosition = new Vector3(2000, 460);
    }

    private void VisibleRemindPanel()//�ٽú��� �г� Ȱ��ȭ
    {
        panel_Remind.transform.localPosition = new Vector3(0, 0);
        isRemindOpened = true;

        UnvisibleDialogPanel();
        UnvisibleSkipButton();
        UnvisibleRemindButton();

        if (panel_Remind.transform.Find("ScrollbarVertical") != null)//��ũ�ѹٿ� Content�� ������ ��ũ�ѹٰ� �������� ���� ���ɼ�
            panel_Remind.transform.Find("ScrollbarVertical").GetComponent<Scrollbar>().value = 0f;  //���� �ֱ��� ��ȭ�� �ٷ� �̵�
    }

    private void UnvisibleRemindPanel()//�ٽú��� �г� ��Ȱ��ȭ
    {
        panel_Remind.transform.localPosition = new Vector3(2000, 0);
        isRemindOpened = false;

        if (data[currentLine - 1]["ChatType"].ToString() != "E")
        {
            VisibleDialogPanel();
            VisibleSkipButton();
        }

        VisibleRemindButton();
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

    private void AddLineToRemindPanel(string text_speaker, string text_belong, string text_dialog)//�ٽú��⿡ �� �߰�
    {
        GameObject one;
        one = Instantiate(pastLine, panel_Remind.transform.Find("Viewport").transform.Find("Content"));
        GameObject speaker = one.transform.Find("Speaker").gameObject;
        GameObject belong = one.transform.Find("Belong").gameObject;
        GameObject dialog = one.transform.Find("Dialog").gameObject;
        speaker.GetComponent<TextMeshProUGUI>().text = text_speaker;
        belong.GetComponent<TextMeshProUGUI>().text = text_belong;
        dialog.GetComponent<TextMeshProUGUI>().text = text_dialog;

        Canvas.ForceUpdateCanvases();

        //Speaker�� ���̸� ������ Belong�� ��ġ ����
        float newPosX = speaker.transform.localPosition.x + speaker.GetComponent<RectTransform>().rect.width + 20;
        float newPosY = belong.transform.localPosition.y;
        belong.transform.localPosition = new Vector3(newPosX, newPosY, 0);
    }

    private void AddParagraphToRemindPanel(string paragraph)//�ٽú��⿡ �ܶ� �߰�
    {
        GameObject one;
        one = Instantiate(pastParagraph, panel_Remind.transform.Find("Viewport").transform.Find("Content"));
        one.transform.Find("Paragraph").GetComponent<TextMeshProUGUI>().text = paragraph;
    }

    private void VisibleChoiceOne()//�߰� ������ �ϳ� Ȱ��ȭ
    {
        panel_Choice.transform.localPosition = new Vector3(0, 0);
        button_Choice0.transform.localPosition = new Vector3(0, 0);
    }

    private void VisibleChoiceTwo()//�߰� ������ 2�� Ȱ��ȭ
    {
        panel_Choice.transform.localPosition = new Vector3(0, 0);
        button_Choice0.transform.localPosition = new Vector3(0, 75);
        button_Choice1.transform.localPosition = new Vector3(0, -75);
    }

    private void VisibleChoiceThree()//�߰� ������ 3�� Ȱ��ȭ
    {
        panel_Choice.transform.localPosition = new Vector3(0, 0);
        button_Choice0.transform.localPosition = new Vector3(0, 150);
        button_Choice1.transform.localPosition = new Vector3(0, 0);
        button_Choice2.transform.localPosition = new Vector3(0, -150);
    }

    private void UnvisibleChoiceAll()//�߰� ������ ���� ��Ȱ��ȭ
    {
        panel_Choice.transform.localPosition = new Vector3(-2000, 0);
        button_Choice0.transform.localPosition = new Vector3(-2000, 150);
        button_Choice1.transform.localPosition = new Vector3(-2000, 0);
        button_Choice2.transform.localPosition = new Vector3(-2000, -150);
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
        Dictionary<string, object> currentLineData = data[currentLine - 1];
        switch (data[currentLine - 1]["ChatType"].ToString())
        {
            case "A":
                currentLine++;
                break;
            case "C2":
                currentLine = int.Parse(currentLineData["Jump_" + choice.ToString()].ToString());
                break;
            case "C3":
                currentLine = int.Parse(currentLineData["Jump_" + choice.ToString()].ToString());
                break;
        }
        AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Choice_" + choice.ToString()].ToString());
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
            VisibleSkipButton();
            VisibleRemindButton();
            UnvisibleSkipPanel();
        }
        else
        {
            UnvisibleDialogPanel();
            UnvisibleSkipButton();
            UnvisibleRemindButton();
            VisibleSkipPanel();
        }
    }

    public void ExecuteSkip()//��ŵ ����
    {
        Debug.Log("ExecuteSkipOrder");

        UnvisibleSkipPanel();
        UnvisibleSkipButton();
        VisibleRemindButton();

        SetLayerDefault();

        dialog.StopCoroutineFlowTextWithDelay();

        for (currentLine += 1; ;currentLine++)
        {
            Dictionary<string, object> currentLineData = data[currentLine - 1];
            Dictionary<string, object> nextChatLineData = data[currentLine] != null ? data[currentLine] : null;
            
            bool condition0 = nextChatLineData["ChatType"].ToString() == "C2";
            bool condition1 = nextChatLineData["ChatType"].ToString() == "C3";
            bool isBeforeChoiceLine = condition0 || condition1;

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
                case "A":
                    AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Choice_0"].ToString());
                    break;
                case "J":
                    currentLine = int.Parse(currentLineData["Jump_0"].ToString());                     
                    break;
                case "E":
                    return;
            }
        } 
    }

    public virtual void LoadLine(int line)//������ ������ �ε�
    {
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
                UnvisibleChoiceAll();
                VisibleDialogPanel();
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
                UnvisibleChoiceAll();
                VisibleDialogPanel();
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
            case "A":
                //������ 3��
                UnvisibleDialogPanel();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleChoiceOne();

                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                break;
            case "C2":
                //������ 3��
                UnvisibleDialogPanel();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleChoiceTwo();

                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
                break;
            case "C3":
                //������ 3��
                UnvisibleDialogPanel();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleChoiceThree();

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
                VisibleRemindButton();
                UnvisibleDialogPanel();
                UnvisibleChoiceAll();
                UnvisibleSkipButton();
                SetLayerDefault();
                break;
        }
    }

    public List<int> GetListOfTutorialPhase()//Ʃ�丮�� �������� ������ ����� ����Ʈ ��ȯ
    {
        List<int> tutorialPhaseList = new List<int>();

        int line = 1;
        foreach(Dictionary<string, object> currentLineData in data)
        {
            if (currentLineData["TutorialPhase"].ToString() != "")
                tutorialPhaseList.Add(line);
            line++;
        }

        return tutorialPhaseList;
    }

    protected virtual void SetLayerDefault()//���̾ �������� ȸ��
    {

    }
}
