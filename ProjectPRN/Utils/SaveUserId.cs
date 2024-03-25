using Newtonsoft.Json;

namespace ProjectPRN.Utils
{
    public static class SaveUserId
    {
      /*  private static string SessionKey = "UserId";
        public static void SetUserID(HttpContext httpContext, int userID)
        {
            httpContext.Session.SetInt32(SessionKey, userID);
        }
        
        public static int GetUserID(HttpContext httpContext)
        {
            var i = int.Parse(httpContext.Session.GetString(SessionKey));
            return i;
        }*/
        public static T GetSessionValue<T>(this HttpContext context, string sessionKey)
        {
            string value = context.Session.GetString(sessionKey);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }

        public static void AddToSession(this HttpContext context, string sessionKey, object data)
        {
            context.Session.SetString(sessionKey, JsonConvert.SerializeObject(data));
        }
    }
}
