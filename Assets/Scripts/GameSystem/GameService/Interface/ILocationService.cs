using System;

namespace GameService
{
    public interface ILocationService : IService
    {
        WorldVector CurrentPosition { get; }
        
        event Action<WorldVector> OnPosChanged;
        
        void MoveLocation(WorldVector vector);
        
        bool IsBlocked(WorldVector vector);
    }
}
