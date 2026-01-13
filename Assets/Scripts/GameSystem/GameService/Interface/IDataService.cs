using System;
using System.Collections.Generic;

namespace GameService
{
    public interface IDataService : IService
    {
        /// <summary>
        /// 게임 데이터 정보
        /// </summary>
        DailyData Data 
        {
            get;
        }

        event Action<int> OnDataChanged;

        /// <summary>
        /// 데이터 인덱스 설정 및 데이터 로드
        /// </summary>
        void LoadDay(int dateIndex = -1);

        /// <summary>
        /// 오늘 날짜 세부 정보 불러오기
        /// </summary>
        Date GetDateInfo();
        
        /// <summary>
        /// 오늘 시작 좌표 정보 불러오기
        /// </summary>
        WorldVector GetStartLocation();
                
        /// <summary>
        /// 오늘 날짜 시간대별 정보 불러오기
        /// </summary>
        /// <returns></returns>
        List<TimeData> GetTimeData();

        /// <summary>
        /// 오늘 날짜의 업무 불러오기
        /// </summary>
        List<Work> GetWorkList();

    }
}