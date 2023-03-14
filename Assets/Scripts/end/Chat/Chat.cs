using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Chat : MonoBehaviour
{
    [Header("CSV 파일 이름")]
    public string FileName;

    [Header("다음 라인 호출 대기 시간")]
    public float Delay;

    [Header("바로 시작 여부")]
    public bool PlayOnAwake;

    [Header("다시보기 줄")]
    public GameObject pastLine;

    [Header("다시보기 단락")]
    public GameObject pastParagraph;

    //오디오 소스
    private AudioSource audioSource;

    //CSV 데이터
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

        if (PlayOnAwake)//바로 대화창을 열고 첫 라인을 로드
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

    private void ControlInput()//입력 감지
    {
        if (isLastNowFlowText == true && dialog.GetIsNowFlowText() == false)//흐름 출력이 종료되었다면 delay 시간 후에 막혀있던 다음 라인 출력이 가능해진다
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
                case false://흐름 출력 중이 아니었음
                    bool condition_0 = (data[currentLine - 1]["ChatType"].ToString() == "TD") || (data[currentLine - 1]["ChatType"].ToString() == "TE");
                    bool condition_1 = isAbleToMoveNextLine;
                    if (condition_0 && condition_1)
                        LoadLine(++currentLine);
                    break;
            }
        }
    }

    private void VisibleSkipButton()//스킵 버튼 활성화
    {
        button_Skip.transform.localPosition = new Vector3(810, 375);
    }

    private void UnvisibleSkipButton()//스킵 버튼 비활성화
    {
        button_Skip.transform.localPosition = new Vector3(2000, 375);
    }

    private void VisibleRemindButton()//다시보기 버튼 활성화
    {
        button_Remind.transform.localPosition = new Vector3(810, 460);
    }

    private void UnvisibleRemindButton()//다시보기 버튼 비활성화
    {
        button_Remind.transform.localPosition = new Vector3(2000, 460);
    }

    private void VisibleRemindPanel()//다시보기 패널 활성화
    {
        panel_Remind.transform.localPosition = new Vector3(0, 0);
        isRemindOpened = true;

        UnvisibleDialogPanel();
        UnvisibleSkipButton();
        UnvisibleRemindButton();

        if (panel_Remind.transform.Find("ScrollbarVertical") != null)//스크롤바에 Content가 부족해 스크롤바가 존재하지 않을 가능성
            panel_Remind.transform.Find("ScrollbarVertical").GetComponent<Scrollbar>().value = 0f;  //가장 최근의 대화로 바로 이동
    }

    private void UnvisibleRemindPanel()//다시보기 패널 비활성화
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

    private void VisibleSkipPanel()//스킵 패널 활성화
    {
        panel_Skip.transform.localPosition = new Vector3(0, 0);
        isSkipOpened = true;
    }

    private void UnvisibleSkipPanel()//스킵 패널 비활성화
    {
        panel_Skip.transform.localPosition = new Vector3(4000, 0);
        isSkipOpened = false;
    }

    private void AddLineToRemindPanel(string text_speaker, string text_belong, string text_dialog)//다시보기에 줄 추가
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

        //Speaker의 길이를 고려해 Belong의 위치 조정
        float newPosX = speaker.transform.localPosition.x + speaker.GetComponent<RectTransform>().rect.width + 20;
        float newPosY = belong.transform.localPosition.y;
        belong.transform.localPosition = new Vector3(newPosX, newPosY, 0);
    }

    private void AddParagraphToRemindPanel(string paragraph)//다시보기에 단락 추가
    {
        GameObject one;
        one = Instantiate(pastParagraph, panel_Remind.transform.Find("Viewport").transform.Find("Content"));
        one.transform.Find("Paragraph").GetComponent<TextMeshProUGUI>().text = paragraph;
    }

    private void VisibleChoiceOne()//중간 선택지 하나 활성화
    {
        panel_Choice.transform.localPosition = new Vector3(0, 0);
        button_Choice0.transform.localPosition = new Vector3(0, 0);
    }

    private void VisibleChoiceTwo()//중간 선택지 2개 활성화
    {
        panel_Choice.transform.localPosition = new Vector3(0, 0);
        button_Choice0.transform.localPosition = new Vector3(0, 75);
        button_Choice1.transform.localPosition = new Vector3(0, -75);
    }

    private void VisibleChoiceThree()//중간 선택지 3개 활성화
    {
        panel_Choice.transform.localPosition = new Vector3(0, 0);
        button_Choice0.transform.localPosition = new Vector3(0, 150);
        button_Choice1.transform.localPosition = new Vector3(0, 0);
        button_Choice2.transform.localPosition = new Vector3(0, -150);
    }

    private void UnvisibleChoiceAll()//중간 선택지 전부 비활성화
    {
        panel_Choice.transform.localPosition = new Vector3(-2000, 0);
        button_Choice0.transform.localPosition = new Vector3(-2000, 150);
        button_Choice1.transform.localPosition = new Vector3(-2000, 0);
        button_Choice2.transform.localPosition = new Vector3(-2000, -150);
    }

    private void VisibleDialogPanel()//밑 활성화
    {
        panel_Dialog.transform.localPosition = new Vector3(0, 0);
    }

    private void UnvisibleDialogPanel()//밑 비활성화
    {
        panel_Dialog.transform.localPosition = new Vector3(2000, 0);
    }

    private void SetTrueIsAbleToNextLine()//다음 라인 출력 가능, Invoke용
    {
        isAbleToMoveNextLine = true;
    }

    public void OnChoiceDown(int choice)//선택지가 클릭
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

    public void OnRemindDown()//다시보기 클릭
    {
        if (isRemindOpened)
            UnvisibleRemindPanel();
        else
            VisibleRemindPanel();
    }

    public void OnSkipDown()//스킵 클릭
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

    public void ExecuteSkip()//스킵 실행
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

    public virtual void LoadLine(int line)//지정된 라인을 로드
    {
        //현재 줄 업데이트
        currentLine = line;

        //사운드 재생
        audioSource.Play();

        //연속으로 넘어가지 않도록 차단
        isAbleToMoveNextLine = false;

        //현재 줄의 데이터 로드
        Dictionary<string, object> currentLineData = data[line - 1];

        //단락 내용을 다시보기에 추가
        if (currentLineData["RemindInstruction"].ToString() != "")
            AddParagraphToRemindPanel(currentLineData["RemindInstruction"].ToString());

        switch (currentLineData["ChatType"].ToString())
        {
            case "TD":
                //Delay에 따른 텍스트 출력
                UnvisibleChoiceAll();
                VisibleDialogPanel();
                VisibleSkipButton();
                VisibleRemindButton();

                //speaker와 belong의 텍스트를 설정한다
                speaker.SetText(currentLineData["Speaker"].ToString());
                belong.SetText(currentLineData["Belong"].ToString());
                
                //delay를 확인하고 흐름 출력한다
                float delay = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 0.03f;
                dialog.FlowTextWithDelay(currentLineData["Dialog"].ToString(), delay);
                
                //belong을 speaker의 20 뒤에 있도록 재배치
                belong.transform.localPosition = new Vector3(speaker.transform.localPosition.x + speaker.GetWidth() + 20, belong.transform.localPosition.y, 0);

                //다시보기에 줄 추가
                AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                break;
            case "TE":
                //EndTime에 따른 텍스트 출력
                UnvisibleChoiceAll();
                VisibleDialogPanel();
                VisibleSkipButton();
                VisibleRemindButton();

                //speaker와 belong의 텍스트를 설정한다
                speaker.SetText(currentLineData["Speaker"].ToString());
                belong.SetText(currentLineData["Belong"].ToString());

                //endTime을 확인하고 흐름 출력한다
                float endTime = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 2.0f;
                dialog.FlowTextWithEndTime(currentLineData["Dialog"].ToString(), endTime);

                //belong을 speaker의 10 뒤에 있도록 재배치
                belong.transform.localPosition = new Vector3(speaker.transform.localPosition.x + speaker.GetWidth() + 20, belong.transform.localPosition.y, 0);

                //다시보기에 줄 추가
                AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                break;
            case "A":
                //선택지 3개
                UnvisibleDialogPanel();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleChoiceOne();

                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                break;
            case "C2":
                //선택지 3개
                UnvisibleDialogPanel();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleChoiceTwo();

                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
                break;
            case "C3":
                //선택지 3개
                UnvisibleDialogPanel();
                UnvisibleSkipButton();
                UnvisibleRemindButton();
                VisibleChoiceThree();

                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
                button_Choice2.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_2"].ToString();
                break;
            case "J":
                //줄 점프
                currentLine = int.Parse(currentLineData["Jump_0"].ToString());
                LoadLine(int.Parse(currentLineData["Jump_0"].ToString()));
                break;
            case "E":
                //종료
                VisibleRemindButton();
                UnvisibleDialogPanel();
                UnvisibleChoiceAll();
                UnvisibleSkipButton();
                SetLayerDefault();
                break;
        }
    }

    public List<int> GetListOfTutorialPhase()//튜토리얼 페이즈의 라인이 저장된 리스트 반환
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

    protected virtual void SetLayerDefault()//레이어를 보통으로 회귀
    {

    }
}
