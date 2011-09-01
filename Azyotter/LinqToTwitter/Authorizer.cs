using System;
using System.Linq;
using LinqToTwitter;

namespace Azyobuzi.Azyotter.LinqToTwitter
{
    public class Authorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        public void Authorize()
        {
            throw new NotSupportedException();
        }

        public string GetAuthorizationLink(out string token)
        {
            var re = this.OAuthTwitter.AuthorizationLinkGet(this.OAuthRequestTokenUrl, this.OAuthAuthorizeUrl, "oob", false);
            token = new string(re.SkipWhile(c => c != '?').Skip(1).ToArray())
                .Split('&')
                .Select(s => s.Split('='))
                .Where(s => s.FirstOrDefault() == "oauth_token")
                .Select(s => s.LastOrDefault())
                .FirstOrDefault();
            return re;
        }

        public UserIdentifier GetAccessToken(string token, string pin)
        {
            string screenName, userId;
            this.OAuthTwitter.AccessTokenGet(token, pin, this.OAuthAccessTokenUrl, string.Empty, out screenName, out userId);
            this.Credentials.OAuthToken = this.OAuthTwitter.OAuthToken;
            this.Credentials.AccessToken = this.OAuthTwitter.OAuthTokenSecret;
            return new UserIdentifier()
            {
                ID = userId,
                UserID = userId,
                ScreenName = screenName
            };
        }
    }
}
