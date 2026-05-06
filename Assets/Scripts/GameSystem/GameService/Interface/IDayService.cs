using System;

namespace GameService
{
    public interface IDayService : IService
    {
        
        int Time { get; set; }
        
        public TimeData TimeData { get; }

        /// <summary>
        /// 시간대 변경 이벤트
        /// </summary>
        event Action<int, TimeData> OnTimeChanged;

        /// <summary>
        /// 날짜 상세 정보 확인
        /// </summary>
        /// <returns>현재 날짜 정보</returns>
        public Date GetDateInfo();
    }
}
