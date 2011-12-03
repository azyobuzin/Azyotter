using System.Collections.Generic;

namespace Azyobuzi.TaskingTwLib.Methods.OAuth
{
    /// <summary>
    /// oauth/request_token
    /// </summary>
    public class RequestTokenApi : ITwitterApi<Token>
    {
        private RequestTokenApi() { }

        string ITwitterApi<Token>.RequestUri
        {
            get { return "https://api.twitter.com/oauth/request_token"; }
        }

        HttpMethodType ITwitterApi<Token>.MethodType
        {
            get { return HttpMethodType.POST; }
        }

        IEnumerable<FormData> ITwitterApi<Token>.Parameters
        {
            get { return null; }
        }

        string ITwitterApi<Token>.ContentType
        {
            get { return null; }
        }

        string ITwitterApi<Token>.Verifier
        {
            get { return null; }
        }

        private string callback;
        string ITwitterApi<Token>.Callback
        {
            get { return this.callback; }
        }

        Token ITwitterApi<Token>.Parse(string response, Token token)
        {
            return Token.Create(token.ConsumerKey, token.ConsumerSecret, response);
        }

        /// <summary>
        /// oauth/request_tokenのリクエストを作成します。
        /// </summary>
        /// <param name="callback">コールバック</param>
        /// <returns>リクエスト</returns>
        public static RequestTokenApi Create(string callback = "oob")
        {
            return new RequestTokenApi() { callback = callback };
        }
    }
}
