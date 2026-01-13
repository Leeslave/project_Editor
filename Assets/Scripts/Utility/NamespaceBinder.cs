using Newtonsoft.Json.Serialization;
using System;

namespace Utility
{
    public class NamespaceBinder : ISerializationBinder
    {
        public Type BindToType(string assemblyName, string typeName)
        {
            // JSON Type 지정자
            if (typeName == "DailyData")            // DailyData : GameData 네임스페이스
            {
                return typeof(GameData.DailyData);
            }

            return Type.GetType($"{typeName}, {assemblyName}") ?? throw new InvalidOperationException();
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.FullName;
        }
    }
}