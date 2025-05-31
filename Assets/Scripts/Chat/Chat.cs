using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Chat : Singleton<Chat>
{
    /**
    대사 출력 코드
    - Paragraph들 출력하기
    - 대사중 버튼을 눌러 다음 대사로 넘어가기
    - 스킵하기를 눌러서 대사 종료 (이벤트 함수 실행, Ending 선택지 제외)
    - 대사 출력 후 로그 텍스트에 1개씩 추가
    */
    private readonly int CGCOUNT = 4;
    private readonly int CHOICECOUNT = 3;
    private readonly string CHARACTERFILEPATH = "Chat/Character/";    // 캐릭터 파일 경로
    private readonly string BACKGROUNDFILEPATH = "Chat/Background/";   // 배경 CG 파일 경로
    
    private GameObject ChatUI => transform.GetChild(0).gameObject;
    [SerializeField] private ChatTutorialManager ChatTutorial;

    [Header("UI 요소")]
    [SerializeField] private Image background;   // 배경 이미지
    [SerializeField] private SoundManager bgm;    // 배경 음악

    [Space(20)] [Header("대화 패널")] 
    [SerializeField] private List<Image> CG;
    [SerializeField] private GameObject talkPanel;   // 대화 패널
    public TMP_Text talkerName;     // 발화자 이름
    [SerializeField] private TMP_Text talkerInfo;    // 발화자 정보
    public TMP_Text text;           // 대화 내용
    public SoundManager textSFX;    // 대화 효과음

    [Space(20)]
    [Header("선택지 패널")]
    [SerializeField] private GameObject choicePanel;     // 선택지 패널 (선택지 3개)
    
    [Space(20)]
    [Header("옵션 패널")]
    [SerializeField] private GameObject optionPanel;     // 옵션 패널 (다시보기, 스킵)
    [SerializeField] private GameObject remindContent;     // 다시보기 패널
    [SerializeField] private GameObject remindTalkNode;      // 대화 다시보기 노드 프리팹
    [SerializeField] private GameObject remindChoiceNode;    // 선택지 다시보기 노드 프리팹
    
    [Space(10)] 
    [Header("대화 상태")]
    private Queue<Paragraph> chatList;   // 대화 리스트
    private Queue<Paragraph> logList;   // 대화 기록 리스트

    /// 이벤트
    private Action action;    // 대사 반응 함수
    private Action[] choiceActions = new Action[3];    // 선택지 이벤트

    new void Awake()
    {
        base.Awake();

        // 이벤트 초기화
        choiceActions = new Action[3];
    }

    ///<summary>
    ///대화 시작
    ///</summary>
    /// <param name="obj">대사를 실행한 오브젝트</param>
    ///<param name="_chatList">대사 리스트</param>
    public void StartChat(GameObject obj, List<Paragraph> _chatList)
    {
        // 대화 리스트 오류
        if (_chatList == null)
        {
            Debug.Log($"CHAT DATA CANNOT FOUND");
            return;
        }
        
        // 대화 리스트 할당
        chatList = new Queue<Paragraph>(_chatList);
        logList = new Queue<Paragraph>();   

        ChatUI.SetActive(true);
        NextChat();        
    }

    ///<summary>
    ///다음 대사 출력 함수
    ///</summary>
    ///<param name="idx">출력할 대사 인덱스</param>
    public void NextChat()
    {      
        // 대사 진행중이면 종료
        StopAllCoroutines();  

        // 이전 대사 반응 함수 실행
        if (logList.Count != 0)
        {
            action?.Invoke();
        }        

        // 마지막 대사 이후 or index 오류
        if (chatList.Count <= 0)
        {
            FinishChat();    // Chat 종료 및 비활성화
            return;
        }

        Paragraph paragraph = chatList.Dequeue(); // 현재 대사 불러오기
        AddLog(paragraph);

        SetChat(paragraph);  // 대사 타입에 따라 설정
    }


    /// <summary>
    /// 대사 넘기기 함수
    /// </summary>
    public void SkipChat()
    {
        NextChat();
    }

    /// 대사 종료
    private void FinishChat()
    {
        background.sprite = null;   // 배경 초기화
        
        bgm.Pause();    // BGM 종료 및 월드 BGM 재개
        WorldSceneManager.Instance.worldBGM.Resume();
        
        ClearLog();     //로그 초기화
        ChatUI.SetActive(false);    // UI 종료
    }

    /// <summary>
    /// 대사 다시보기 추가
    /// </summary>
    /// <param name="para">추가할 대사</param>
    private void AddLog(Paragraph para)
    {
        logList.Enqueue(para);

        if (para is TalkParagraph)      // 대사 다시보기
        {
            TalkParagraph talkPara = para as TalkParagraph;
            GameObject newNode = Instantiate(remindTalkNode, remindContent.transform);

            newNode.transform.GetChild(0).GetComponent<TMP_Text>().text = talkPara.talker;  // 발화자 설정
            newNode.transform.GetChild(1).GetComponent<TMP_Text>().text = talkPara.text;    // 대사 내용
        }
        else if (para is ChoiceParagraph)   // 선택지 다시보기
        {
            ChoiceParagraph choicePara = para as ChoiceParagraph;
            GameObject newNode = Instantiate(remindChoiceNode, remindContent.transform);
            
            for(int i = 0; i < choicePara.choiceList.Count; i++)        // 선택지들 활성화
            {
                newNode.transform.GetChild(i).GetComponent<TMP_Text>().text = choicePara.choiceList[i].text;
                newNode.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// 대화 다시보기 초기화
    /// </summary>
    private void ClearLog()
    {
        for(int i = 0; i < remindContent.transform.childCount; i++)
        {
            Destroy(remindContent.transform.GetChild(i).gameObject);
        }
    }
    
    
    /// <summary>
    /// 선택지 할당
    /// </summary>
    /// <param name="choiceNum">선택지 번호</param>
    /// <param name="choice">선택지 정보</param>
    public void SetChoice(int choiceNum, Choice choice)
    {
        // 선택지 버튼 오류
        if (choiceNum >= choicePanel.transform.childCount || choiceNum < 0)
            return;

        // 해당 선택지 버튼
        GameObject button = choicePanel.transform.GetChild(choiceNum).gameObject;

        // 선택지 미사용시 비활성화
        if (choice == null)
        {
            button.SetActive(false);                        
            return;
        }   

        // 선택지 텍스트에 변수값 적용
        button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = ParseVariables(choice.text, choice.variables);  // 선택지 텍스트 설정
        // 선택지 반응 설정
        choiceActions[choiceNum] = ActionHandler.GetAction(choice.reaction, choice.reactionParam);        

        button.SetActive(true);     // 선택지 활성화
    }

    /// <summary>
    /// 대사 타입에 맞는 UI 설정
    /// </summary>
    /// <param name="data">대사 데이터</param>
    private void SetChat(Paragraph data)
    {
        // 패널들 초기 설정
        talkPanel.SetActive(true);
        choicePanel.gameObject.SetActive(false);
        optionPanel.gameObject.SetActive(true);

        // 대화 타입에 맞춰 UI들 설정
        if (data is TalkParagraph talk)        // 일반 대사
        {
            // 대사 설정
            if (!string.IsNullOrEmpty(talk.text))
            {
                talkPanel.SetActive(true); // 대화 패널 활성화

                talkerName.text = talk.talker; // 발화자 이름
                talkerInfo.text = talk.talkerInfo; // 발화자 설명

                text.fontSize = talk.GetFontSize(); // 대사 크기 설정

                if (talk.GetFontSize() == TalkParagraph.LARGEFONTSIZE)
                    textSFX.SetClip(1);
                else if (talk.GetFontSize() == TalkParagraph.NORMALFONTSIZE)
                    textSFX.SetClip(0);
                else
                    textSFX = new();
                
                StartCoroutine(TextAnimation(talk));
            }
        }
        
        else if(data is ChoiceParagraph choice)         // 일반 선택지
        {
            choicePanel.SetActive(true);    // 선택지 패널 활성화
            talkPanel.SetActive(false);
            optionPanel.SetActive(false);   // 옵션 패널 비활성화

            for (int i = 0; i < CHOICECOUNT; i++)
            {
                SetChoice(i, choice.choiceList[i]);
            }
        }
        else        // 대사 타입 오류
        {
            return;
        }
        
        // 캐릭터 CG 설정
        for(int i = 0; i < CGCOUNT; i++)
        {
            CharacterCG character = data.characters[i];
            // CG 없음
            if (character is null)
            {
                CG[i].sprite = null;
                CG[i].gameObject.SetActive(false);
                continue;
            }
                
            //CG 설정
            if (CG[i].sprite == null || character.fileName != CG[i].sprite.name)
            {
                CG[i].sprite = GetSprite(CHARACTERFILEPATH + character.fileName, character.index);
            }
            CG[i].gameObject.SetActive(true);
        }
        
        // 배경 설정
        if (data.background is null)
        {
            background.gameObject.SetActive(false);
        }
        else
        {
            if (background.sprite == null || background.sprite.name != data.background)
            {
                background.sprite = GetSprite(BACKGROUNDFILEPATH + data.background); // 배경 이미지 설정 
            }
            background.gameObject.SetActive(true);      // 배경 이미지 활성화
        }
    
        // 배경음악 설정
        if (data.bgm != "none")
        {
            // 모든 음악 중지
            if (data.bgm == "STOP")
            {
                WorldSceneManager.Instance?.worldBGM.Pause();
                bgm.Pause();
            }
            // 월드 음악으로 되돌림
            else if (data.bgm == "RETURN")
            {
                bgm.Stop();
                WorldSceneManager.Instance?.worldBGM.Resume();
            }
            // 대화 음악 재실행
            else if (data.bgm == "RESTART")
            {
                bgm.Resume();
            }
            // 대화 음악 새로 실행
            else
            {
                if (int.TryParse(data.bgm, out int result))
                {
                    WorldSceneManager.Instance?.worldBGM.Pause();
                    bgm.SetClip(result);
                    bgm.Play();
                }
            }
        }

        // 반응 설정
        action = ActionHandler.GetAction(data.action, data.actionParam);  
    }

    
    /************************************UI 이벤트 함수*****************************************/
    /// 대화 스킵 버튼
    public void OnSkipPressed()
    {
    #if DEBUG
        Debug.Log("Skip Pressed");
    #endif
        while(true)
        {
            // 대사 종료까지 반복
            if (chatList.Count == 0)
            {
                return;
            }
            // 도중 선택지까지 반복
            if (chatList.Peek().hasAction())
            {
                NextChat();
                return;
            }
            NextChat();            
        }
    }

    /// 선택지 버튼 입력 함수
    public void OnChoicePressed(int num)
    {   
        // 선택지 번호 오류
        if ( (num >= choicePanel.transform.childCount) || (num < 0) )
            return;

        // 반응 함수 실행
        choiceActions[num]?.Invoke();
        NextChat();
    }

    
    /***********************************텍스트 출력용 함수***************************************/
    /// <summary>
    /// 대사 출력 애니메이션
    /// </summary>
    /// <param name="paragraph">출력할 대사</param>
    /// <remarks>대사 delay, 변수값, SFX 적용</remarks>
    IEnumerator TextAnimation(TalkParagraph paragraph)
    {
        // 대사 초기화
        text.text = "";

        // 변수값 적용
        paragraph.text = ParseVariables(paragraph.text, paragraph.variables);

        // 한 글자씩 애니메이션
        for (int i = 0; i < paragraph.text.Length; i++)
        {
            // 텍스트 추가
            text.text += paragraph.text[i];
            
            // 텍스트 효과음 실행
            textSFX.Play();

            yield return new WaitForSeconds(paragraph.textDelay / 10);
        }
    }

    /// <summary>
    /// 대사 내 변수값 전환하기
    /// </summary>
    /// <param name="text">전환 전 대사</param>
    /// <param name="varList">대사 내 변수 리스트</param>
    /// <returns>전환된 대사</returns>
    private string ParseVariables(string text, List<Paragraph.VariableReplace> varList)
    {
        string result = text;

        foreach(var iter in varList)
        {
            string keyword = iter.keyword;
            string variableName = iter.variableName;
            
            if (result.Contains(keyword))
            {
                result = result.Replace(keyword, GetVariableValue(variableName));
            }
        }

        return result;
    }

/**************************************데이터값 호출 함수 static****************************************/


    /// <summary>
    /// 변수 텍스트 적용
    /// </summary>
    /// <param name="variableName">적용할 변수명</param>
    /// <returns>변수 실재값 반환</returns>
    private static string GetVariableValue(string variableName)
    {
        switch(variableName)
        {
            case "year":
                return GameSystem.Instance.DayData.date.year.ToString();
            case "month":
                return GameSystem.Instance.DayData.date.month.ToString();
            case "day":
                return GameSystem.Instance.DayData.date.day.ToString();
        }
        return "";
    }

    
    /// <summary>
    /// 스프라이트 이미지 불러오기
    /// </summary>
    /// <param name="filePath">이미지 경로</param>
    /// <returns></returns>
    private static Sprite GetSprite(string filePath)
    {
        Sprite result = Resources.Load<Sprite>(filePath);
        
    #if UNITY_EDITOR
        if (result == null)
        {
            Debug.Log($"Image Load Failed : {filePath}");
        }
    #endif
        
        return result;
    }

    
    /// <summary>
    /// 멀티 스프라이트 이미지 불러오기
    /// </summary>
    /// <param name="filePath">이미지 경로</param>
    /// <param name="i">멀티 이미지내 인덱스</param>
    /// <returns></returns>
    private static Sprite GetSprite(string filePath, int i)
    {
        Sprite[] result = Resources.LoadAll<Sprite>(filePath);
        
        // 파일명 오류
        if (result == null)
        {
        #if UNITY_EDITOR
            Debug.Log($"Image Load Failed : {filePath}");
        #endif

            return null;
        }

        // 파일 번호 오류
        if (result.Length < i)
        {
        #if UNITY_EDITOR
            Debug.Log($"Image Load Failed : {filePath} with {i}");
        #endif
            i = 0;
        }
        return result[i];
    }
}
