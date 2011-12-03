using System.Collections.Generic;
using Azyobuzi.TaskingTwLib.DataModels;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.Methods.Favorites
{
    /// <summary>
    /// favorites/create、favorites/destroy
    /// </summary>
    public class FavoriteOperationApi : ITwitterApi<Status>
    {
        private FavoriteOperationApi() { }

        private string requestUri;
        string ITwitterApi<Status>.RequestUri
        {
            get { return this.requestUri; }
        }

        HttpMethodType ITwitterApi<Status>.MethodType
        {
            get { return HttpMethodType.POST; }
        }

        private List<FormData> parameters = new List<FormData>();
        IEnumerable<FormData> ITwitterApi<Status>.Parameters
        {
            get { return this.parameters; }
        }

        string ITwitterApi<Status>.ContentType
        {
            get { return HttpContentType.ApplicationXWwwFormUrlencoded; }
        }

        string ITwitterApi<Status>.Verifier
        {
            get { return null; }
        }

        string ITwitterApi<Status>.Callback
        {
            get { return null; }
        }

        Status ITwitterApi<Status>.Parse(string response, Token token)
        {
            return ResponseParser.ParseStatus(response);
        }

        /// <summary>
        /// リクエストを作成します。
        /// </summary>
        /// <param name="type">お気に入り操作APIの種類</param>
        /// <param name="id">ツイートID</param>
        /// <returns>リクエスト</returns>
        public static FavoriteOperationApi Create(FavoriteOperationApiType type, ulong id)
        {
            var re = new FavoriteOperationApi();
            re.requestUri = string.Format("http://api.twitter.com/1/favorites/{0}/{1}.json", type.ToString().ToLower(), id);
            re.parameters.Add(new FormData("id", id.ToString()));
            re.parameters.Add(new FormData("include_entities", "true"));

            return re;
        }
    }

    /// <summary>
    /// お気に入り操作APIの種類
    /// </summary>
    public enum FavoriteOperationApiType
    {
        /// <summary>
        /// 作成
        /// </summary>
        Create,
        /// <summary>
        /// 破棄
        /// </summary>
        Destroy
    }
}
