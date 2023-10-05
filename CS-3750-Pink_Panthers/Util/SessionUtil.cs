using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;

namespace Pink_Panthers_Project.Util
{
    public static class SessionUtil
    {
        public static void SetSessionValue<T>(this ISession session, string name, T value)
        {
            session.SetString(name, JsonSerializer.Serialize(value));
        }

        public static T? GetSessionValue<T>(this ISession session, string name)
        {
            var value = session.GetString(name);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
