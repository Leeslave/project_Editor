using System;
using System.Collections.Generic;

[Serializable]
public class Paragraph
{
    /**
    상속용 대사 클래스
    - 대사 타입과 기본 데이터
    */
    public string chatType = "Normal";    // 대사 타입
    public List<CharacterCG> characters = new();  // 캐릭터 CG 리스트
    public string bgm = null;         // 배경음악
    public string action = null;    // 대화 후 반응
    public string actionParam = null;    // 반응 매개변수
    
    [Serializable]
    public class VariableReplace
    {
        public string keyword;      // 텍스트상의 변수 키워드
        public string variableName; // 해당하는 변수코드명
    }
}

[Serializable]
public class NormalParagraph : Paragraph
{
    /**
    일반 대사/컷씬 클래스
    - 기본적인 대화
    - 컷씬용 배경
    */
    public string talker;  // 발화자
    public string talkerInfo = "";    // 발화자 설명
    public string text;  // 내용
    public List<VariableReplace> variables = new(); // 변수값
    public int fontSize = 16;   // 글자 크기
    public float textDelay = 0.3f;      // 텍스트간 딜레이
    public string background = null;  // 배경 이미지
}   

[Serializable]
public class ChoiceParagraph : Paragraph
{
    /**
    선택지 대사 클래스
    - 대화 선택지들
    */
    public List<Choice> choiceList = null; // 선택지들 리스트
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
    public List<Paragraph.VariableReplace> variables = new(); // 변수값
    public string reaction = null;      //선택지 반응
    public string reactionParam = null;       // 선택지 반응 매개변수
}

[Serializable]
public class CharacterCG
{
    /**
    캐릭터CG
    - 캐릭터 이미지 정보
    - 대사 애니메이션
    */
    public string fileName;    // 캐릭터 파일명
    public int index = 0;    // 이미지 내 인덱스
    public string position = "Center"; // 왼쪽, 가운데, 오른쪽 CG 위치

    public bool isHighlight = true; // 캐릭터 하이라이트
    public string animation = null; // 캐릭터 모션
}