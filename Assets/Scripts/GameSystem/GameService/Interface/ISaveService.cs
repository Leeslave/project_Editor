using System;

namespace GameService
{
    public interface ISaveService : IService
    {
        /// <summary>
        /// 명성치
        /// </summary>
        public int Renown { get; set; }
        
        /// <summary>
        /// 명성치 변화 이벤트
        /// </summary>
        /// <remarks>변화된 현재 명성치 전달</remarks>
        public event Action<int> OnRenownChanged;

        /// <summary>
        /// 명성치 조건 확인
        /// </summary>
        /// <param name="condition">명성치 조건</param>
        /// <returns>조건 달성 여부 반환</returns>
        public bool CheckRenown(int condition);
        
    }
}