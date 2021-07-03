using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;

namespace DetaCSharp.Utils
{
    public static class ObjectToDictionaryConverter
    {

        public static IDictionary<string, object> ToJsonNammingDictionaryReflection<T>(T source, JsonNamingPolicy namingPolicy)
        {
            return typeof(T).GetProperties().ToDictionary(x => namingPolicy.ConvertName(x.Name), x => x.GetValue(source, null));
        }

        public static IDictionary<string, object> ToDictionaryWithTypeDescriptor(object data, JsonNamingPolicy namingPolicy)
        {
            var dictionary = new JsonNamingPolicyDictionary(namingPolicy);
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(data))
            {
                dictionary.Add(property.Name, property.GetValue(data));
            }

            return dictionary;
        }

        public static IDictionary<string, object> ToDictionaryWithSerialize(object data, JsonSerializerOptions serializerOptions)
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(data, serializerOptions), serializerOptions);
        }
    }
}
