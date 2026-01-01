using GameData;
using System;

namespace GameService
{
    public interface IDayService : IService
    {
        /// <summary>
        /// 날짜 변경 이벤트
        /// </summary>
        event Action<uint> OnDateChanged;

        /// <summary>
        /// 시간대 변경 이벤트
        /// </summary>
        event Action<uint> OnTimeChanged;
        
        // 날짜, 시간대 
        uint Date { get; set; }

        uint Time { get; set; }

        /// <summary>
        /// 날짜 상세 정보 확인
        /// </summary>
        /// <returns>현재 날짜 정보</returns>
        Date GetDateInfo();
    }
}