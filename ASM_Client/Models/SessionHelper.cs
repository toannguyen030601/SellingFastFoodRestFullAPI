using Newtonsoft.Json;

namespace ASM_Client.Models
{
    public static class SessionHelper
    {
        public static void SetObjectAsJson(this ISession session, string key, Object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
