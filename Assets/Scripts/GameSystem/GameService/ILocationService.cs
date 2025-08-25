using System;

namespace GameService
{
    public interface ILocationService
    {
        public event Action<Location> OnLocationChanged;
        
        public Location GetLocation();

        public int GetPosition();
        
        public void MoveLocation(Location location, int position);
    }
}