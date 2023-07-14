using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO;
using System.Text;
using Newtonsoft.Json;

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


    [Header("파일 경로")]
    public string chatFilePath; // 대사 파일 경로
    public string characterFilePath;    // 캐릭터 파일 경로
    public string backgroundFilePath;   // 배경 CG 파일 경로
    
    [Header("대화 상태")]
    public bool onTalk;            // 현재 대화 활성화 여부
    [SerializeField]
    int paragraphIndex;   // 현재 대화 인덱스

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
            _instance = this;
            onTalk = false;
            paragraphIndex = -1;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void StartChat(string chatName)
    {
        /*
        * 대화
        - chatName으로 파일 불러오고 Chat 활성화
        */
        string fileName = $"{chatName}_{GameSystem.Instance.player.dateIndex}_{GameSystem.Instance.player.time}";    // debug
        LoadChatFile(fileName);
        
        onTalk = true;
        paragraphIndex = 0;

        NextChat(0);
        
    }

    private void LoadChatFile(string fileName)
    {
        string path = Application.dataPath + chatFilePath + fileName + ".json";
        if (!File.Exists(path))
        {
            Debug.Log("NO FILE EXISTS: " + path);
            paragraphs = null;
            return;
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
    }

    ///<summary>
    ///다음 대사 출력 함수
    ///</summary>
    ///<param name="idx">출력할 대사 인덱스</param>
    public void NextChat(int idx = -1)
    {
        if (idx == -1)
        {
            idx = paragraphIndex +1;
        }

        // 마지막 대사 이후 or index 오류
        if (idx >= paragraphs.Count)
        {
            Debug.Log($"Chat OFF: INDEX={idx}");
            gameObject.SetActive(false);    // Chat 종료 및 비활성화
            return;
        }

        // 현재 대사 불러오기
        Paragraph paragraph = paragraphs[idx];

        SetChatUI(paragraph.type);  // 대사 타입에 따라 UI 설정
        
    }

    /// 변수 텍스트 적용
    private string GetVariableValue(string variableName)
    {
        // switch(variableName)
        return "";
    }

    /// 대사 타입에 따라 UI 설정
    private void SetChatUI(Paragraph.TalkType talkType)
    {

    }

    /// 다시보기 대화 로그 버튼
    public void Replay()
    {

    }

    /// 대화 스킵 버튼
    public void Skip()
    {

    }

    /////////////////////////////////////////////////////////////////////////////////////
    public void DebugChatFile()
    {
        // 리스트 할당 디버그////////////////
        if (paragraphs == null)
        {
            Debug.Log("paragraph 리스트 null");
            return;
        }
        Debug.Log($"Paragraph 개수 : {paragraphs.Count.ToString()}");

        // 리스트내 각 요소 디버그/////////////
        foreach (Paragraph iter in paragraphs)
        {   
            // 요소 null 디버그
            if (iter == null)
            {
                Debug.Log("paragraph 요소 null");
                return;
            }

            Debug.Log($"paragraph 첫 데이터 정보");
            Debug.Log($"타입 : {iter.type}");

            // 캐릭터 CG 디버그
            if  (iter.characters == null)
            {   
                Debug.Log("Character 리스트 null");
            }
            else    
                Debug.Log($"캐릭터 CG 정보 : {iter.characters.Count}개");

            Debug.Log($"배경 : {iter.background}");
            Debug.Log($"이벤트 : {iter.action}");

            // 일반 대사일때 디버그
            if(iter.type == Paragraph.TalkType.talk)
            {
                // 형변환 디버그
                Debug.Log($"자료형 상태 : {iter.GetType().ToString()}");
                Debug.Log($"자료형 여부 : {iter is NormalParagraph}");
                NormalParagraph normalParagraph = iter as NormalParagraph;
                if (normalParagraph == null)
                {
                    Debug.Log("형변환 실패");
                    return;
                }
                // 일반 대사 내용 디버그
                Debug.Log($"발화자 : {normalParagraph.talker} + {normalParagraph.talkerInfo}");
                Debug.Log($"내용 : {normalParagraph.text}");
                Debug.Log($"변수 목록 : {normalParagraph.variables.Count}개");
            }

            // 선택지 대사일때 디버그
            else if(iter.type == Paragraph.TalkType.choice)
            {
                // 형변환 디버그
                ChoiceParagraph choiceParagraph = iter as ChoiceParagraph;
                if (choiceParagraph == null)
                {
                    Debug.Log("형변환 실패");
                    return;
                }
                // 선택지 대사 디버그
                Debug.Log($"선택지 : {choiceParagraph.choices.Count}개");
                Debug.Log($"첫 선택지 정보 : {choiceParagraph.choices[0].text} + {choiceParagraph.choices[0].reaction}반응");
            }
        }
    }
}
