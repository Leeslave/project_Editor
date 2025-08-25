
using System;

namespace GameService
{
    public interface ISaveService
    {
        public event Action<int> OnRenownChanged;

        /// <summary>
        /// 명성치 조건 확인
        /// </summary>
        /// <param name="condition"></param>
        /// <returns>조건 달성 여부 반환</returns>
        public bool CheckRenown(int condition);
        
        public void AddRenown(int renown);
    }
}