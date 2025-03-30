using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBuilder : MonoBehaviour
{
    /**
     * 게임 데이터 빌드 시스템
     * : 별도의 Editor로 조작 예정
     * - 폴더를 통해 현재 저장되어있는 데이터 확인 가능
     * - 특정 파일을 선택하여 수정/생성
     *      1. DailyData
     *      2. ChatData : List<Paragraph>
     */
    
    private string inputText = "";
    
    public List<DailyData> dailyData = new List<DailyData>();

    void Awake()
    {
        LoatDayList();
    }


    public void LoatDayList()
    {
        // 폴더 상의 게임 데이터 로드
        
    }


    public void LoadChatList()
    {
        
    }
}