using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Chat : MonoBehaviour
{
    [Header("CSV 파일 이름")]
    public string FileName;

    [Header("다음 줄로 넘어갈 수 있는 간격")]
    public float Delay;

    [Header("바로 실행")]
    public bool PlayOnAwake;

    [Header("다시보기 줄")]
    public GameObject pastLine;

    [Header("다시보기 단락")]
    public GameObject pastParagraph;

    //오디오 소스 컴포넌트
    private AudioSource audioSource;

    //CSV 데이터 저장
    protected List<Dictionary<string, object>> data;

    private bool isLastNowFlowText;
    private bool isAbleToMoveNextLine;
    private int currentLine;
    private bool isRemindOpened;
    private bool isSkipOpened;

    private GameObject button_Choice0;
    private GameObject button_Choice1;
    private GameObject button_Choice2;
    private GameObject button_Skip;
    private GameObject button_Remind;
    private GameObject panel_Choice;
    private GameObject panel_Dialog;
    private GameObject panel_Remind;
    private GameObject panel_Skip;
    private TextFieldProUGUI belong;
    private TextFieldProUGUI speaker;
    private TextFieldProUGUI dialog;

    private void Start()
    {
        //CSV 데이터 로드
        data = CSVReader.Read("Assets/Resources/Chats/" + FileName + ".csv");

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

        UnvisibleDialogPanel();
        UnvisibleChoiceAll();
        UnvisibleSkipButton();
        UnvisibleRemindButton();

        Invoke("StartChat", 1f);
    }

    private void Update()
    {
        ControlInput();
    }

    private void ControlInput()//입력 감지
    {
        if (isLastNowFlowText == true && dialog.GetIsNowFlowText() == false)//출력이 끝나면 Delay이 후에 다음으로 넘어갈 수 있게 된다
            Invoke("SetTrueIsAbleToNextLine", Delay);
        isLastNowFlowText = dialog.GetIsNowFlowText();

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && (!isRemindOpened && !isSkipOpened && !EventSystem.current.IsPointerOverGameObject()))
        {
            switch (dialog.GetIsNowFlowText())
            {
                case true://흐름 출력 중이었음
                    if (data[currentLine - 1]["ChatType"].ToString() == "TD")
                    {
                        dialog.StopCoroutineFlowTextWithDelay();
                        dialog.SetText(data[currentLine - 1]["Dialog"].ToString());
                    }
                    break;
                case false://다음으로 넘어간다
                    bool condition_0 = (data[currentLine - 1]["ChatType"].ToString() == "TD") || (data[currentLine - 1]["ChatType"].ToString() == "TE");
                    bool condition_1 = isAbleToMoveNextLine;
                    if (condition_0 && condition_1)
                        LoadLine(++currentLine);
                    break;
            }
        }
    }

    private void StartChat()//Chat를 시작합니다
    {
        if (!PlayOnAwake)
            return;

        currentLine = 1;
        LoadLine(currentLine);
    }

    private void VisibleSkipButton()//건너뛰기 버튼 가시
    {
        button_Skip.transform.localPosition = new Vector3(810, 375);
    }

    private void UnvisibleSkipButton()//건너뛰기 버튼 비가시
    {
        button_Skip.transform.localPosition = new Vector3(2000, 375);
    }

    private void VisibleRemindButton()//다시보기 버튼 가시
    {
        button_Remind.transform.localPosition = new Vector3(810, 460);
    }

    private void UnvisibleRemindButton()//다시보기 버튼 비가시
    {
        button_Remind.transform.localPosition = new Vector3(2000, 460);
    }

    private void VisibleRemindPanel()//다시보기 판넬 가시
    {
        panel_Remind.transform.localPosition = new Vector3(0, 0);
        isRemindOpened = true;
    }

    private void UnvisibleRemindPanel()//다시보기 판넬 비가시
    {
        panel_Remind.transform.localPosition = new Vector3(2000, 0);
        isRemindOpened = false;
    }

    private void VisibleSkipPanel()//건너뛰기 판넬 가시
    {
        panel_Skip.transform.localPosition = new Vector3(0, 0);
        isSkipOpened = true;
    }

    private void UnvisibleSkipPanel()//건너뛰기 판넬 비가시
    {
        panel_Skip.transform.localPosition = new Vector3(4000, 0);
        isSkipOpened = false;
    }

    private void AddLineToRemindPanel(string text_speaker, string text_belong, string text_dialog)//다시보기 판넬에 대화 추가
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

        //Speaker의 길이에 따라 Belong위치 조정
        float newPosX = speaker.transform.localPosition.x + speaker.GetComponent<RectTransform>().rect.width + 20;
        float newPosY = belong.transform.localPosition.y;
        belong.transform.localPosition = new Vector3(newPosX, newPosY, 0);
    }

    private void AddParagraphToRemindPanel(string paragraph)//다시보기 판넬에 단락 추가
    {
        GameObject one;
        one = Instantiate(pastParagraph, panel_Remind.transform.Find("Viewport").transform.Find("Content"));
        one.transform.Find("Paragraph").GetComponent<TextMeshProUGUI>().text = paragraph;
    }

    private void VisibleChoiceOne()//선택지 하나 가시
    {
        panel_Choice.transform.localPosition = new Vector3(0, 0);
        button_Choice0.transform.localPosition = new Vector3(0, 0);
    }

    private void VisibleChoiceTwo()//선택지 둘 가시
    {
        panel_Choice.transform.localPosition = new Vector3(0, 0);
        button_Choice0.transform.localPosition = new Vector3(0, 75);
        button_Choice1.transform.localPosition = new Vector3(0, -75);
    }

    private void VisibleChoiceThree()//선택지 셋 가시
    {
        panel_Choice.transform.localPosition = new Vector3(0, 0);
        button_Choice0.transform.localPosition = new Vector3(0, 150);
        button_Choice1.transform.localPosition = new Vector3(0, 0);
        button_Choice2.transform.localPosition = new Vector3(0, -150);
    }

    private void UnvisibleChoiceAll()//선택지 전부 비가시
    {
        panel_Choice.transform.localPosition = new Vector3(-2000, 0);
        button_Choice0.transform.localPosition = new Vector3(-2000, 150);
        button_Choice1.transform.localPosition = new Vector3(-2000, 0);
        button_Choice2.transform.localPosition = new Vector3(-2000, -150);
    }

    private void VisibleDialogPanel()//대화 판넬 가시
    {
        panel_Dialog.transform.localPosition = new Vector3(0, 0);
    }

    private void UnvisibleDialogPanel()//대화 판넬 비가시
    {
        panel_Dialog.transform.localPosition = new Vector3(2000, 0);
    }

    private void SetTrueIsAbleToNextLine()//Invoke 용
    {
        isAbleToMoveNextLine = true;
    }

    public void OnChoiceDown(int choice)//선택지 다운
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

    public void OnRemindDown()//다시보기 다운
    {
        if (isRemindOpened)
        {
            UnvisibleRemindPanel();
            if (data[currentLine - 1]["ChatType"].ToString() != "E")
            {
                VisibleDialogPanel();
                VisibleSkipButton();
            }
            VisibleRemindButton();
        }
        else
        {
            VisibleRemindPanel();
            UnvisibleDialogPanel();
            UnvisibleSkipButton();
            UnvisibleRemindButton();

            if (panel_Remind.transform.Find("ScrollbarVertical") != null)//스크롤바를 가장 최근 대화로 이동
                panel_Remind.transform.Find("ScrollbarVertical").GetComponent<Scrollbar>().value = 0f;
        }
    }

    public void OnSkipDown()//건너뛰기 다운
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

    public void ExecuteSkip()//건너뛰기 실행
    {
        int tempLine = currentLine;
        Dictionary<string, object> nextChatLineData = data[tempLine] != null ? data[tempLine] : null;

        //이미 건너뛰기 한계 위치에 있음
        if((nextChatLineData["ChatType"].ToString() == "C2") || (nextChatLineData["ChatType"].ToString() == "C3"))
        {
            Debug.Log("이미 건너뛰기 한계 위치이므로 건너뛰기 명령 수행 불가");
            return;
        }

        Debug.Log("ExecuteSkipOrder!");

        UnvisibleSkipPanel();
        UnvisibleSkipButton();
        VisibleRemindButton();

        SetLayerDefault();

        dialog.StopCoroutineFlowTextWithDelay();

        for(;;tempLine++)
        {
            nextChatLineData = data[tempLine] != null ? data[tempLine] : null;

            //건너뛰기 한계 위치에 도달
            if((nextChatLineData["ChatType"].ToString() == "C2") || (nextChatLineData["ChatType"].ToString() == "C3"))
            {
                LoadLine(tempLine);
                UnvisibleSkipButton();
                return;
            }

            //단락 추가
            if (nextChatLineData["RemindInstruction"].ToString() != "")
                AddParagraphToRemindPanel(nextChatLineData["RemindInstruction"].ToString());

            switch (nextChatLineData["ChatType"].ToString())
            {
                case "TD":
                    AddLineToRemindPanel(nextChatLineData["Speaker"].ToString(), nextChatLineData["Belong"].ToString(), nextChatLineData["Dialog"].ToString());
                    break;
                case "TE":
                    AddLineToRemindPanel(nextChatLineData["Speaker"].ToString(), nextChatLineData["Belong"].ToString(), nextChatLineData["Dialog"].ToString());
                    break;
                case "A":
                    AddLineToRemindPanel(nextChatLineData["Speaker"].ToString(), nextChatLineData["Belong"].ToString(), nextChatLineData["Choice_0"].ToString());
                    break;
                case "J":
                    tempLine = int.Parse(nextChatLineData["Jump_0"].ToString());                     
                    break;
                case "E":
                    currentLine = tempLine + 1;
                    return;
            }
        } 
    }

    public virtual void LoadLine(int line)//지정한 라인 로드
    {
        currentLine = line;

        //오디오 재생
        audioSource.Play();

        //연속으로 다음 라인으로 넘어가는 것 차단
        isAbleToMoveNextLine = false;

        //현재 라인 데이터 로드
        Dictionary<string, object> currentLineData = data[line - 1];

        //단락 확인 및 다시보기에 추가
        if (currentLineData["RemindInstruction"].ToString() != "")
            AddParagraphToRemindPanel(currentLineData["RemindInstruction"].ToString());

        switch (currentLineData["ChatType"].ToString())
        {
            case "TD":
                //Delay에 따라 출력
                UnvisibleChoiceAll();
                UnvisibleRemindPanel();
                VisibleDialogPanel();
                VisibleRemindButton();
                VisibleSkipButton();

                speaker.SetText(currentLineData["Speaker"].ToString());
                belong.SetText(currentLineData["Belong"].ToString());
                
                //delay값 결정
                float delay = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 0.03f;
                dialog.FlowTextWithDelay(currentLineData["Dialog"].ToString(), delay);
                
                //speaker의 길이에 따라 belong위치 조정
                belong.transform.localPosition = new Vector3(speaker.transform.localPosition.x + speaker.GetWidth() + 20, belong.transform.localPosition.y, 0);

                //다시보기 판넬에 라인 추가
                AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                break;
            case "TE":
                //EndTime에 따라 출력
                UnvisibleChoiceAll();
                UnvisibleRemindPanel();
                VisibleDialogPanel();
                VisibleRemindButton();
                VisibleSkipButton();

                speaker.SetText(currentLineData["Speaker"].ToString());
                belong.SetText(currentLineData["Belong"].ToString());

                //endTime값 결정
                float endTime = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 2.0f;
                dialog.FlowTextWithEndTime(currentLineData["Dialog"].ToString(), endTime);

                //speaker의 길이에 따라 belong위치 조정
                belong.transform.localPosition = new Vector3(speaker.transform.localPosition.x + speaker.GetWidth() + 20, belong.transform.localPosition.y, 0);

                //다시보기 판넬에 라인 추가
                AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                break;
            case "A":
                UnvisibleDialogPanel();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleChoiceOne();

                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                break;
            case "C2":
                UnvisibleDialogPanel();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleChoiceTwo();

                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
                break;
            case "C3":
                UnvisibleDialogPanel();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleChoiceThree();

                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
                button_Choice2.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_2"].ToString();
                break;
            case "J":
                currentLine = int.Parse(currentLineData["Jump_0"].ToString());
                LoadLine(int.Parse(currentLineData["Jump_0"].ToString()));
                break;
            case "E":
                VisibleRemindButton();
                UnvisibleDialogPanel();
                UnvisibleChoiceAll();
                UnvisibleSkipButton();
                SetLayerDefault();
                break;
        }
    }

    public int[] GetListOfTutorialPhase()//튜토리얼 리스트를 반환한다
    {
        List<int> tutorialPhaseList = new List<int>();

        int line = 1;
        foreach(Dictionary<string, object> currentLineData in data)
        {
            if (currentLineData["TutorialPhase"].ToString() != "")
                tutorialPhaseList.Add(line);
            line++;
        }

        return tutorialPhaseList.ToArray();
    }

    protected virtual void SetLayerDefault()//입력 제어
    {

    }
}
