using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameAction;
using Utility;


public class Chat : Singleton<Chat>
{
    /**
    대사 출력 코드
    - Paragraph들 출력하기
    - 대사중 버튼을 눌러 다음 대사로 넘어가기
    - 스킵하기를 눌러서 대사 종료 (이벤트 함수 실행, Ending 선택지 제외)
    - 대사 출력 후 로그 텍스트에 1개씩 추가
    */
    private static readonly int CG_COUNT = 4;
    private static readonly int CHOICE_COUNT = 3;
    private const string CharacterPath = "Chat/Character/";    // 캐릭터 파일 경로
    private const string BackgroundPath = "Chat/Background/";   // 배경 standImages 파일 경로
    
    private GameObject ChatUI => transform.GetChild(0).gameObject;

    [Header("UI 요소")]
    [SerializeField] private Image background;   // 배경 이미지
    [SerializeField] private FadeCurtain curtain;
    [SerializeField] private SoundManager bgm;    // 배경 음악

    [Space(20)] [Header("대화 패널")] 
    [SerializeField] private List<Image> standImages;
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
    private Paragraph _currentParagraph;
    private Queue<Paragraph> _chatList;   // 대화 리스트
    private Queue<Paragraph> _logList;   // 대화 기록 리스트

    /// 이벤트
    private IGameAction _gameAction = new NotImpletedAction();    // 대사 반응 함수
    private IGameAction[] _choiceActions = new IGameAction[3];    // 선택지 이벤트

    private Coroutine _talkAnimation;
    private Coroutine _sfxAnimation;

    private void Awake()
    {
        // 이벤트 초기화
        _choiceActions = new IGameAction[3];
    }

    #region Chat Control
    
    ///<summary>
    ///대화 시작
    ///</summary>
    ///<param name="chats">대사 리스트</param>
    public void StartChat(List<Paragraph> chats)
    {
        // 대화 리스트 오류
        if (chats == null)
        {
            EditorLogger.Log($"CHAT DATA CANNOT FOUND");
            return;
        }

        // 변수키워드 적용
        for (int i = 0; i < chats.Count; i++)
        {
            chats[i] = ReplaceKeywords(chats[i]);
        }
        
        // 대화 리스트 할당
        _chatList = new Queue<Paragraph>(chats);
        _logList = new Queue<Paragraph>();   

        ChatUI.SetActive(true);
        NextChat();        
    }

    ///<summary>
    ///다음 대사 출력 함수
    ///</summary>
    public void NextChat()
    {      
        // 대사 진행중이면 종료
        if (_talkAnimation != null)
        {
            StopCoroutine(_sfxAnimation);
            StopCoroutine(_talkAnimation); 
            _talkAnimation = null;
            _sfxAnimation = null;
            
            // 대사 즉시 표시
            if (_currentParagraph is TalkParagraph talk)
            {
                text.text = talk.text;
                return;
            }
        } 

        // 이전 대사 반응 함수 실행
        if (_logList.Count != 0)
        {
            _gameAction?.Invoke();
        }        

        // 마지막 대사 이후 or index 오류
        if (_chatList.Count <= 0)
        {
            FinishChat();    // Chat 종료 및 비활성화
            return;
        }

        _currentParagraph = _chatList.Dequeue(); // 현재 대사 불러오기
        AddLog(_currentParagraph);

        SetChat(_currentParagraph);  // 대사 타입에 따라 설정
    }
    
    /// <summary>
    /// 대사 넘기기 함수
    /// </summary>
    public void SkipChat()
    {
        // 진행중 대사 즉시 종료
        if (_talkAnimation != null)
        {
            StopAllCoroutines();
            _talkAnimation = null;
        }
        
        // 모든 대화 이벤트 실행
        foreach (Paragraph paragraph in _chatList)
        {
            if (paragraph is not TalkParagraph talk)
            {
                continue;
            }

            if (talk.hasAction())
            {
                ActionHandler.Create(talk.action, talk.actionParam).Invoke();
            }
        }
        FinishChat();
    }

    /// 대사 종료
    private void FinishChat()
    {
        background.sprite = null;   // 배경 초기화
        
        // BGM 종료 및 월드 BGM 재개
        bgm.Stop();
        SoundManager.ResumeBGM();
        
        ClearLog();     //로그 초기화
        ChatUI.SetActive(false);    // UI 종료
    }
    
    #endregion
    
    #region Log Control

    /// <summary>
    /// 대사 다시보기 추가
    /// </summary>
    /// <param name="para">추가할 대사</param>
    private void AddLog(Paragraph para)
    {
        _logList.Enqueue(para);

        switch (para)
        {
            // 대사 다시보기
            case TalkParagraph talk:
                {
                    GameObject newNode = Instantiate(remindTalkNode, remindContent.transform);

                    newNode.transform.GetChild(0).GetComponent<TMP_Text>().text = talk.talker;  // 발화자 설정
                    newNode.transform.GetChild(1).GetComponent<TMP_Text>().text = talk.text;    // 대사 내용
                    break;
                }
            // 선택지 다시보기
            case ChoiceParagraph choice:
                {
                    GameObject newNode = Instantiate(remindChoiceNode, remindContent.transform);
                    for(int i = 0; i < CHOICE_COUNT; i++)        // 선택지들 활성화
                    {
                        newNode.transform.GetChild(i).GetComponent<TMP_Text>().text = choice.choiceList[i].text;
                        newNode.transform.GetChild(i).gameObject.SetActive(true);
                    }
                    break;
                }
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
    
    #endregion
    
    #region UI Control
    
    /// <summary>
    /// 선택지 할당
    /// </summary>
    /// <param name="choiceNum">선택지 번호</param>
    /// <param name="choice">선택지 정보</param>
    private void SetChoice(int choiceNum, Choice choice)
    {
        // 선택지 버튼 오류
        if (choiceNum >= choicePanel.transform.childCount || choiceNum < 0)
            return;

        // 해당 선택지 버튼
        GameObject button = choicePanel.transform.GetChild(choiceNum).gameObject;

        // 선택지 미사용시 비활성화
        if (string.IsNullOrEmpty(choice.text))
        {
            button.SetActive(false);                        
            return;
        }   
        button.transform.GetChild(0).GetComponent<TMP_Text>().text = choice.text;
        
        // 선택지 반응 설정
        _choiceActions[choiceNum] = ActionHandler.Create(choice.reaction, choice.reactionParam);        
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
                
                _talkAnimation = StartCoroutine(TextAnimation(talk));
            }
            
            // 배경음악 설정
            if (talk.bgm != "none")
            {
                // 모든 음악 중지
                if (talk.bgm == "STOP")
                {
                    SoundManager.PauseBGM();
                }
                // 월드 음악으로 되돌림
                else if (talk.bgm == "RETURN")
                {
                    bgm.Stop();
                }
                // 대화 음악 재실행
                else if (talk.bgm == "RESTART")
                {
                    bgm.Play();
                }
                // 대화 음악 새로 실행
                else
                {
                    if (int.TryParse(talk.bgm, out int result))
                    {
                        bgm.SetClip(result);
                        bgm.Play();
                    }
                }
            }
            
            
            // 반응 설정
            _gameAction = ActionHandler.Create(talk.action, talk.actionParam);  
        }
        
        else if(data is ChoiceParagraph choice)         // 일반 선택지
        {
            choicePanel.SetActive(true);    // 선택지 패널 활성화
            talkPanel.SetActive(false);
            optionPanel.SetActive(false);   // 옵션 패널 비활성화

            for (int i = 0; i < CHOICE_COUNT; i++)
            {
                SetChoice(i, choice.choiceList[i]);
            }
        }
        else        // 대사 타입 오류
        {
            return;
        }
        // 캐릭터 standImages 설정
        for(int i = 0; i < CG_COUNT; i++)
        {
            CharacterCG character = data.characters[i];
            // standImages 없음
            if (string.IsNullOrEmpty(character.fileName))
            {
                standImages[i].sprite = null;
                standImages[i].gameObject.SetActive(false);
                continue;
            }
                
            //standImages 설정
            if (!standImages[i].sprite || character.fileName != standImages[i].sprite.name)
            {
                standImages[i].sprite = DataLoader.GetSprite(character.index, CharacterPath, character.fileName);
            }
            standImages[i].gameObject.SetActive(true);
        }
        
        // 배경 설정
        if (data.background is null or "none")
        {
            background.gameObject.SetActive(false);
        }
        else
        {
            if (!background.sprite || background.sprite.name != data.background)
            {
                background.sprite = DataLoader.GetSprite(BackgroundPath, data.background); // 배경 이미지 설정 
            }
            
            if (data.isFade)        // 배경 전환 효과
            {
                curtain.Fade(FadeMode.In);
            }
            
            background.gameObject.SetActive(true);      // 배경 이미지 활성화
        }
    }
    
    #endregion

    #region Event

    /// 대화 스킵 버튼
    public void OnSkipPressed() => SkipChat();

    /// 선택지 버튼 입력 함수
    public void OnChoicePressed(int num)
    {   
        // 선택지 번호 오류
        if ( (num >= choicePanel.transform.childCount) || (num < 0) )
            return;

        // 반응 함수 실행
        _choiceActions[num]?.Invoke();
        NextChat();
    }

    #endregion
    
    #region Text Animation

    /// <summary>
    /// 대사 출력 애니메이션
    /// </summary>
    /// <param name="paragraph">출력할 대사</param>
    /// <remarks>대사 delay, 변수값, SFX 적용</remarks>
    IEnumerator TextAnimation(TalkParagraph paragraph)
    {
        // 대사 초기화
        text.text = "";
        
        // 효과음 코루틴 시작
        if (paragraph.sfxDelay > 0f)
        {
            _sfxAnimation = StartCoroutine(TextSFX(paragraph.sfxDelay / 10));
        }
        
        // 한 글자씩 애니메이션
        try
        {
            foreach (char t in paragraph.text)
            {
                // 텍스트 추가
                text.text += t;
                yield return new WaitForSeconds(paragraph.textDelay / 10);
            }
        }
        finally
        {
            if (_sfxAnimation != null)
            {
                StopCoroutine(_sfxAnimation);
            }
            _talkAnimation = null;
        }
    }
    
    private IEnumerator TextSFX(float delay)
    {
        while (true)
        {
            textSFX.Play();
            yield return new WaitForSeconds(delay);
        }
    }

    #endregion
    
    #region Set Variable Keyword
    
    /// <summary>
    /// 대사 내 변수값 전환하기
    /// </summary>
    /// <param name="data"></param>
    /// <returns>전환된 대사</returns>
    private static Paragraph ReplaceKeywords(Paragraph data)
    {
        StringBuilder sb = new();
        switch (data)
        {
            case TalkParagraph talk:
                sb.Append(talk.text);
                ProcessKeywords(sb);
                talk.text = sb.ToString();
                return talk;
            case ChoiceParagraph choice:
                for(int i = 0; i < CHOICE_COUNT; i++)
                {
                    Choice newChoice = choice.choiceList[i];
                    sb.Clear();
                    sb.Append(newChoice.text);
                    ProcessKeywords(sb);
                    newChoice.text = sb.ToString();
            
                    choice.choiceList[i] = newChoice;
                }
                return choice;
            default:
                return data; // 다른 타입의 Paragraph는 그대로 반환
        }
    }
    
    /// <summary>
    /// 키워드 대체 함수
    /// </summary>
    /// <param name="sb"></param>
    private static void ProcessKeywords(StringBuilder sb)
    {
        foreach (string keyword in StringExtensions.Keywords)
        {
            if (!sb.ToString().Contains(keyword))
            {
                continue;
            }

            string value = keyword;
            value.SwitchToValue();
            sb.Replace(keyword, value);
        }
    }
    
    #endregion
}
