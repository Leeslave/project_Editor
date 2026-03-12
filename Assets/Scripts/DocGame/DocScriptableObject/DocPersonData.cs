using System;
using UnityEngine;

[Serializable]
public class CitizenRecord
{
    [Header("기본 정보")]
    public string citizenName;
    public TimelineEntry[] testimony; // 시민의 증언 내용

    public TimelineEntry[] actualRecord; // 시민의 실제 행동 내용

    public bool hasDiscrepancy; // 실제 행동과 증언이 일치하면 true, 불일치 시 false -> 나중 최종 점검에 점검할 내용
}

[Serializable]
public class TimelineEntry
{
    public string timeLable; // 타임 라인 라벨
    [TextArea(2, 4)]
    public string content; // 내용

    public string[] keywords; // 핵심 키워드
}


[CreateAssetMenu(fileName = "DocDayData_", menuName = "DocGame/DayData")]
public class DocPersonData : ScriptableObject
{
    [Header("조사 대상 시민 목록")]
    public CitizenRecord[] citizens;
}


