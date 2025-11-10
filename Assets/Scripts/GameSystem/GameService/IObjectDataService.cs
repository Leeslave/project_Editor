using System.Collections.Generic;

namespace GameService
{
    public interface IObjectDataService : IService
    {
        public List<ChatObjectData> GetNPC();

        public List<ActionObjectData> GetActionObject();

        public List<WorldVector> GetBlock();

        public List<BGMData> GetBGM();
    }
}