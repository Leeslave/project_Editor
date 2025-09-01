using System;
using System.Collections.Generic;
using GameData;
using GameService;

namespace GameSystem
{
    public class GameSystem : Singleton<GameSystem>, ISaveService, IDayService, IWorkService
    {
        //////// Save 관리 ////////
        
        public SaveData SaveData;
        public event Action<int> OnRenownChanged;
        
        public bool CheckRenown(int condition)
        {
            if (SaveData.renown >= condition)
            {
                return true;
            }

            return false;
        }

        public void AddRenown(int renown)
        {
            SaveData.renown += renown;
            
            OnRenownChanged?.Invoke(SaveData.renown);
        }
        
        
        private DailyData _dayData;
        
        //////// 날짜 및 시간 관리 //////// 

        private int _dateIndex;
        private int _timeIndex;
  
        public event Action<int> OnDateChanged;
        public event Action<int> OnTimeChanged;
        public int GetDate()
        {
            return _dateIndex;
        }

        public int GetTime()
        {
            return _timeIndex;
        }

        public Date GetDateInfo()
        {
            return _dayData.date;
        }

        public void SetDate(int date)
        {
            DailyData newData = LoadDayData(date);
            if (newData == null) return;

            // 데이터 세이브
            SavePlayerData();
            
            _dateIndex = date;
            
            OnDateChanged?.Invoke(date);
        }

        public void SetTime(int time)
        {
            if (time < 0 || time > 3) return;

            _timeIndex = time;
            
            OnTimeChanged?.Invoke(time);
        }

        /// <summary>
        /// 해당 날짜 데이터 로드
        /// </summary>
        /// <param name="index">로드할 날짜 인덱스</param>
        public DailyData LoadDayData(int index)
        {
            // TODO: 날짜데이터 json으로부터 로드 함수 구현하기
            throw new NotImplementedException();
        }

        /// <summary>
        /// 플레이어 데이터 저장
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void SavePlayerData()
        {
            // TODO: 플레이어 세이브 함수 구현하기
            throw new NotImplementedException();
        }

        
        //////// 업무 관리 ////////
        
        public bool isScreenOn { get; set; }
        
        public event Action OnWorkClear;
        
        public List<Work> GetList()
        {
            return _dayData.workList;
        }

        public int GetStage(string workCode)
        {
            Work work = _dayData.workList.Find(w => w.code == workCode);
            if (work == null)
            {
                return -1;
            }

            return work.stage;
        }

        public void ClearWork(string workCode)
        {
            Work work = _dayData.workList.Find(w => w.code == workCode);
            if (work == null)
            {
                return;
            }
            work.isClear = true;
            
            // 전체 클리어 여부 확인
            if (IsWorkClear())
            {
                OnWorkClear?.Invoke();
            }
        }

        public bool IsWorkClear()
        {
            foreach (var w in _dayData.workList)
            {
                if (!w.isClear)
                {
                    return false;
                }
            }

            return true;
        }
    }
}