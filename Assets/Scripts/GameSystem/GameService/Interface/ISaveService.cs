using System;

namespace GameService
{
    public interface ISaveService : IService
    {
        DaySave Save { get; }
        
        /// <summary>
        /// 명성치
        /// </summary>
        int Renown { get; }
        
        void SelectDay(int day);
        void SelectBranch(int branch);
        
        /// <summary>
        /// 명성치 변화 이벤트
        /// </summary>
        /// <remarks>변화된 현재 명성치 전달</remarks>
        event Action<int> OnRenownChanged;

        /// <summary>
        /// 명성치 조건 확인
        /// </summary>
        /// <param name="condition">명성치 조건</param>
        /// <returns>조건 달성 여부 반환</returns>
        bool CheckRenown(int condition);
        
    }
}