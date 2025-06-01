using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class Paragraph
{
    /**
    상속용 대사 클래스
    */
    public string chatType = "Normal";    // 대사 타입
    public List<CharacterCG>  characters = new() { new CharacterCG(), new CharacterCG(), new CharacterCG(), new CharacterCG() };
    public string bgm = "none";         // 배경음악
    public string background = null;  // 배경 이미지
    public string action = null;    // 대화 후 반응
    public string actionParam = null;    // 반응 매개변수


    public virtual bool hasAction()
    {
        if (action != null)
        {
            return true;
        }
        return false;
    }
}

[Serializable]
public class TalkParagraph : Paragraph
{
    /**
    일반 대사/컷씬 클래스
    - 기본적인 대화
    - 컷씬용 배경
    */
    public string talker;  // 발화자
    public string talkerInfo;    // 발화자 설명
    public string text;  // 내용

    // 글자 크기 기본값들
    public const int NORMALFONTSIZE = 16; 
    public const int LARGEFONTSIZE = 32;
    public const int SMALLFONTSIZE = 10;

    public string fontSize = "normal";   // 글자 크기
    public float textDelay = 0.4f;      // 텍스트간 딜레이


    public int GetFontSize()
    {
        switch(fontSize)
        {
            case "large":
                return LARGEFONTSIZE;
            case "small":
                return SMALLFONTSIZE;
            default:
                return NORMALFONTSIZE;
        }
    }

    [JsonConstructor]
    public TalkParagraph(string text)
    {
        talker = "";
        this.text = text;
    }
    
    public TalkParagraph(string talker, string talkerInfo, string text)
    {
        this.talker = talker;
        this.talkerInfo = talkerInfo;
        this.text = text;
    }
}   

[Serializable]
public class ChoiceParagraph : Paragraph
{
    /**
    선택지 대사 클래스
    - 대화 선택지들
    */
    public List<Choice> choiceList = new() { null, null, null}; // 선택지들 리스트

    public override bool hasAction()
    {
        bool hasChoiceAction = false;
        foreach(var choice in choiceList)
        {
            if (choice.reaction != null)
            {
                hasChoiceAction = true;
                break;
            }
        }
        return base.hasAction() || hasChoiceAction;
    }
}

[Serializable]
public class Choice
{
    /**
    선택지
    - 선택지 타입 (엔딩 분기 여부)
    - 선택지별 데이터
    */
    public bool isEnding = false;  // 선택지 타입
    public string text;     //선택지 텍스트
    public string reaction = null;      //선택지 반응
    public string reactionParam = null;       // 선택지 반응 매개변수
}

[Serializable]
public struct CharacterCG
{
    /**
    캐릭터CG
    - 캐릭터 이미지 정보
    - 대사 애니메이션
    */
    public string fileName;    // 캐릭터 파일명
    public int index;    // 이미지 내 인덱스
    public bool isHighlight; // 캐릭터 하이라이트

    public CharacterCG(string fileName = "", int index = 0, bool isHighlight = false)
    {
        this.fileName = fileName;
        this.index = index;
        this.isHighlight = isHighlight;
    }
}