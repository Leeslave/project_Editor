
using System.Collections.Generic;
using GameData;
using System;

namespace GameService
{
    public interface IDayService
    {
        /** 게임 데이터 제공 인터페이스
         * 하루의 날짜 정보 제공
        */

        // 날짜 인덱스 반환
        public int GetDateIndex();
        
        // 시간대 인덱스 반환
        public int GetTimeIndex ();
        
        // 날짜 정보 반환
        public Date GetDateInfo();

        // 날짜 변경
        public event Action<int> OnDateChanged;
        public void SetDate(int date);
        
        // 시간대 변경
        public event Action<int> OnTimeChanged;
        public void SetTime(int time);
        
        
    }
}