using Newtonsoft.Json;

namespace AspNetCore.UnitOfWork.Common.Extensions
{
    public static class JSON
    {
        public static T Deserialize<T>(string objString , JsonSerializerSettings jsonSerializerSettings = null)
        {
            if(jsonSerializerSettings == null)
            {
                jsonSerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
            }

            return JsonConvert.DeserializeObject<T>(objString, jsonSerializerSettings);
        }

        public static string SerializeObject(object obj , JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (jsonSerializerSettings == null)
            {
                jsonSerializerSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
            }

            return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }
    }
}
