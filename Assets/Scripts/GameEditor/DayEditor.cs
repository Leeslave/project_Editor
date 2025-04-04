using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayEditor : MonoBehaviour
{
    /**
     * 날짜 데이터 수정 기능
     * - 날짜 정보
     * - 시작 지점
     * - 업무 목록
     * - 현재 수정중인 시간대 데이터
     *  - 시간대 정보
     *  - NPC 리스트
     *  - action 리스트
     *  - 지역 락 정보
     *  - BGM 정보
     */
    private DailyData dailyData;

    public string year {get; set;}
    public string month {get; set;}
    public string day {get; set;}

    public void SetData()
    {
        dailyData = DayBuilder.Instance.dailyData;
    }
}
