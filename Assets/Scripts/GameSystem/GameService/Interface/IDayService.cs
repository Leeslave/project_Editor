using System;

namespace GameService
{
    public interface IDayService : IService
    {
        public DailyData Data { get; }
        public TimeData TimeData { get; }
        
        /// <summary>
        /// 날짜 변경 이벤트
        /// </summary>
        event Action<int> OnDateChanged;

        /// <summary>
        /// 시간대 변경 이벤트
        /// </summary>
        event Action<int> OnTimeChanged;
        
        // 날짜, 시간대 
        int Date { get; set; }

        int Time { get; set; }

        /// <summary>
        /// 날짜 상세 정보 확인
        /// </summary>
        /// <returns>현재 날짜 정보</returns>
        public Date GetDateInfo();
        
        /// <summary>
        /// 현재 날짜 시작지점 확인
        /// </summary>
        /// <returns>시작 위치 벡터</returns>
        public WorldVector GetStartLocation();
    }
}