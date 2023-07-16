using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Events;

public class Chat : MonoBehaviour
{
    /**
    대사 실행 코드
    - chat이름으로 대사 파일 불러오기
        chat이름_날짜index_시간.json
    - 대사중 버튼을 눌러 다음 대사로 넘어가기
    - 스킵하기를 눌러서 다음 선택지 or 대사 종료 (이벤트 함수 실행)
    - 대사 출력 후 로그 텍스트에 1개씩 추가 
    */
    [Header("UI 요소")]
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
    public string chatFilePath; // 대사 파일 경로
    public string characterFilePath;    // 캐릭터 파일 경로
    public string backgroundFilePath;   // 배경 CG 파일 경로
    
    [Header("대화 상태")]
    public bool isTalk;            // 현재 대화 활성화 여부
    [SerializeField]
    private int index;   // 현재 대화 인덱스

    /// 이벤트
    private UnityEvent normalEvent = new UnityEvent();
    private UnityEvent[] choiceEvents = new UnityEvent[3];

    private List<Paragraph> paragraphs; // 대화 리스트

    /// 싱글턴 선언
    private static Chat _instance;
    public static Chat Instance
    {
        get { return _instance; }
    }
    public void Awake()
    {
        if(!_instance)
        {
            _instance = this;   // 싱글톤 할당

            isTalk = false;     //대사 초기화
            index = -1;

            // 이벤트 초기화
            for(int i = 0; i<3; i++)
            {
                choiceEvents[i] = new UnityEvent();
            }

            StartChat("Henderson");         ///디버깅
        }
        else
        {
            Destroy(gameObject);
        }
    }

    ///<summary>
    ///대화 시작
    ///</summary>
    ///<param name="chatName">대사 파일명+날짜및시간으로 파일 탐색</param>
    ///<remarks>대화 파일 탐색 후 파일 존재시 대화 시작</remarks>
    public void StartChat(string chatName)
    {
        /*
        * 대화
        - chatName으로 파일 불러오고 Chat 활성화
        */
        string fileName = $"{chatName}_{GameSystem.Instance.player.dateIndex}_{GameSystem.Instance.player.time}";    // debug
        isTalk = LoadChatFile(fileName);
        if (isTalk == false)
            return;
        NextChat(0);        
    }

    ///<summary>
    ///다음 대사 출력 함수
    ///</summary>
    ///<param name="idx">출력할 대사 인덱스</param>
    public void NextChat(int idx = -1)
    {       
        // 파일 미할당  
        if (paragraphs == null)
            return;

        StopAllCoroutines();    // 대사 진행중이면 종료

        // 디폴트: 다음 텍스트로
        if (idx == -1)
        {
            idx = index + 1;
        }

        index = idx;    // 대사 넘김

        // 마지막 대사 이후 or index 오류
        if (idx >= paragraphs.Count)
        {
            Debug.Log($"Chat OFF: INDEX={idx}");
            isTalk = false;
            gameObject.SetActive(false);    // Chat 종료 및 비활성화
            return;
        }

        // 현재 대사 불러오기
        Paragraph paragraph = paragraphs[idx];

        SetChatUI(paragraph.type);  // 대사 타입에 따라 UI 설정
        
        if (paragraph.type != Paragraph.TalkType.choice)
        {
            Coroutine textAnimation = StartCoroutine(TextAnimation(paragraph as NormalParagraph));
        }
    }

    /// 대사 타입에 따라 UI 설정
    private void SetChatUI(Paragraph.TalkType talkType)
    {
        switch(talkType)
        {
            /// 선택지 상태일때
            case Paragraph.TalkType.choice:
                ChoiceParagraph choiceParagraph = paragraphs[index] as ChoiceParagraph;

                /// 캐릭터 CG 활성화
                choicePanel.SetActive(true);    // 선택지 패널 활성화
                optionPanel.SetActive(false);   // 옵션 패널 비활성화
                if (choiceParagraph == null)
                {
                    Debug.Log("선택지 없음");
                    return;
                }

                GameObject choiceButton;    // 할당할 버튼 iterator
                switch(choiceParagraph.choices.Count)
                {   
                    // 선택지 1개일때 (가운데 2번 사용)
                    case 1:
                        choicePanel.transform.GetChild(0).gameObject.SetActive(false);  // 1번 선택지 비활성화
                        choiceButton = choicePanel.transform.GetChild(1).gameObject;    // 2번 선택지 선택
                        choicePanel.transform.GetChild(2).gameObject.SetActive(false);  // 3번 선택지 비활성화

                        choiceButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = choiceParagraph.choices[0].text;  // 선택지 텍스트 설정
                        SetEventListener(choiceEvents[1], choiceParagraph.action);      // 이벤트 함수 설정
                        choiceButton.SetActive(true);   //버튼 활성화
                        break;
                    // 선택지 2개일때 (위, 아래 1,3번 사용)
                    case 2:
                        // 1번 선택지 설정
                        choiceButton = choicePanel.transform.GetChild(0).gameObject;
                        choiceButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = choiceParagraph.choices[0].text;
                        SetEventListener(choiceEvents[0], choiceParagraph.action);
                        choiceButton.SetActive(true);

                        // 3번 선택지 설정
                        choiceButton = choicePanel.transform.GetChild(2).gameObject;
                        choiceButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = choiceParagraph.choices[1].text;
                        SetEventListener(choiceEvents[2], choiceParagraph.action);
                        choiceButton.SetActive(true);
                        
                        choicePanel.transform.GetChild(1).gameObject.SetActive(false);  // 2번 선택지 비활성화
                        break;
                    // 선택지 3개일때
                    case 3:
                        // 첫번째 버튼 설정
                        choiceButton = choicePanel.transform.GetChild(0).gameObject;
                        choiceButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = choiceParagraph.choices[0].text;
                        SetEventListener(choiceEvents[0], choiceParagraph.action);
                        choiceButton.SetActive(true);
                        // 두번째 버튼 설정
                        choiceButton = choicePanel.transform.GetChild(1).gameObject;
                        choiceButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = choiceParagraph.choices[1].text;
                        SetEventListener(choiceEvents[1], choiceParagraph.action);
                        choiceButton.SetActive(true);
                        // 세번째 버튼 설정
                        choiceButton = choicePanel.transform.GetChild(2).gameObject;
                        choiceButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = choiceParagraph.choices[2].text;
                        SetEventListener(choiceEvents[2], choiceParagraph.action);
                        choiceButton.SetActive(true);
                        break;
                }
                break;

            /// 컷씬 상태일때
            case Paragraph.TalkType.cutScene:
                NormalParagraph cutSceneParagraph = paragraphs[index] as NormalParagraph;
                background.sprite = GetSprite(cutSceneParagraph.background);    // 배경 이미지 설정 
                background.gameObject.SetActive(true);      // 배경 이미지 활성화

                /// 캐릭터 CG 활성화
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

                SetEventListener(normalEvent, cutSceneParagraph.action);
                break;
            
            /// 일반 대화 상태일때
            case Paragraph.TalkType.talk:
                NormalParagraph normalParagraph = paragraphs[index] as NormalParagraph;

                /// 캐릭터 CG 활성화
                choicePanel.SetActive(false);   // 선택지 패널 비활성화
                optionPanel.SetActive(true);    // 옵션 패널 활성화 
                talkPanel.SetActive(true);      // 대화 패널 활성화

                talkerName.text = normalParagraph.talker;     // 발화자 이름
                talkerInfo.text = normalParagraph.talkerInfo; // 발화자 설명

                text.fontSize = normalParagraph.fontSize;   // 대사 크기 설정
                text.text = "";             // 대사 초기화

                SetEventListener(normalEvent, normalParagraph.action);
                break;
        }
    }

    /// 다시보기 대화 로그 버튼
    public void OnReplayPressed()
    {

    }

    /// 대화 스킵 버튼
    public void OnSkipPressed()
    {
        while(paragraphs[index].type != Paragraph.TalkType.choice)
        {
            NextChat();            
        }
    }

    /// 선택지 버튼 입력 함수
    public void OnChoicePressed(int num)
    {   
        // 선택지 번호 오류
        if ( (num >= 3) || (num < 0) )
            return;

        if (choiceEvents[num].GetPersistentEventCount() != 0)
            choiceEvents[num].Invoke();
    }

    /// 대사 출력 애니메이션
    IEnumerator TextAnimation(NormalParagraph paragraph)
    {
        /**
        * BGM 실행
        * 대사 출력
            - delay
            - 변수값 적용
        */
        text.text = paragraph.text;
        yield return new WaitForSeconds(0.1f);
    }

    /// 함수코드로 이벤트리스터 할당
    public void SetEventListener(UnityEvent _event, string func)
    {

    }

    /// 변수 텍스트 적용
    private string GetVariableValue(string variableName)
    {
        // switch(variableName)
        return "";
    }

    /// 스프라이트 파일 불러오기
    private Sprite GetSprite(string spriteName)
    {
        return null;
    }

    
    /// 대사파일명으로 데이터 불러옴
    private bool LoadChatFile(string fileName)
    {
        string path = Application.dataPath + chatFilePath + fileName + ".json";
        if (!File.Exists(path))
        {
            Debug.Log("NO FILE EXISTS: " + path);
            paragraphs = null;
            return false;
        }

        FileStream fileStream = new FileStream(path, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        // jsonString 읽어오기
        string jsonText = Encoding.UTF8.GetString(data);

        // paragraphs 초기화
        paragraphs = new List<Paragraph>();

        //Wrapper에서 데이터 추출
        ParagraphWrapper wrapper = JsonConvert.DeserializeObject<ParagraphWrapper>(jsonText,  new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        paragraphs = wrapper.data;
        return true;
    }
}
