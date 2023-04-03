using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Chat : MonoBehaviour
{
    [Header("시작 CSV 파일 이름")]
    public string StartingCSVFileName;
    [Header("다음 줄로 넘어갈 수 있는 시간 간격")]
    public float Delay;
    [Header("바로 실행")]
    public bool PlayOnAwake;
    [Header("다시보기 줄")]
    public GameObject pastLine;
    [Header("다시보기 단락")]
    public GameObject pastParagraph;

    //CSV 데이터 저장
    protected List<Dictionary<string, object>> data;
    protected Dictionary<string, object> currentLineData;

    private bool isLastNowFlowText;
    private bool isAbleToMoveNextLine;
    private int currentLine;

    private AudioSource audioSource;
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
    private TextFieldProUGUI dialog;
    private TextFieldProUGUI speaker;

    private void Start() 
    {
        //시작 CSV 파일 데이터 로드
        LoadData(StartingCSVFileName);

        audioSource = transform.GetChild(0).GetComponent<AudioSource>();
        panel_Dialog = transform.GetChild(1).gameObject;        
        panel_Skip = transform.GetChild(2).gameObject;
        panel_Remind = transform.GetChild(3).gameObject;
        panel_Choice = transform.GetChild(4).gameObject;
        button_Skip = transform.GetChild(5).gameObject;
        button_Remind = transform.GetChild(6).gameObject;
        button_Choice0 = transform.GetChild(7).gameObject;
        button_Choice1 = transform.GetChild(8).gameObject;
        button_Choice2 = transform.GetChild(9).gameObject;

        speaker = panel_Dialog.transform.GetChild(2).GetComponent<TextFieldProUGUI>();
        dialog = panel_Dialog.transform.GetChild(3).GetComponent<TextFieldProUGUI>();
        belong = panel_Dialog.transform.GetChild(4).GetComponent<TextFieldProUGUI>();

        //UI요소 비활성화
        panel_Dialog.SetActive(false);
        panel_Choice.SetActive(false);
        panel_Skip.SetActive(false);
        panel_Remind.SetActive(false);
        button_Choice0.SetActive(false);
        button_Choice1.SetActive(false);
        button_Choice2.SetActive(false);
        button_Skip.SetActive(false);
        button_Remind.SetActive(false);

        //바로 실행
        if (PlayOnAwake)
            LoadLine(currentLine);
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

        //대화 판넬 활성 && 다시보기 판넬 비활성 && 건너뛰기 판넬 비활성 && 다른 UI 오브젝트 위 아님 - 키보드 스페이스 입력 확인 시
        bool condition_0 = panel_Dialog.activeSelf && !panel_Remind.activeSelf && !panel_Skip.activeSelf;
        //대화 판넬 활성 && 다시보기 판넬 비활성 && 건너뛰기 판넬 비활성 && 다른 UI 오브젝트 위 아님 - 마우스 왼쪽 버튼 다운 시, 다른 버튼을 누른 것이 아닌지 걸러낼 필요가 있음
        bool condition_1 = panel_Dialog.activeSelf && !panel_Remind.activeSelf && !panel_Skip.activeSelf && !EventSystem.current.IsPointerOverGameObject();

        //조건을 만족하는 입력이 들어옴
        if ((Input.GetKeyDown(KeyCode.Space) && condition_0) || (Input.GetMouseButtonDown(0) && condition_1))
        {
            switch (dialog.GetIsNowFlowText())
            {
                case true://흐름 출력 중이었을 때는 흐름 출력을 종료하고 Dialog를 즉시 채운다 
                    if (data[currentLine - 1]["ChatType"].ToString() == "TD")
                    {
                        dialog.StopCoroutineFlowTextWithDelay();
                        dialog.SetText(data[currentLine - 1]["Dialog"].ToString());
                    }
                    break;
                case false://흐름 출력이 이미 끝나 있었을 때는 다음 줄로 넘어간다
                    bool condition_2 = (data[currentLine - 1]["ChatType"].ToString() == "TD") || (data[currentLine - 1]["ChatType"].ToString() == "TE");
                    bool condition_3 = isAbleToMoveNextLine;
                    if (condition_2 && condition_3)
                        LoadLine(++currentLine);
                    break;
            }
        }
    }

    private void AddLineToRemindPanel(string text_speaker, string text_belong, string text_dialog)//다시보기 판넬에 대화 추가
    {
        GameObject one = Instantiate(pastLine);
        one.GetComponentsInChildren<TextMeshProUGUI>()[0].text = text_belong;
        one.GetComponentsInChildren<TextMeshProUGUI>()[1].text = text_dialog;
        one.GetComponentsInChildren<TextMeshProUGUI>()[2].text = text_speaker;
        Canvas.ForceUpdateCanvases();

        //Speaker의 길이에 따라 Belong위치 조정
        float newPosX = one.GetComponentsInChildren<RectTransform>()[3].transform.localPosition.x + one.GetComponentsInChildren<RectTransform>()[3].rect.width + 20;
        float newPosY = one.GetComponentsInChildren<RectTransform>()[1].transform.localPosition.y;
        one.GetComponentsInChildren<RectTransform>()[1].transform.localPosition = new Vector3(newPosX, newPosY, 0);
        one.transform.SetParent(panel_Remind.transform.Find("Viewport").transform.Find("Content"));
        one.transform.localScale = new Vector3(1,1,1);
    }

    private void AddParagraphToRemindPanel(string paragraph)//다시보기 판넬에 단락 추가
    {
        GameObject one = Instantiate(pastParagraph);
        one.transform.Find("Paragraph").GetComponent<TextMeshProUGUI>().text = paragraph;
        one.transform.SetParent(panel_Remind.transform.Find("Viewport").transform.Find("Content"));
        one.transform.localScale = new Vector3(1,1,1);
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
        if (panel_Remind.activeSelf)
        {
            panel_Remind.SetActive(false);
            button_Remind.SetActive(true);

            if (data[currentLine - 1]["ChatType"].ToString() != "E")
            {
                panel_Dialog.SetActive(true);
                button_Skip.SetActive(true);
            }
        }
        else
        {
            panel_Remind.SetActive(true);
            panel_Dialog.SetActive(false);
            button_Skip.SetActive(false);
            button_Remind.SetActive(false);

            if (panel_Remind.transform.Find("ScrollbarVertical") != null)//스크롤바를 가장 최근 대화로 이동
                panel_Remind.transform.Find("ScrollbarVertical").GetComponent<Scrollbar>().value = 0.01f;
        }
    }

    public void OnSkipDown()//건너뛰기 다운
    {
        if (panel_Skip.activeSelf)
        {
            panel_Dialog.SetActive(true);
            button_Skip.SetActive(true);
            button_Remind.SetActive(false);
            panel_Skip.SetActive(false);
        }
        else
        {
            panel_Dialog.SetActive(false);
            button_Skip.SetActive(false);
            button_Remind.SetActive(false);
            panel_Skip.SetActive(true);
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

        Debug.Log("ExecuteSkipOrder");

        panel_Skip.SetActive(false);
        button_Skip.SetActive(false);
        button_Remind.SetActive(true);

        dialog.StopCoroutineFlowTextWithDelay();

        for(;;tempLine++)
        {
            nextChatLineData = data[tempLine] != null ? data[tempLine] : null;

            //건너뛰기 한계 위치에 도달
            if((nextChatLineData["ChatType"].ToString() == "C2") || (nextChatLineData["ChatType"].ToString() == "C3"))
            {
                LoadLine(tempLine);
                button_Skip.SetActive(false);
                return;
            }

            //단락 유무 확인 및 다시보기에 추가
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
                    SetLayerAtEnd();
                    return;
            }
        } 
    }

    public void LoadData(string TargetingCSVFileName)//새로운 CSV 파일 로드
    {
        data = CSVReader.Read("Assets/Resources/Chat/" + TargetingCSVFileName + ".csv");
        if(data==null)
            Debug.LogWarning("Coundn't Find CSV File '" + TargetingCSVFileName + "'");
        else
            Debug.Log("Load CSV File '" + TargetingCSVFileName + "'");
    }

    public virtual void LoadLine(int line)//CSV파일의 지정한 줄 로드
    {
        //로드된 데이터가 없으면 종료
        if(data == null)
            return;

        currentLine = line;

        //오디오 재생
        if(audioSource.clip!=null)
            audioSource.Play();

        //연속으로 다음 라인으로 넘어가는 것 차단
        isAbleToMoveNextLine = false;

        //현재 라인 데이터 로드
        currentLineData = data[line - 1];

        //단락 유무 확인 및 다시보기에 추가
        if (currentLineData["RemindInstruction"].ToString() != "")
            AddParagraphToRemindPanel(currentLineData["RemindInstruction"].ToString());

        switch (currentLineData["ChatType"].ToString())
        {
            case "TD":
                //Delay에 따라 출력
                panel_Dialog.SetActive(true);
                panel_Choice.SetActive(false);
                panel_Remind.SetActive(false);
                button_Choice0.SetActive(false);
                button_Choice1.SetActive(false);
                button_Choice2.SetActive(false);
                button_Remind.SetActive(true);
                button_Skip.SetActive(true);

                //delay값 결정
                float delay = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 0.03f;
                dialog.FlowTextWithDelay(currentLineData["Dialog"].ToString(), delay);

                //화자와 소속 표시
                speaker.SetText(currentLineData["Speaker"].ToString());
                belong.SetText(currentLineData["Belong"].ToString());
                belong.transform.localPosition = new Vector3(speaker.transform.localPosition.x + speaker.GetWidth() + 20, belong.transform.localPosition.y, 0);
                
                //다시보기 판넬에 줄 추가
                AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                break;
            case "TE":
                //EndTime에 따라 출력
                panel_Dialog.SetActive(true);
                panel_Choice.SetActive(false);
                panel_Remind.SetActive(false);
                button_Choice0.SetActive(false);
                button_Choice1.SetActive(false);
                button_Choice2.SetActive(false);
                button_Remind.SetActive(true);
                button_Skip.SetActive(true);

                //endTime값 결정
                float endTime = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 2.0f;
                dialog.FlowTextWithEndTime(currentLineData["Dialog"].ToString(), endTime);

                //화자와 소속 표시
                speaker.SetText(currentLineData["Speaker"].ToString());
                belong.SetText(currentLineData["Belong"].ToString());
                belong.transform.localPosition = new Vector3(speaker.transform.localPosition.x + speaker.GetWidth() + 20, belong.transform.localPosition.y, 0);

                //다시보기 판넬에 줄 추가
                AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                break;
            case "A":
                panel_Dialog.SetActive(false);
                panel_Choice.SetActive(true);
                button_Choice0.SetActive(true);
                button_Skip.SetActive(false);
                button_Remind.SetActive(false);
                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice0.transform.localPosition = new Vector3(0,0,0);
                break;
            case "C2":
                panel_Dialog.SetActive(false);
                panel_Choice.SetActive(true);
                button_Choice0.SetActive(true);
                button_Choice1.SetActive(true);
                button_Skip.SetActive(false);
                button_Remind.SetActive(false);
                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
                button_Choice0.transform.localPosition = new Vector3(0,75,0);
                button_Choice1.transform.localPosition = new Vector3(0,-75,0);
                break;
            case "C3":
                panel_Dialog.SetActive(false);
                panel_Choice.SetActive(true);
                button_Choice0.SetActive(true);
                button_Choice1.SetActive(true);
                button_Choice2.SetActive(true);
                button_Skip.SetActive(false);
                button_Remind.SetActive(false);
                button_Choice0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
                button_Choice2.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_2"].ToString();
                button_Choice0.transform.localPosition = new Vector3(0,150,0);
                button_Choice1.transform.localPosition = new Vector3(0,0,0);
                button_Choice2.transform.localPosition = new Vector3(0,-150,0);
                break;
            case "J":
                currentLine = int.Parse(currentLineData["Jump_0"].ToString());
                LoadLine(int.Parse(currentLineData["Jump_0"].ToString()));
                break;
            case "E":
                panel_Dialog.SetActive(false);
                panel_Choice.SetActive(false);
                button_Choice0.SetActive(false);
                button_Choice1.SetActive(false);
                button_Choice2.SetActive(false);
                button_Remind.SetActive(true);
                button_Skip.SetActive(false);
                SetLayerAtEnd();
                break;
        }
    }

    public int[] GetArrayOfTutorialPhase()//튜토리얼 페이즈 정보가 담긴 배열을 반환한다
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

    protected virtual void SetLayerAtEnd()//입력 제어
    {
        
    }
}
