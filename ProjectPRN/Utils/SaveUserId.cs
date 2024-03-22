namespace ProjectPRN.Utils
{
    public static class SaveUserId
    {
        private static string SessionKey = "UserId";
        public static void SetUserID(HttpContext httpContext, string userID)
        {
            httpContext.Session.SetString(SessionKey, userID);
        }

        public static string GetUserID(HttpContext httpContext)
        {
            return httpContext.Session.GetString(SessionKey);
        }
    }
}