using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JobHub.JobSearch.Contracts
{
    public class JsonHelper
    {
        public static bool TryGet<T>(string jsonContent, string sectionName, out T value)
        {
            value = default;
            if (!jsonContent.IsNullOrWhiteSpace())
            {
                var jObject = JsonConvert.DeserializeObject<JObject>(jsonContent);
                if (jObject.TryGetValue(sectionName, out var sectionValue))
                {
                    if (typeof(T) == typeof(string) || typeof(T).IsValueType)
                    {
                        value = sectionValue.Value<T>();
                    }
                    else
                    {
                        value = JsonConvert.DeserializeObject<T>(sectionValue.ToString());
                    }
                    return true;
                }
            }

            return false;
        }
    }
}
