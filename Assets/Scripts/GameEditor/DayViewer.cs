using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayViewer : MonoBehaviour
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

    [Header("UI 연결")] 
    public TMP_InputField yearField;
    public TMP_InputField monthField;
    public TMP_InputField dayField;
    
    
    private DailyData data;

    public string year {get; set;}
    public string month {get; set;}
    public string day {get; set;}


    public void StartEditing()
    {
        
    }

    private void SetData()
    {
        data = DayBuilder.Instance.dailyData;
    }
}
