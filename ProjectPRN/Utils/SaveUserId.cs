namespace ProjectPRN.Utils
{
    public static class SaveUserId
    {
        private static string SessionKey = "UserId";
        public static void SetUserID(HttpContext httpContext, string userID)
        {
            httpContext.Session.SetString(SessionKey, userID);
        }

        public static int GetUserID(HttpContext httpContext)
        {
            return int.Parse(httpContext.Session.GetString(SessionKey));
        }
    }
}
