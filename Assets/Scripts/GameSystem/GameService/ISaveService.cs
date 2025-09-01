
using System;

namespace GameService
{
    public interface ISaveService
    {
        public event Action<int> OnRenownChanged;

        /// <summary>
        /// 명성치 조건 확인
        /// </summary>
        /// <param name="condition">명성치 조건</param>
        /// <returns>조건 달성 여부 반환</returns>
        public bool CheckRenown(int condition);
        
        /// <summary>
        /// 명성치 변화 적용
        /// </summary>
        /// <param name="renown">변화할 명성치</param>
        public void AddRenown(int renown);
    }
}