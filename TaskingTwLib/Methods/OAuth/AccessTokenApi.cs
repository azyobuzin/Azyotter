using System;
using System.Collections.Generic;
using System.Linq;
using Azyobuzi.TaskingTwLib.DataModels;

namespace Azyobuzi.TaskingTwLib.Methods.OAuth
{
    /// <summary>
    /// oauth/access_token
    /// </summary>
    public class AccessTokenApi : ITwitterApi<Tuple<Token, UserId>>
    {
        private AccessTokenApi() { }

        string ITwitterApi<Tuple<Token, UserId>>.RequestUri
        {
            get { return "https://api.twitter.com/oauth/access_token"; }
        }

        HttpMethodType ITwitterApi<Tuple<Token, UserId>>.MethodType
        {
            get { return HttpMethodType.POST; }
        }

        IEnumerable<FormData> ITwitterApi<Tuple<Token, UserId>>.Parameters
        {
            get { return null; }
        }

        string ITwitterApi<Tuple<Token, UserId>>.ContentType
        {
            get { return null; }
        }

        private string verifier;
        string ITwitterApi<Tuple<Token, UserId>>.Verifier
        {
            get { return this.verifier; }
        }

        string ITwitterApi<Tuple<Token, UserId>>.Callback
        {
            get { return null; }
        }

        Tuple<Token, UserId> ITwitterApi<Tuple<Token, UserId>>.Parse(string response, Token token)
        {
            var retToken = Token.Create(token.ConsumerKey, token.ConsumerSecret, response);

            var dic = response.Split('&')
                .Select(param => param.Split('='))
                .ToDictionary(param => param[0], param => param[1]);
            var retUserId = new UserId() { Id = long.Parse(dic["user_id"]), ScreenName = dic["screen_name"] };

            return Tuple.Create(retToken, retUserId);
        }

        /// <summary>
        /// oauth/access_tokenのリクエストを作成します。
        /// </summary>
        /// <param name="verifier">OAuth Verifier (PIN)</param>
        /// <returns>リクエスト</returns>
        public static AccessTokenApi Create(string verifier)
        {
            return new AccessTokenApi() { verifier = verifier };
        }
    }
}
