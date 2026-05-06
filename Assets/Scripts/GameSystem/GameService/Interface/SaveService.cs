using System;
using UnityEngine;

namespace GameService
{
    public abstract class SaveService : MonoBehaviour
    {
        /// <summary>
        /// 현재 선택한 날짜 세이브 정보
        /// </summary>
        protected DaySave Save;

        public int DayIndex => Save.dayID / 100;

        /// <summary>
        /// 명성치
        /// </summary>
        public int Renown
        {
            get => Save.renown;
            set
            {
                Save.renown = value;
                OnRenownChanged?.Invoke(value);
            }
        }
        
        /// <summary>
        /// 명성치 변화 이벤트
        /// </summary>
        /// <remarks>변화된 현재 명성치 전달</remarks>
        public event Action<int> OnRenownChanged;


        protected virtual void Awake()
        {
            GameSystem.SaveService = this;
            OnAwake();
        }

        protected virtual void OnDestroy()
        {
            GameSystem.SaveService = null;
            GetDestroy();
        }

        protected virtual void OnAwake() { }
        protected virtual void GetDestroy() { }
        
        public abstract void Init();
        
        public abstract PlayerData GetPlayerData();

        /// <summary>
        /// 세이브 변경 : 날짜 변경
        /// </summary>
        /// <param name="day">변경할 날짜</param>
        /// <param name="branch">해당 브랜치</param>
        public abstract void SelectDay(int day, int branch);

        /// <summary>
        /// 날짜 전환 : 다음 날로
        /// </summary>
        public abstract void SwitchDay();
        
        /// <summary>
        /// 세이브 변경 : 분기 변경
        /// </summary>
        /// <param name="branch">변경할 분기 번호 (같은 날짜)</param>
        public abstract void SwitchBranch(int branch);
        
        /// <summary>
        /// 명성치 조건 확인
        /// </summary>
        /// <param name="condition">명성치 조건</param>
        /// <returns>조건 달성 여부 반환</returns>
        public abstract bool CheckRenown(int condition);
    }
}
