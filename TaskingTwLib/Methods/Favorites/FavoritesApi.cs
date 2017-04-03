using System.Collections.Generic;
using Azyobuzi.TaskingTwLib.DataModels;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.Methods.Favorites
{
    /// <summary>
    /// favorites
    /// </summary>
    public class FavoritesApi : ITwitterApi<Status[]>
    {
        private FavoritesApi() { }

        string ITwitterApi<Status[]>.RequestUri
        {
            get { return "https://api.twitter.com/1.1/favorites.json"; }
        }

        HttpMethodType ITwitterApi<Status[]>.MethodType
        {
            get { return HttpMethodType.GET; }
        }

        private List<FormData> parameters = new List<FormData>();
        IEnumerable<FormData> ITwitterApi<Status[]>.Parameters
        {
            get { return this.parameters; }
        }

        string ITwitterApi<Status[]>.ContentType
        {
            get { return null; }
        }

        string ITwitterApi<Status[]>.Verifier
        {
            get { return null; }
        }

        string ITwitterApi<Status[]>.Callback
        {
            get { return null; }
        }

        Status[] ITwitterApi<Status[]>.Parse(string response, Token token)
        {
            return ResponseParser.ParseStatuses(response);
        }

        /// <summary>
        /// リクエストを作成します。
        /// </summary>
        /// <param name="userId">ユーザーのID</param>
        /// <param name="screenName">ユーザーの表示名</param>
        /// <param name="count">取得する件数</param>
        /// <param name="sinceId">指定したID以降のツイートを返します。</param>
        /// <param name="page">ページ</param>
        /// <returns>リクエスト</returns>
        public static FavoritesApi Create(long? userId = null, string screenName = null, int count = 20, ulong? sinceId = null, int page = 1)
        {
            var re = new FavoritesApi();

            re.parameters.Add(new FormData("count", count.ToString()));
            re.parameters.Add(new FormData("page", page.ToString()));
            re.parameters.Add(new FormData("include_entities", "true"));

            if (userId.HasValue)
                re.parameters.Add(new FormData("user_id", userId.ToString()));

            if (!string.IsNullOrEmpty(screenName))
                re.parameters.Add(new FormData("screen_name", screenName));

            if (sinceId.HasValue)
                re.parameters.Add(new FormData("since_id", sinceId.ToString()));

            return re;
        }
    }
}
