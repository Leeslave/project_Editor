using System;
using System.Collections.Generic;

namespace GameService
{
    public interface IDataService : IService
    {
        #region DateInfo
        
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
        DailyData LoadDay(int dateIndex = -1);

        /// <summary>
        /// 오늘 날짜의 업무 불러오기
        /// </summary>
        List<Work> GetWorkList();
        
        #endregion

        #region SaveInfo

        /// <summary>
        /// 세이브 데이터 정보
        /// </summary>
        SaveData Save
        {
            get;
        }

        DaySave GetDaySave(int dateIndex = -1);

        #endregion
    }
}