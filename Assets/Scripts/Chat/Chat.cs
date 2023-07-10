using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO;
using System.Text;

public class Chat : MonoBehaviour
{

    [Header("UI 요소")]

    [Header("파일 경로")]
    public string filePath; // 대사 파일 경로
    
    [Header("대화 상태")]
    private List<Paragraph> paragraphs; // 대화 리스트
    bool onTalk;            // 현재 대화 활성화 여부
    int currentParagraph;   // 현재 대화 인덱스

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
            StartChat("Henderson");
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
            - 각 Paragraph를 순차로 이동하며 실행
            - 타입에 따라 분류 실행
        */
        string fileName = $"{chatName}_0_1";    // debug
        LoadChatFile(fileName);

        DebugChatFile();
    }

    private void LoadChatFile(string fileName)
    {
        string path = Application.dataPath + filePath + fileName + ".json";
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
        string jsonObjectData = Encoding.UTF8.GetString(data);

        // paragraphs 초기화
        paragraphs = new List<Paragraph>();

        //Wrapper에서 데이터 추출
        ParagraphWrapper wrapper = JsonUtility.FromJson<ParagraphWrapper>(jsonObjectData);
        paragraphs = wrapper.data;
    }

    /// 대사 진행 코루틴
    IEnumerator OnParagraph(Paragraph paragraph)
    {
        yield return new WaitForSeconds(1);
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
        if (paragraphs == null)
        {
            Debug.Log("paragraph 리스트 null");
            return;
        }
        Debug.Log($"Paragraph 개수 : {paragraphs.Count.ToString()}");

        if (paragraphs[0] == null)
        {
            Debug.Log("paragraph 요소 null");
            return;
        }
        Debug.Log($"paragraph 첫 데이터 정보");
        Debug.Log($"타입 : {paragraphs[0].type}");

        if (paragraphs[0].characters == null)
        {
            Debug.Log("Character 리스트 null");
        }
        else    
            Debug.Log($"캐릭터 CG 정보 : {paragraphs[0].characters.Count}개");

        Debug.Log($"배경 : {paragraphs[0].background}");
        Debug.Log($"이벤트 : {paragraphs[0].action}");

        if(paragraphs[0].type == Paragraph.TalkType.talk)
        {
            Debug.Log($"자료형 상태 : {paragraphs[0].GetType().ToString()}");
            NormalParagraph normalParagraph = paragraphs[0] as NormalParagraph;
            if (normalParagraph == null)
            {
                Debug.Log("형변환 실패");
                return;
            }
            Debug.Log($"발화자 : {normalParagraph.talker} + {normalParagraph.talkerInfo}");
            Debug.Log($"내용 : {normalParagraph.text}");
            Debug.Log($"변수 목록 : {normalParagraph.variables.Count}개");
        }

        if(paragraphs[0].type == Paragraph.TalkType.choice)
        {
            ChoiceParagraph choiceParagraph = paragraphs[0] as ChoiceParagraph;
            if (choiceParagraph == null)
            {
                Debug.Log("형변환 실패");
                return;
            }
            Debug.Log($"선택지 : {choiceParagraph.choices.Count}개");
            Debug.Log($"첫 선택지 정보 : {choiceParagraph.choices[0].text} + {choiceParagraph.choices[0].reaction}반응");
        }
    }
}
