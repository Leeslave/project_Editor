using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Chat : MonoBehaviour
{
    [Header("시작 CSV 파일 이름")]
    public string StartingCSVFileName;
    [Header("다음 줄을 로드할 수 있는 시간 간격")]
    public float Delay;
    [Header("바로 실행 여부")]
    public bool PlayOnAwake;
    [Header("튜토리얼 실행 여부")]
    // <summary> 체크되어 있으면 바로 실행 여부와 관계없이 바로 실행
    public bool PlayAsTutorial;
    [Header("다시보기 줄 프리펩")]
    public GameObject pastLine;
    [Header("다시보기 단락 프리펩")]
    public GameObject pastParagraph;

    /// <summary> CSV 데이터 </summary>
    protected List<Dictionary<string, object>> data;
    /// <summary> 현재 줄 데이터 </summary>
    protected Dictionary<string, object> currentLineData;
    /// <summary> 현재 줄 번호 </summary>
    private int currentLineNumber;
    /// <summary> 흐름 출력 상태 </summary>
    private bool lastFlowTextStatus;
    /// <summary> 다음 줄 로드 가능 여부 </summary>
    private bool isAbleToMoveNextLine;
    /// <summary> 튜토리얼 단계 배열 </summary>
    private int[] tutorialPhaseArray;
    /// <summary> 현재 튜토리얼 단계 </summary>
    private int currentTutorialPhase;

    private AudioSource audioSource;
    private GameObject button_Choice_0;
    private GameObject button_Choice_1;
    private GameObject button_Choice_2;
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

        //각 UI요소 접근자
        audioSource = transform.GetChild(0).GetComponent<AudioSource>();
        panel_Dialog = transform.GetChild(1).gameObject;        
        panel_Skip = transform.GetChild(2).gameObject;
        panel_Remind = transform.GetChild(3).gameObject;
        panel_Choice = transform.GetChild(4).gameObject;
        button_Skip = transform.GetChild(5).gameObject;
        button_Remind = transform.GetChild(6).gameObject;
        button_Choice_0 = transform.GetChild(7).gameObject;
        button_Choice_1 = transform.GetChild(8).gameObject;
        button_Choice_2 = transform.GetChild(9).gameObject;
        speaker = panel_Dialog.transform.GetChild(2).GetComponent<TextFieldProUGUI>();
        dialog = panel_Dialog.transform.GetChild(3).GetComponent<TextFieldProUGUI>();
        belong = panel_Dialog.transform.GetChild(4).GetComponent<TextFieldProUGUI>();

        //각 UI요소 비활성화
        panel_Dialog.SetActive(false);
        panel_Choice.SetActive(false);
        panel_Skip.SetActive(false);
        panel_Remind.SetActive(false);
        button_Choice_0.SetActive(false);
        button_Choice_1.SetActive(false);
        button_Choice_2.SetActive(false);
        button_Skip.SetActive(false);
        button_Remind.SetActive(false);

        //튜토리얼 실행
        if (PlayAsTutorial)
        {
            tutorialPhaseArray = GetArrayOfTutorialPhase();
            currentTutorialPhase = -1;
            MoveToNextTutorialPhase(0f);
            return;
        }

        //바로 실행
        if (PlayOnAwake)
        {
            LoadLine(1);
            return;
        }
    }

    private void Update()
    {
        ControlInput();
    }

    ///<summary> 스페이스 입력 또는 마우스 왼클릭을 감지하여 다음 줄 로드 명령을 수신한다 </summary>
    private void ControlInput()
    {
        //바쁜 대기로 대화창의 흐름 출력이 끝났는지 확인한다
        if (lastFlowTextStatus == true && dialog.GetIsNowFlowText() == false)
        {
            //출력이 끝나면 Delay 이후에 다음으로 넘어갈 수 있게 된다
            Invoke("SetTrueIsAbleToNextLine", Delay);
        }
        //바쁜 대기를 위해서 업데이트
        lastFlowTextStatus = dialog.GetIsNowFlowText();

        //대화 판넬 활성 && 다시보기 판넬 비활성 && 건너뛰기 판넬 비활성 && 다른 UI 오브젝트 위 아님
        //키보드 스페이스 입력 확인 시 사용한다.
        bool condition_0 = panel_Dialog.activeSelf && !panel_Remind.activeSelf && !panel_Skip.activeSelf;

        //대화 판넬 활성 && 다시보기 판넬 비활성 && 건너뛰기 판넬 비활성 && 다른 UI 오브젝트 위 아님
        //마우스 왼쪽 버튼 다운 확인 시 사용한다. 키보드 스페이스 입력과는 다르게, 다른 UI 버튼을 누른 것이 아닌지 걸러낼 필요가 있음
        bool condition_1 = panel_Dialog.activeSelf && !panel_Remind.activeSelf && !panel_Skip.activeSelf && !EventSystem.current.IsPointerOverGameObject();

        //조건을 만족하는 입력이 들어오지 않았다면 리턴
        if (!(Input.GetKeyDown(KeyCode.Space) && condition_0) && !(Input.GetMouseButtonDown(0) && condition_1))
            return;

        //흐름 출력 여부를 확인
        if(dialog.GetIsNowFlowText())   
        {
            //흐름 출력을 종료하고 Dialog를 즉시 채운다, 그런데 TE 흐름 출력은 즉시 못 채운다
            if (data[currentLineNumber - 1]["ChatType"].ToString() == "TD")
            {
                dialog.StopCoroutineFlowTextWithDelay();
                dialog.SetText(data[currentLineNumber - 1]["Dialog"].ToString());
            }
        }
        else
        {
            //지정 시간 Delay가 지나 다음 줄을 로드 가능한지 확인한다
            bool condition_2 = (data[currentLineNumber - 1]["ChatType"].ToString() == "TD") || (data[currentLineNumber - 1]["ChatType"].ToString() == "TE");
            bool condition_3 = isAbleToMoveNextLine;
            if (condition_2 && condition_3)
                LoadLine(++currentLineNumber);
        }
    }

    ///<summary> Invoke용으로, 지정 시간 Delay 이후에 다음 줄을 로드 가능하게 만들어준다 </summary>
    private void SetTrueIsAbleToNextLine()//Invoke 용
    {
        isAbleToMoveNextLine = true;
    }

    ///<summary> 다시보기 창에 줄 정보를 추가한다 </summary>
    ///<param name="speaker"> 추가하고자 하는 발화자 정보 </param>
    ///<param name="belong"> 추가하고자 하는 소속 정보 </param>
    ///<param name="dialog"> 추가하고자 하는 대화 정보 </param>
    private void AddLineToRemindPanel(string speaker, string belong, string dialog)//다시보기 창에 대화 추가
    {
        //줄 프리펩 생성
        GameObject one = Instantiate(pastLine);
        //줄 프리펩의 텍스트 업데이트
        one.GetComponentsInChildren<TextMeshProUGUI>()[0].text = belong;
        one.GetComponentsInChildren<TextMeshProUGUI>()[1].text = dialog;
        one.GetComponentsInChildren<TextMeshProUGUI>()[2].text = speaker;
        //캔버스 강제 업데이트
        Canvas.ForceUpdateCanvases();
        //Speaker의 길이에 따라 Belong위치 조정
        float newPosX = one.GetComponentsInChildren<RectTransform>()[3].transform.localPosition.x + one.GetComponentsInChildren<RectTransform>()[3].rect.width + 20;
        float newPosY = one.GetComponentsInChildren<RectTransform>()[1].transform.localPosition.y;
        one.GetComponentsInChildren<RectTransform>()[1].transform.localPosition = new Vector3(newPosX, newPosY, 0);
        //줄 프리펩을 다시보기 창 스크롤 영역에 추가
        one.transform.SetParent(panel_Remind.transform.Find("Viewport").transform.Find("Content"));
        //아무튼 필요했나 봄
        one.transform.localScale = new Vector3(1,1,1);
    }

    ///<summary> 다시보기 창에 단락 정보를 추가한다 </summary>
    ///<param name="paragraph"> 추가하고자 하는 단락 정보 </param>
    private void AddParagraphToRemindPanel(string paragraph)
    {
        //단락 프리펩 생성
        GameObject one = Instantiate(pastParagraph);
        //단락 프리펩의 텍스트 업데이트
        one.GetComponentsInChildren<TextMeshProUGUI>()[0].text = paragraph;
        //단락 프리펩을 다시보기 창 스크롤 영역에 추가
        one.transform.SetParent(panel_Remind.transform.Find("Viewport").transform.Find("Content"));
        //아무튼 필요했나 봄
        one.transform.localScale = new Vector3(1,1,1);
    }

    ///<summary> 선택지 버튼이 눌리는 이벤트가 발생했다 </summary>
    ///<param name="choiceNumber"> 몇번 선택지 버튼이 눌렸는가? </param>
    ///<remarks> 이 함수는 Chat 스크립트에서는 호출하지 않는다. ChatButton_Choice 스크립트에서 선택지 버튼이 눌리는 이벤트가 발생했을 때, 선택지 번호를 매개변수로 호출된다 </remarks>
    public void OnChoiceDown(int choiceNumber)//선택지 버튼 다운
    {
        Dictionary<string, object> currentLineData = data[currentLineNumber - 1];
        switch (data[currentLineNumber - 1]["ChatType"].ToString())
        {
            case "A":
                currentLineNumber++;
                break;
            case "C2":
                currentLineNumber = int.Parse(currentLineData["Jump_" + choiceNumber.ToString()].ToString());
                break;
            case "C3":
                currentLineNumber = int.Parse(currentLineData["Jump_" + choiceNumber.ToString()].ToString());
                break;
        }
        AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Choice_" + choiceNumber.ToString()].ToString());
        LoadLine(currentLineNumber);
    }

    /// <summary> 다시보기 버튼이 눌리는 이벤트가 발생했다 </summary>
    /// <remarks> 이 함수는 Chat 스크립트에서는 호출하지 않는다. ChatButton_Remind 스크립트에서 다시보기 관련 버튼이 눌리는 이벤트가 발생했을 때 호출된다 </remarks>
    public void OnRemindDown()
    {
        if (panel_Remind.activeSelf)    //다시보기 창이 활성되어 있는 상태, 비활성화한다
        {
            if (data[currentLineNumber - 1]["ChatType"].ToString() == "E")  //채팅창이 미사용 중이니 다시보기 버튼만 되돌리면 된다
            {
                panel_Remind.SetActive(false);
                button_Remind.SetActive(true);
            }
            else                                                            //채팅창이 사용 중이니 건너뛰기 버튼과 대화 창도 되돌려야 함
            {
                panel_Remind.SetActive(false);
                panel_Dialog.SetActive(true);
                button_Remind.SetActive(true);
                button_Skip.SetActive(true);
            }
        }
        else                            //다시보기 창이 비활성되어 있는 상태, 활성화한다
        {
            panel_Remind.SetActive(true);
            panel_Dialog.SetActive(false);
            button_Skip.SetActive(false);
            button_Remind.SetActive(false);

            if (panel_Remind.transform.Find("ScrollbarVertical") != null)//스크롤바를 가장 최근 대화로 이동
                panel_Remind.transform.Find("ScrollbarVertical").GetComponent<Scrollbar>().value = 0.01f;
        }
    }

    /// <summary> 스킵 버튼이 눌리는 이벤트가 발생했다 </summary>
    /// <remarks> 이 함수는 Chat 스크립트에서는 호출하지 않는다. ChatButton_Skip 스크립트에서 건너뛰기 관련 버튼이 눌리는 이벤트가 발생했을 때 호출된다 </remarks>
    public void OnSkipDown()
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

    /// <summary> 채팅 타입 End를 만나거나 채팅 타입 C2, C3를 만나기 전 줄까지 대화를 건너뛴다 </summary>
    /// <remarks> 이 함수는 Chat 스크립트에서는 호출하지 않는다. ChatButton_ExecuteSkip 스크립트에서 건너뛰기 실행 관련 버튼이 눌리는 이벤트가 발생했을 때 호출된다 </remarks>
    public void ExecuteSkip()
    {
        //건너뛰기 동안 일시적으로 사용할 tempLineNumber 선언
        int tempLineNumber = currentLineNumber;
        Dictionary<string, object> tempLineData = data[tempLineNumber];
        if(tempLineData == null)
        {
            Debug.LogError("다음 줄 데이터를 불러오지 못함!");
            return;
        }

        //현재 건너뛰기 할 수 없는 위치에 있음 -> LoadLine에서 자동적으로 건너뛰기 버튼을 못 누르게 처리해주지만, 이 코드는 안전 장치 성격으로 남겨놓았다
        if((tempLineData["ChatType"].ToString() == "C2") || (tempLineData["ChatType"].ToString() == "C3"))
        {
            Debug.LogError("이미 건너뛰기 한계 위치이므로 건너뛰기 명령 수행 불가!");
            return;
        }

        //건너뛰기가 정상적으로 실행었음을 알린다
        Debug.Log("건너뛰기를 실행합니다!");

        panel_Skip.SetActive(false);
        button_Skip.SetActive(false);
        button_Remind.SetActive(true);

        //현재 흐름 출력 중인 텍스트을 강제 중지
        dialog.StopCoroutineFlowTextWithDelay();

        //건너뛰기가 중지되는 조건은 오직 채팅 타입 End에 도달하거나 다음 줄의 채팅타입이 C2, C3인 경우이다
        for(;;tempLineNumber++)
        {
            tempLineData = data[tempLineNumber];

            //다음 줄의 채팅 타입이 C2, C3라면 건너뛰기를 중단하고 채팅창에 C2, C3 직전의 대화를 출력
            if((tempLineData["ChatType"].ToString() == "C2") || (tempLineData["ChatType"].ToString() == "C3"))
            {
                //헷갈릴 수도 있지만 tempLineNumber 그대로 넣는 것이 맞다
                LoadLine(tempLineNumber);
                //어차피 tempLineNumber의 대화에서는 바로 다시 건너뛰기 할 수 없으므로, 건너뛰기 버튼을 일시적으로 비활성화
                button_Skip.SetActive(false);
                return;
            }

            //단락 유무 확인 및 다시보기에 추가
            if (tempLineData["RemindInstruction"].ToString() != "")
                AddParagraphToRemindPanel(tempLineData["RemindInstruction"].ToString());

            //현재 줄의 채팅 타입에 따라 다른 작업 수행
            switch (tempLineData["ChatType"].ToString())
            {
                case "TD":
                    //일반적인 대화이므로 다시보기 창에 추가
                    AddLineToRemindPanel(tempLineData["Speaker"].ToString(), tempLineData["Belong"].ToString(), tempLineData["Dialog"].ToString());
                    break;
                case "TE":
                    //일반적인 대화이므로 다시보기 창에 추가
                    AddLineToRemindPanel(tempLineData["Speaker"].ToString(), tempLineData["Belong"].ToString(), tempLineData["Dialog"].ToString());
                    break;
                case "A":
                    //대답형 연출이고, 플레이어가 알아야 하므로 다시보기 창에 추가
                    AddLineToRemindPanel(tempLineData["Speaker"].ToString(), tempLineData["Belong"].ToString(), tempLineData["Choice_0"].ToString());
                    break;
                case "J":
                    //점프할 줄의 번호를 파싱하여 순회용 tempLineNumber을 업데이트
                    tempLineNumber = int.Parse(tempLineData["Jump_0"].ToString());                     
                    break;
                case "E":
                    //건너뛰기를 모두 수행했으므로 currentLineNumber를 업데이트 하고 리턴
                    currentLineNumber = tempLineNumber + 1;
                    SetLayerAtEnd();
                    return;
            }
        } 
    }

    /// <summary> Assets/Resources/Chat에서 새로운 CSV 파일을 data에 로드한다 </summary>
    /// <param name="CSVFileName"> 새롭게 로드할 파일의 이름 </param>
    public void LoadData(string CSVFileName)
    {
        data = CSVReader.Read("Assets/Resources/Chat/" + CSVFileName + ".csv");
        if(data==null)
            Debug.LogWarning("Coundn't Find CSV File '" + CSVFileName + "'");
        else
            Debug.Log("Load CSV File '" + CSVFileName + "'");
    }

    /// <summary> data의 특정 줄로 이동하여 채팅 타입에 따라 여러 작업을 수행한다 </summary>
    /// <param name="line"> 작업을 수행하고 싶은 특정 줄 번호 </param> 
    public virtual void LoadLine(int line)
    {
        //로드된 데이터가 없으면 오류 발생
        if(data == null)
        {
            Debug.LogError("로드되어있는 CSV 파일이 없습니다!");
            return;
        }

        //currentLineNumber의 업데이트는 ExecuteSkip, OnChoiceDown 함수의 실행을 위해서 필요
        currentLineNumber = line;

        //현재 줄 데이터를 업데이트
        currentLineData = data[currentLineNumber - 1];

        //오디오 재생
        if(audioSource.clip!=null)
            audioSource.Play();

        //연속으로 다음 라인으로 넘어가는 것 차단
        isAbleToMoveNextLine = false;

        //현재 줄의 단락 정보 유무를 확인, 의미있는 정보가 있다면 다시보기 창에 추가
        if (currentLineData["RemindInstruction"].ToString() != "")
            AddParagraphToRemindPanel(currentLineData["RemindInstruction"].ToString());

        //만약 다음 줄의 채팅 타입이 C2, C3라면 건너뛰기를 실행할 수 없도록 막아야 한다
        if((data[currentLineNumber]["ChatType"].ToString() == "C2")||(data[currentLineNumber]["ChatType"].ToString() == "C3"))
            button_Skip.SetActive(false);

        //현재 줄의 채팅 타입에 따라서 다른 작업을 수행
        switch (currentLineData["ChatType"].ToString())
        {
            case "TD":
                //Delay에 따라 대화 출력 작업 수행
                panel_Dialog.SetActive(true);
                panel_Choice.SetActive(false);
                panel_Remind.SetActive(false);
                button_Choice_0.SetActive(false);
                button_Choice_1.SetActive(false);
                button_Choice_2.SetActive(false);
                button_Remind.SetActive(true);
                button_Skip.SetActive(true);

                //delay값 결정
                float delay = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 0.03f;
                dialog.FlowTextWithDelay(currentLineData["Dialog"].ToString(), delay);

                //화자와 소속 텍스트 업데이트
                speaker.SetText(currentLineData["Speaker"].ToString());
                belong.SetText(currentLineData["Belong"].ToString());
                belong.transform.localPosition = new Vector3(speaker.transform.localPosition.x + speaker.GetWidth() + 20, belong.transform.localPosition.y, 0);
                
                //다시보기 창에 이번 대화 내용을 추가
                AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                break;
            case "TE":
                //EndTime에 따라 대화 출력 작업 수행
                panel_Dialog.SetActive(true);
                panel_Choice.SetActive(false);
                panel_Remind.SetActive(false);
                button_Choice_0.SetActive(false);
                button_Choice_1.SetActive(false);
                button_Choice_2.SetActive(false);
                button_Remind.SetActive(true);
                button_Skip.SetActive(true);

                //endTime값 결정
                float endTime = currentLineData["Time"].ToString() != "" ? float.Parse(currentLineData["Time"].ToString()) : 2.0f;
                dialog.FlowTextWithEndTime(currentLineData["Dialog"].ToString(), endTime);

                //화자와 소속 텍스트 업데이트
                speaker.SetText(currentLineData["Speaker"].ToString());
                belong.SetText(currentLineData["Belong"].ToString());
                belong.transform.localPosition = new Vector3(speaker.transform.localPosition.x + speaker.GetWidth() + 20, belong.transform.localPosition.y, 0);

                //다시보기 창에 이번 대화 내용을 추가
                AddLineToRemindPanel(currentLineData["Speaker"].ToString(), currentLineData["Belong"].ToString(), currentLineData["Dialog"].ToString());
                break;
            case "A":
                //대답형 연출 출력 작업 수행
                panel_Dialog.SetActive(false);
                panel_Choice.SetActive(true);
                button_Choice_0.SetActive(true);
                button_Skip.SetActive(false);
                button_Remind.SetActive(false);

                //0번 선택지 버튼의 텍스트를 업데이트
                button_Choice_0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();

                //0번 선택지 버튼을 화면 정중앙에 배치
                button_Choice_0.transform.localPosition = new Vector3(0,0,0);
                break;
            case "C2":
                //2개 선택지 출력 작업 수행
                panel_Dialog.SetActive(false);
                panel_Choice.SetActive(true);
                button_Choice_0.SetActive(true);
                button_Choice_1.SetActive(true);
                button_Skip.SetActive(false);
                button_Remind.SetActive(false);

                //0번 선택지 버튼과 1번 선택지 버튼의 텍스트를 업데이트
                button_Choice_0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice_1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();

                //2개의 선택지 버튼을 화면 중앙에 위아래로 배치
                button_Choice_0.transform.localPosition = new Vector3(0,75,0);
                button_Choice_1.transform.localPosition = new Vector3(0,-75,0);
                break;
            case "C3":
                //3개 선택지 출력 작업 수행
                panel_Dialog.SetActive(false);
                panel_Choice.SetActive(true);
                button_Choice_0.SetActive(true);
                button_Choice_1.SetActive(true);
                button_Choice_2.SetActive(true);
                button_Skip.SetActive(false);
                button_Remind.SetActive(false);

                //0번 선택지 버튼, 1번 선택지 버튼 그리고 2번 선택지 버튼의 텍스트를 업데이트하고, 화면 중앙에 위아래로 배치
                button_Choice_0.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_0"].ToString();
                button_Choice_1.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_1"].ToString();
                button_Choice_2.GetComponentInChildren<TextMeshProUGUI>().text = currentLineData["Choice_2"].ToString();

                //3개의 선택지 버튼을 화면 중앙에 위아래로 배치
                button_Choice_0.transform.localPosition = new Vector3(0,150,0);
                button_Choice_1.transform.localPosition = new Vector3(0,0,0);
                button_Choice_2.transform.localPosition = new Vector3(0,-150,0);
                break;
            case "J":
                //점프할 줄 번호를 파싱하여 LoadLine 함수 호출
                LoadLine(int.Parse(currentLineData["Jump_0"].ToString()));
                break;
            case "E":
                //채팅창 숨김 작업 수행
                panel_Dialog.SetActive(false);
                panel_Choice.SetActive(false);
                button_Choice_0.SetActive(false);
                button_Choice_1.SetActive(false);
                button_Choice_2.SetActive(false);
                button_Remind.SetActive(true);
                button_Skip.SetActive(false);

                SetLayerAtEnd();
                break;
        }
    }

    /// <summary> 채팅 타입 End를 만나 채팅창이 닫힐 때, 입력을 제어한다 </summary>
    /// <remarks> 현재에는 EncodeDecode 미니게임의 튜토리얼 구현에 사용되고 있음 </remarks>
    protected virtual void SetLayerAtEnd()
    {
        
    }





    /// <summary> data 전체를 순회한 후에 각 튜토리얼 단계의 줄 번호 정보가 담긴 배열을 반환한다 </summary> 
    private int[] GetArrayOfTutorialPhase()
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

    /// <summary> 현재 튜토리얼 단계를 반환한다 </summary>
    public int GetCurrentTutorialPhase()
    {
        return currentTutorialPhase;
    }

    /// <summary> 지정 시간을 대기한 후, 다음 튜토리얼 단계로 넘어간다 </summary>
    /// <param name="endTime"> 대기할 시간 </param>
    public void MoveToNextTutorialPhase(float endTime)
    {
        if (!PlayAsTutorial)
            return;
        currentTutorialPhase++;
        Debug.Log("Start TutorialPhase : " + currentTutorialPhase);
        StartCoroutine(MoveToNextTutorialPhase_IE(0, endTime));
    }

    private IEnumerator<WaitForSeconds> MoveToNextTutorialPhase_IE(float currentTime, float endTime)//MoveToNextTutorialPhase_IEnumerator
    {
        if(endTime == 0f)
        {
            DisplayTutorialDialog(tutorialPhaseArray[currentTutorialPhase], 0f);
            yield break;
        }

        currentTime += endTime / 100;
        if (currentTime > endTime)
        {
            DisplayTutorialDialog(tutorialPhaseArray[currentTutorialPhase], 0f);
            yield break;
        }

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(MoveToNextTutorialPhase_IE(currentTime, endTime));
    }

    /// <summary> 지정 시간을 대기한 후, 지정한 줄의 튜토리얼 대화를 표시한다 </summary>
    /// <param name="line"> 이동할 줄 번호 </param>
    /// <param name="endTime"> 대기할 시간 </param>
    public void DisplayTutorialDialog(int line, float endTime)
    {
        if (!PlayAsTutorial)
            return;
        StartCoroutine(DisplayTutorialDialog_IE(line, 0, endTime));
    }

    private IEnumerator<WaitForSeconds> DisplayTutorialDialog_IE(int line, float currentTime, float endTime)//DisplayTutorialDialog_IEnumerator
    {
        if(endTime == 0f)
        {
            LoadLine(line);
            yield break;
        }

        currentTime += endTime / 100;
        if(currentTime>endTime)
        {
            LoadLine(line);
            yield break;
        }

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(DisplayTutorialDialog_IE(line, currentTime, endTime));
    }




}
