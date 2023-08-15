using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Paragraph
{
    [System.Serializable]
    public enum TalkType
    {
        /// 대화 타입
        talk,   // 일반 대화
        choice, // 선택지 대화
        cutScene // 컷씬 대화
    }
    public TalkType type;    // 대사 타입
    public List<CharacterCG> characters = new List<CharacterCG>();  // 캐릭터 CG 리스트
    public string background = null;  // 배경 이미지
    public string bgm = null;         // 배경음악
    public string action = null;    // 대화 후 반응
    public int actionParam = -1;    // 반응 매개변수
}

[System.Serializable]
public class ParagraphWrapper
{
    public List<Paragraph> data;
}

/// 일반 대화형 문단
[System.Serializable]
public class NormalParagraph : Paragraph
{
    public string talker;  // 발화자
    public string talkerInfo = "";    // 발화자 설명
    public string text;  // 내용
    public List<VariableReplace> variables = new List<VariableReplace>(); // 변수값
    public int fontSize = 16;   // 글자 크기
    public float textDelay = 0.3f;      // 텍스트간 딜레이

    [System.Serializable]
    public class VariableReplace
    {
        public string keyword;      // 텍스트상의 변수 키워드
        public string variableName; // 해당하는 변수코드명
    }
}   

/// 선택지 문단
[System.Serializable]
public class ChoiceParagraph : Paragraph
{
    public List<Choice> choices = null; // 선택지들 리스트
}



/// 선택지
[System.Serializable]
public class Choice
{
    [System.Serializable]
    public enum ChoiceType
    {
        /// 선택지 타입
        normal,
        Ending
    }
    public ChoiceType type = ChoiceType.normal;  // 선택지 타입
    public string text;     //선택지 텍스트
    public string reaction = null;      //선택지 반응
    public int reactionParam = -1;       // 선택지 반응 매개변수
}

/// 캐릭터 CG
[SerializeField]
public class CharacterCG
{
    public string fileName;    // 캐릭터 파일명
    public string position = "Center"; // 왼쪽, 가운데, 오른쪽 CG 위치
    public int emotion = 0;    // 캐릭터 표정 번호
    public int pose = 0;       // 캐릭터 자세 번호
    public bool isHighlight = true; // 캐릭터 하이라이트
}