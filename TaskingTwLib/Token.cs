using System.Linq;

namespace Azyobuzi.TaskingTwLib
{
    /// <summary>
    /// OAuthトークン
    /// </summary>
    public class Token
    {
        /// <summary>
        /// クライアント識別子
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// クライアント共有鍵
        /// </summary>
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// トークン識別子
        /// </summary>
        public string OAuthToken { get; set; }

        /// <summary>
        /// トークン共有鍵
        /// </summary>
        public string OAuthTokenSecret { get; set; }

        /// <summary>
        /// application/x-www-form-urlencoded形式の文字列から<see cref="T:Token"/>を作成します。
        /// </summary>
        /// <param name="consumerKey">クライアント識別子</param>
        /// <param name="consumerSecret">クライアント共有鍵</param>
        /// <param name="www_form_urlencoded">トークンが格納されたapplication/x-www-form-urlencoded形式の文字列</param>
        /// <returns>作成された<see cref="T:Token"/></returns>
        public static Token Create(string consumerKey, string consumerSecret, string www_form_urlencoded)
        {
            var dic = www_form_urlencoded.Split('&')
                .Select(param => param.Split('='))
                .ToDictionary(param => param[0], param => param[1]);
            return new Token()
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                OAuthToken = dic["oauth_token"],
                OAuthTokenSecret = dic["oauth_token_secret"]
            };
        }
    }
}
