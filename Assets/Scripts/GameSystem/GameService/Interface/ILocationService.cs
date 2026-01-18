using System;
using System.Collections.Generic;

namespace GameService
{
    public interface ILocationService : IService
    {
        public WorldVector currentVector { get; set; }
        public List<WorldVector> BlockList { get; set; }

        public event Action<WorldVector> OnPosChanged;
        
        /// <summary>
        /// 월드 내 지역 이동
        /// </summary>
        public WorldVector MoveLocation(WorldVector vector);
    }
}