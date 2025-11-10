using GameData;
using System;

namespace GameService
{
    public interface IDayService : IService
    {
        /// <summary>
        /// 날짜 변경 이벤트
        /// </summary>
        public event Action<int> OnDateChanged;

        /// <summary>
        /// 시간대 변경 이벤트
        /// </summary>
        public event Action<int> OnTimeChanged;
        
        /// <summary>
        /// 날짜 인덱스 확인
        /// </summary>
        /// <returns>현재 날짜 인덱스</returns>
        public uint GetDate();
        
        /// <summary>
        /// 시간대 확인
        /// </summary>
        /// <returns>현재 시간대</returns>
        public uint GetTime();
        
        /// <summary>
        /// 날짜 상세 정보 확인
        /// </summary>
        /// <returns>현재 날짜 정보</returns>
        public Date GetDateInfo();

        /// <summary>
        /// 날짜 설정
        /// </summary>
        /// <param name="date">변경할 날짜의 인덱스</param>
        public void SetDate(uint date);
        public void SetTime(uint time);
        
        
    }
}