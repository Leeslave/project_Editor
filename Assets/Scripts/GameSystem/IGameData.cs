
using DataType;

namespace GameData
{
    public interface IGameRepo
    {
        /** 게임 데이터 제공 인터페이스
         * : 게임 데이터 테이블 기능
         * - 날짜 정보, 날짜별 데이터 제공 (NPC, 오브젝트, 시작 위치, 시간대 정보)
         * - 업무 정보 (work, 할 일)
        */ 
        
        public DailyData DayData { get; set; }    // 오늘 날짜 데이터

        public DailyData GetData(int index)
        {
            
        }
    }
}