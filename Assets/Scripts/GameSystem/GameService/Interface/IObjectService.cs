using System.Collections.Generic;

namespace GameService
{
    public interface IObjectService : IService
    {
        public List<ChatObjectData> GetNPC();

        public List<ActionObjectData> GetActionObject();

        public List<BGMData> GetBGM();
    }
}