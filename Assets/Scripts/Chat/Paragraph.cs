using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Paragraph
{
    string type;    // 대사 타입
    List<CharacterCG> character_L;
    List<CharacterCG> character_R;
    string background;
    string bgm;

}

[System.Serializable]
public class NormalParagraph : Paragraph
{
    string talker;  // 발화자
    string talkerInfo;    // 발화자 설명
    string[] text;  // 내용
    int fontSize;   // 글자 크기
    int delay;      // 텍스트간 딜레이
}   

[System.Serializable]
public class ChoiceParagraph : Paragraph
{
    List<string> choiceTexts;   // 선택지 텍스트
    List<string> choiceReaction;    // 선택지 반응
}

[SerializeField]
public class CharacterCG
{
    string name;    // 캐릭터 이름 (파일명)
    int emotion;    // 캐릭터 표정 번호
    int pose;       // 캐릭터 자세 번호
}