using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Events;

public class Chat : MonoBehaviour
{
    /**
    대사 출력 코드
    - Paragraph들 출력하기
    - 대사중 버튼을 눌러 다음 대사로 넘어가기
    - 스킵하기를 눌러서 대사 종료 (이벤트 함수 실행, Ending 선택지 제외)
    - 대사 출력 후 로그 텍스트에 1개씩 추가 
    */
    [Header("UI 요소")]
    private GameObject chatUI;  // chat UI 오브젝트
    [SerializeField]
    private Image background;   // 배경 이미지
    [SerializeField]
    private GameObject talkPanel;   // 대화 패널
    public TMP_Text talkerName;     // 발화자 이름
    [SerializeField]
    private TMP_Text talkerInfo;    // 발화자 정보
    public TMP_Text text;           // 대화 내용

    [Space(10)]
    [SerializeField]
    private GameObject choicePanel;     // 선택지 패널 (선택지 3개)
    [SerializeField]
    private GameObject optionPanel;     // 옵션 패널 (다시보기, 스킵)

    [Space(20)] 
    [Header("파일 경로")]
    public string characterFilePath;    // 캐릭터 파일 경로
    public string backgroundFilePath;   // 배경 CG 파일 경로
    
    [Space(10)] 
    [Header("대화 상태")]
    public bool isTalk;            // 현재 대화 활성화 여부

    [SerializeField]
    public int index;   // 현재 대화 인덱스
    private List<Paragraph> chatList;   // 대화 리스트

    [Header("NPC 생성 정보")]
    [SerializeField]
    private GameObject npcPrefab;   // NPC 생성용 프리팹
    [SerializeField]
    private int npcSizeMultiplier;  // NPC 크기 배율

    /// 이벤트
    private ChatAction action;    // 대사 반응 함수
    private List<ChatAction> choiceActions = new List<ChatAction>();    // 선택지 이벤트

    /// 싱글턴 선언
    private static Chat _instance;
    public static Chat Instance { get { return _instance; } }
    public void Awake()
    {
        if(!_instance)
        {
            _instance = this;   // 싱글톤 할당

            isTalk = false;     //대사 초기화
            index = -1;

            // 이벤트 초기화
            choiceActions.Clear();

            // chat UI 초기화
            chatUI = transform.GetChild(0).gameObject;
            chatUI.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    ///<summary>
    ///대화 시작
    ///</summary>
    ///<param name="_chatList">대사 리스트</param>
    public void StartChat(List<Paragraph> _chatList)
    {
        // 대화 리스트 오류
        if (_chatList == null)
        {
            Debug.Log($"CHAT DATA CANNOT FOUND");
            return;
        }
        
        // 대화 리스트 할당
        chatList = _chatList;
        Debug.Log($"Chat Start"); 
        isTalk = true;

        chatUI.SetActive(true);
        NextChat(0);        
    }

    ///<summary>
    ///다음 대사 출력 함수
    ///</summary>
    ///<param name="idx">출력할 대사 인덱스</param>
    public void NextChat(int idx = -1)
    {       
        // 파일 미할당  
        if (chatList == null)
        {
            chatUI.SetActive(false);    //UI 비활성화
            return;
        }

        // 대사 진행중이면 종료
        StopAllCoroutines();    
        // 이전 대사 반응 함수 실행
        if (index != 0)
        {
            action.Invoke();
        }

        // 디폴트: 다음 텍스트로
        if (idx == -1)
        {
            idx = index + 1;
        }

        index = idx;    // 대사 넘김

        // 마지막 대사 이후 or index 오류
        if (idx >= chatList.Count)
        {
            Debug.Log($"Chat OFF: INDEX={idx}");
            FinishChat();    // Chat 종료 및 비활성화
            return;
        }

        Paragraph paragraph = chatList[idx]; // 현재 대사 불러오기

        SetChatUI(paragraph.chatType);  // 대사 타입에 따라 UI 설정
        
        // 대사 애니메이션 실행
        if (paragraph.chatType != "Choice")
        {
            StartCoroutine(TextAnimation(paragraph as NormalParagraph));
        }
    }

    /// 대사 종료
    private void FinishChat()
    {
        background.sprite = null;   // 배경 초기화
        isTalk = false;             // 대화 종료
        chatUI.SetActive(false);    // UI 종료
    }

    /// 다시보기 대화 로그 버튼
    public void OnReplayPressed()
    {

    }

    /// 대화 스킵 버튼
    public void OnSkipPressed()
    {
        while(true)
        {
            if (chatList[index].chatType == "Choice")
            {
                ChoiceParagraph paragraph = chatList[index] as ChoiceParagraph;
                foreach(var i in paragraph.choiceList)
                {
                    if (i.isEnding)
                        return;
                }
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

        choiceActions[num].Invoke();    // 반응 함수 실행
        NextChat();
    }


    /// <summary>
    /// 반응 함수 할당
    /// </summary>
    /// <param name="func"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static ChatAction SetAction(string func, string param)
    {
        ChatAction result;

        switch (func)
        {
        case "Jump":
            result = new ChatJumpAction();
            break;
        case "DayChange":
            result = new DayChangeAction();
            break;
        case "TimeChange":
            result = new TimeChangeAction();
            break;
        default:
            return null;
        }

        result.param = param;
        return result;
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

        button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = ParseVariables(choice.text, choice.variables);  // 선택지 텍스트 설정

        choiceActions[choiceNum] = SetAction(choice.reaction, choice.reactionParam);     // 선택지 반응 설정   

        button.SetActive(true);     // 선택지 활성화
    }

    /// <summary>
    /// 대사 타입에 맞는 UI 설정
    /// </summary>
    /// <param name="talkType">대사 타입</param>
    private void SetChatUI(string talkType)
    {
        switch(talkType)
        {
            /// 선택지 상태일때
            case "Choice":
                ChoiceParagraph choiceParagraph = chatList[index] as ChoiceParagraph;

                /// TODO: 캐릭터 CG 활성화
                choicePanel.SetActive(true);    // 선택지 패널 활성화
                optionPanel.SetActive(false);   // 옵션 패널 비활성화

                switch(choiceParagraph.choiceList.Count)
                {   
                    // 선택지 1개일때 (가운데 2번 사용)
                    case 1:
                        SetChoice(0);                               // 1번 선택지 비활성화
                        SetChoice(1, choiceParagraph.choiceList[0]);   // 2번 선택지 설정
                        SetChoice(2);                               // 3번 선택지 비활성화
                        break;
                    // 선택지 2개일때 (위, 아래 1,3번 사용)
                    case 2:
                        SetChoice(0, choiceParagraph.choiceList[0]);   // 1번 선택지 설정
                        SetChoice(1);                               // 2번 선택지 비활성화
                        SetChoice(2, choiceParagraph.choiceList[1]);   // 3번 선택지 설정
                        break;
                    // 선택지 3개일때
                    case 3:
                        SetChoice(0, choiceParagraph.choiceList[0]);   // 1번 선택지 설정
                        SetChoice(1, choiceParagraph.choiceList[1]);   // 2번 선택지 설정
                        SetChoice(2, choiceParagraph.choiceList[2]);   // 3번 선택지 설정
                        break;
                }
                break;

            /// 컷씬 상태일때
            case "CutScene":
                NormalParagraph cutSceneParagraph = chatList[index] as NormalParagraph;

                if (cutSceneParagraph.background != null)
                {
                    background.sprite = GetSprite(backgroundFilePath + cutSceneParagraph.background);    // 배경 이미지 설정 
                }
                background.gameObject.SetActive(true);      // 배경 이미지 활성화

                /// TODO: 캐릭터 CG 비활성화
                choicePanel.SetActive(false);    // 선택지 패널 비활성화
                optionPanel.SetActive(true);   // 옵션 패널 활성화 

                // 대사 존재시 대사창 활성화
                if (cutSceneParagraph.text != "")  
                {
                    talkPanel.SetActive(true);
                    talkerName.text = cutSceneParagraph.talker;     // 발화자 이름
                    talkerInfo.text = cutSceneParagraph.talkerInfo; // 발화자 설명

                    text.fontSize = cutSceneParagraph.fontSize;   // 대사 크기 설정
                    text.text = "";             // 대사 초기화
                }
                else
                    talkPanel.SetActive(false);  
                break;
            
            /// 일반 대화 상태일때
            case "Talk":
                NormalParagraph normalParagraph = chatList[index] as NormalParagraph;

                /// 캐릭터 CG 활성화
                background.gameObject.SetActive(false); // 배경 비활성화
                choicePanel.SetActive(false);   // 선택지 패널 비활성화
                optionPanel.SetActive(true);    // 옵션 패널 활성화 
                talkPanel.SetActive(true);      // 대화 패널 활성화

                talkerName.text = normalParagraph.talker;     // 발화자 이름
                talkerInfo.text = normalParagraph.talkerInfo; // 발화자 설명

                text.fontSize = normalParagraph.fontSize;   // 대사 크기 설정
                text.text = "";             // 대사 초기화

                break;
        }
        // 반응 설정
        action = SetAction(chatList[index].action, chatList[index].actionParam);    
    }

    /// <summary>
    /// 대사 출력 애니메이션
    /// </summary>
    /// <param name="paragraph">출력할 대사</param>
    IEnumerator TextAnimation(NormalParagraph paragraph)
    {
        /**
        * SFX 실행
        * 대사 출력
            - delay
            - 변수값 적용
        */

        // 변수값 적용
        paragraph.text = ParseVariables(paragraph.text, paragraph.variables);

        // 한 글자씩 애니메이션
        for (int i = 0; i < paragraph.text.Length; i++)
        {
            text.text += paragraph.text[i];
            /// TODO: 텍스트 효과음 출력
            yield return new WaitForSeconds(paragraph.textDelay / 10);
        }
    }

    /// <summary>
    /// 변수 텍스트 적용
    /// </summary>
    /// <param name="variableName">적용할 변수명</param>
    /// <returns>변수 실재값 반환</returns>
    private string GetVariableValue(string variableName)
    {
        switch(variableName)
        {
            case "year":
                return GameSystem.Instance.today.date.year.ToString();
            case "month":
                return GameSystem.Instance.today.date.month.ToString();
            case "day":
                return GameSystem.Instance.today.date.day.ToString();
        }
        return "";
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

    /// <summary>
    /// 스프라이트 이미지 불러오기
    /// </summary>
    /// <param name="filePath">이미지 경로</param>
    /// <returns></returns>
    public static Sprite GetSprite(string filePath)
    {
        Sprite result = Resources.Load<Sprite>(filePath);
        return result;
    }

    /// <summary>
    /// 월드에 새 NPC 생성
    /// </summary>
    /// <param name="npc"></param>
    /// <returns></returns>
    public static GameObject CreateNPC(string newNPCName)
    {
        // 오브젝트 생성
        GameObject newNPCObject = Instantiate(npcPrefab);
        RectTransform npcTransform = newNPCObject.GetComponent<RectTransform>();

        // NPC 데이터 로드하기
        newNPCObject.GetComponent<NPC>().npcFileName = newNPCName;
        newNPCObject.GetComponent<NPC>().GetData();
        NPCData npcData = newNPCObject.GetComponent<NPC>().npcData;
        if (npcData == null)
        {
            Destroy(newNPCObject);
            Debug.Log($"NPC Create Failed : ${newNPCName}");
            return null;
        }

        // 오브젝트 transform 설정
        newNPCObject.SetActive(false);
        newNPCObject.transform.SetParent(locationList[(int)npcData.location].transform.GetChild(npcData.locationIndex));
        npcTransform.anchoredPosition = npcData.position;
        npcTransform.localScale = new Vector3(1,1,1);   // 스케일 초기화

        // 오브젝트 이미지 설정
        if (npcData.image != null)
        {
            Image newImage = newNPCObject.GetComponent<Image>();      
            newImage.sprite = Chat.GetSprite(npcData.image);
            if (newImage.sprite == null)
            {
                Debug.Log($"이미지 없음 : {npcData.name}");
                return null;
            }
            // 오브젝트 크기 설정
            npcTransform.sizeDelta = npcData.size * new Vector2(1, newImage.sprite.rect.height/ newImage.sprite.rect.width) * npcSizeMultiplier;    // 비율 맞춰서 사이즈 설정
        }                
        return newNPCObject;
    }
}
