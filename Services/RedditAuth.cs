namespace Services
{
    internal class RedditAuth
    {
        public string AppId { get; private set; }
        public string RefreshToken { get; private set; }
        public string AccessToken { get; private set; }

        public RedditAuth(string appId, string refreshToken, string accessToken)
        {
            AppId = appId;
            RefreshToken = refreshToken;
            AccessToken = accessToken;
        }
    }
}
