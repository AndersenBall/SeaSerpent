using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GerneralScripts.Utils
{
    public class JsonDeepCopy
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None,
            ContractResolver = new DefaultContractResolver
            {
                DefaultMembersSearchFlags = BindingFlags.NonPublic |
                                            BindingFlags.Public |
                                            BindingFlags.Instance
            }
        };

        public static T Clone<T>(T source)
        {
            if (source == null) return default;
            var json = JsonConvert.SerializeObject(source, Settings);
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }
    }
}