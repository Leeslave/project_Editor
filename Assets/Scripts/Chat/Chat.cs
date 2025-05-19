using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Chat : Singleton<Chat>
{
    /**
    대사 출력 코드
    - Paragraph들 출력하기
    - 대사중 버튼을 눌러 다음 대사로 넘어가기
    - 스킵하기를 눌러서 대사 종료 (이벤트 함수 실행, Ending 선택지 제외)
    - 대사 출력 후 로그 텍스트에 1개씩 추가
    */
    [SerializeField] private GameObject chatTarget;
    private GameObject ChatUI => transform.GetChild(0).gameObject;
    [SerializeField] private ChatTutorialManager ChatTutorial;

    [Header("UI 요소")]
    [SerializeField] private Image background;   // 배경 이미지
    [SerializeField] private SoundManager bgm;    // 배경 음악

    [Space(20)]
    [Header("대화 패널")]
    [SerializeField] private Image characterL;    // 왼쪽 캐릭터 CG
    [SerializeField] private Image characterL2;    // 왼쪽 캐릭터 CG
    [SerializeField] private Image characterR;    // 오른쪽 캐릭터 CG
    [SerializeField] private Image characterR2;    // 왼쪽 캐릭터 CG
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


    [Space(30)] 
    [Header("파일 경로")]
    private readonly string CHARACTERFILEPATH = "Chat/Character/";    // 캐릭터 파일 경로
    private readonly string BACKGROUNDFILEPATH = "Chat/Background/";   // 배경 CG 파일 경로
    
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
        
        // 대화 객체 연결
        chatTarget = obj;
        
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
    public void SetChoice(int choiceNum, Choice choice = null)
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
    /// <param name="talkType">대사 타입</param>
    private void SetChat(Paragraph para)
    {
        // CG 초기 설정
        characterL.gameObject.SetActive(false);
        characterR.gameObject.SetActive(false);
        characterL2.gameObject.SetActive(false);
        characterR2.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        // 패널들 초기 설정
        talkPanel.SetActive(false);
        choicePanel.gameObject.SetActive(false);
        optionPanel.gameObject.SetActive(true);

        // 대화 타입에 맞춰 UI들 설정
        if (para is TalkParagraph)        /// 일반 대사
        {
            TalkParagraph talk = para as TalkParagraph;
            
            // 캐릭터 CG 활성화
            if (!string.IsNullOrEmpty(talk.characterL?.fileName))
            {
                if (characterL.sprite is null ||
                    characterL.sprite.name != talk.characterL.fileName)
                {
                    characterL.sprite = GetSprite($"{CHARACTERFILEPATH}{talk.characterL.fileName}", talk.characterL.index);
                }
                characterL.gameObject.SetActive(true);
            }
            if (!string.IsNullOrEmpty(talk.characterL2?.fileName))
            {
                if (characterL2.sprite == null ||
                    (characterL2.sprite.name != talk.characterL2.fileName))
                {
                    characterL2.sprite = GetSprite($"{CHARACTERFILEPATH}{talk.characterL2.fileName}", talk.characterL2.index);
                }
                characterL2.gameObject.SetActive(true);
            }
            if (!string.IsNullOrEmpty(talk.characterR?.fileName))
            {
                if (characterR.sprite == null ||
                    (characterR.sprite.name != talk.characterR.fileName))
                {
                    characterR.sprite = GetSprite($"{CHARACTERFILEPATH}{talk.characterR.fileName}", talk.characterR.index);
                }
                characterR.gameObject.SetActive(true);
            }
            if (!string.IsNullOrEmpty(talk.characterR2?.fileName))
            {
                if (characterR2.sprite == null ||
                    (characterR2.sprite.name != talk.characterR2.fileName))
                {
                    characterR2.sprite = GetSprite($"{CHARACTERFILEPATH}{talk.characterR2.fileName}", talk.characterR2.index);
                }
                characterR2.gameObject.SetActive(true);
            }
            
            // 대화 존재시
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
        
        else if(para is ChoiceParagraph)         /// 일반 선택지
        {
            ChoiceParagraph choicePara = para as ChoiceParagraph;

            // 캐릭터 CG 활성화
            if (choicePara.characterL is not null)
            {
                characterL.sprite = GetSprite($"{CHARACTERFILEPATH}{choicePara.characterL.fileName}_{choicePara.characterL.index}");
                characterL.gameObject.SetActive(true);
            }
            if (choicePara.characterR is not null)
            {
                characterR.sprite = GetSprite($"{CHARACTERFILEPATH}{choicePara.characterR.fileName}_{choicePara.characterR.index}");
                characterR.gameObject.SetActive(true);
            }
            if (choicePara.characterL2 is not null)
            {
                characterL2.sprite = GetSprite($"{CHARACTERFILEPATH}{choicePara.characterL2.fileName}_{choicePara.characterL2.index}");
                characterL2.gameObject.SetActive(true);
            }
            if (choicePara.characterR is not null)
            {
                characterR2.sprite = GetSprite($"{CHARACTERFILEPATH}{choicePara.characterR2.fileName}_{choicePara.characterR2.index}");
                characterR2.gameObject.SetActive(true);
            }

            choicePanel.SetActive(true);    // 선택지 패널 활성화
            optionPanel.SetActive(false);   // 옵션 패널 비활성화

            switch(choicePara.choiceList.Count)
            {   
                // 선택지 1개일때 (가운데 2번 사용)
                case 1:
                    SetChoice(0);                               // 1번 선택지 비활성화
                    SetChoice(1, choicePara.choiceList[0]);   // 2번 선택지 설정
                    SetChoice(2);                               // 3번 선택지 비활성화
                    break;
                // 선택지 2개일때 (위, 아래 1,3번 사용)
                case 2:
                    SetChoice(0, choicePara.choiceList[0]);   // 1번 선택지 설정
                    SetChoice(1);                               // 2번 선택지 비활성화
                    SetChoice(2, choicePara.choiceList[1]);   // 3번 선택지 설정
                    break;
                // 선택지 3개일때
                case 3:
                    SetChoice(0, choicePara.choiceList[0]);   // 1번 선택지 설정
                    SetChoice(1, choicePara.choiceList[1]);   // 2번 선택지 설정
                    SetChoice(2, choicePara.choiceList[2]);   // 3번 선택지 설정
                    break;
            }
        }
        else        // 대사 타입 오류
        {
            throw new Exception("Unknown Chat Data");
        }

        // 배경이미지 설정 (컷씬)
        // 배경 활성화

        if (para.background is not null)
        {
            background.sprite = GetSprite(BACKGROUNDFILEPATH + para.background);    // 배경 이미지 설정 
        }
        if (background.sprite is not null)
            background.gameObject.SetActive(true);      // 배경 이미지 활성화

    
        // 배경음악 설정
        if (para.bgm != "none")
        {
            // 모든 음악 중지
            if (para.bgm == "STOP")
            {
                WorldSceneManager.Instance.worldBGM.Pause();
                bgm.Pause();
            }
            // 월드 음악으로 되돌림
            else if (para.bgm == "RETURN")
            {
                bgm.Stop();
                WorldSceneManager.Instance.worldBGM.Resume();
            }
            // 대화 음악 재실행
            else if (para.bgm == "RESTART")
            {
                bgm.Resume();
            }
            // 대화 음악 새로 실행
            else
            {
                if (int.TryParse(para.bgm, out int result))
                {
                    WorldSceneManager.Instance.worldBGM.Pause();
                    bgm.SetClip(result);
                    bgm.Play();
                }
            }
        }

        // 반응 설정
        action = ActionHandler.GetAction(para.action, para.actionParam);  
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
    public static string GetVariableValue(string variableName)
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
    public static Sprite GetSprite(string filePath)
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
    public static Sprite GetSprite(string filePath, int i)
    {
        Sprite[] result = Resources.LoadAll<Sprite>(filePath);
        
        if (result == null)
        {
        #if UNITY_EDITOR
            Debug.Log($"Image Load Failed : {filePath}");
        #endif

            return null;
        }
        
        return result[i];
    }
}
