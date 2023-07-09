using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Paragraph
{
    [System.Serializable]
    public enum TalkType
    {
        /// 대화 타입
        talk,   // 일반 대화
        choice, // 
        cutScene
    }
    public TalkType type;    // 대사 타입
    public List<CharacterCG> character_L;  // 왼쪽 캐릭터 CG 리스트
    public List<CharacterCG> character_R;  // 오른쪽 캐릭터 CG 리스트
    public string background;  // 배경 이미지
    public string bgm;         // 배경음악
    public string action;  // 대화 후 이벤트
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
    public string talkerInfo;    // 발화자 설명
    public string text;  // 내용
    public List<VariableReplace> variables; // 변수값
    public int fontSize;   // 글자 크기
    public int delay;      // 텍스트간 딜레이

    [System.Serializable]
    public class VariableReplace
    {
        public string keyword;
        public string varaibleName;
    }
}   

/// 선택지 문단
[System.Serializable]
public class ChoiceParagraph : Paragraph
{
    public List<Choice> choices;
}



/// 선택지
[System.Serializable]
public class Choice
{
    [System.Serializable]
    public enum ChoiceType
    {
        /// 선택지 타입
    }
    public ChoiceType type;
    public string text;
    public string reaction;
    
}

/// 캐릭터 CG
[SerializeField]
public class CharacterCG
{
    public string name;    // 캐릭터 이름 (파일명)
    public int emotion;    // 캐릭터 표정 번호
    public int pose;       // 캐릭터 자세 번호
    public bool isHighlight;
}