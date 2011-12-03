using System;
using System.Collections.Generic;
using Azyobuzi.TaskingTwLib.DataModels;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.Methods.Tweets
{
    /// <summary>
    /// タイムライン系API
    /// </summary>
    public class TimelinesApi : ITwitterApi<Status[]>
    {
        private TimelinesApi() { }

        private string requestUri;
        string ITwitterApi<Status[]>.RequestUri
        {
            get { return this.requestUri; }
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
        /// <param name="type">タイムラインの種類</param>
        /// <param name="userId">ユーザーのID</param>
        /// <param name="screenName">ユーザーの表示名</param>
        /// <param name="count">取得する件数</param>
        /// <param name="sinceId">指定したID以降のツイートを返します。</param>
        /// <param name="maxId">指定したID以前のツイートを返します。</param>
        /// <param name="page">ページ</param>
        /// <param name="includeRts">リツイートも取得するかどうか</param>
        /// <returns>リクエスト</returns>
        public static TimelinesApi Create(TimelineType type, long? userId = null, string screenName = null, int count = 20, ulong? sinceId = null, ulong? maxId = null, int page = 1, bool includeRts = false)
        {
            var re = new TimelinesApi();

            switch (type)
            {
                case TimelineType.HomeTimeline:
                    re.requestUri = "http://api.twitter.com/1/statuses/home_timeline.json";
                    break;
                case TimelineType.Mentions:
                    re.requestUri = "http://api.twitter.com/1/statuses/mentions.json";
                    break;
                case TimelineType.PublicTimeline:
                    re.requestUri = "http://api.twitter.com/1/statuses/public_timeline.json";
                    break;
                case TimelineType.RetweetedByMe:
                    re.requestUri = "http://api.twitter.com/1/statuses/retweeted_by_me.json";
                    break;
                case TimelineType.RetweetedToMe:
                    re.requestUri = "http://api.twitter.com/1/statuses/retweeted_to_me.json";
                    break;
                case TimelineType.RetweetsOfMe:
                    re.requestUri = "http://api.twitter.com/1/statuses/retweets_of_me.json";
                    break;
                case TimelineType.UserTimeline:
                    re.requestUri = "http://api.twitter.com/1/statuses/user_timeline.json";
                    break;
                case TimelineType.RetweetedToUser:
                    re.requestUri = "http://api.twitter.com/1/statuses/retweeted_to_user.json";
                    break;
                case TimelineType.RetweetedByUser:
                    re.requestUri = "http://api.twitter.com/1/statuses/retweeted_by_user.json";
                    break;
                default:
                    throw new ArgumentException("不正なtypeです。");
            }

            re.parameters.Add(new FormData("count", count.ToString()));
            re.parameters.Add(new FormData("page", page.ToString()));
            re.parameters.Add(new FormData("include_entities", "true"));

            if (userId.HasValue)
                re.parameters.Add(new FormData("user_id", userId.ToString()));

            if (!string.IsNullOrEmpty(screenName))
                re.parameters.Add(new FormData("screen_name", screenName));

            if (sinceId.HasValue)
                re.parameters.Add(new FormData("since_id", sinceId.ToString()));

            if (maxId.HasValue)
                re.parameters.Add(new FormData("max_id", maxId.ToString()));

            if (includeRts)
                re.parameters.Add(new FormData("include_rts", "true"));

            return re;
        }
    }

    /// <summary>
    /// タイムラインの種類
    /// </summary>
    public enum TimelineType
    {
        /// <summary>
        /// ホーム
        /// </summary>
        HomeTimeline,

        /// <summary>
        /// 返信
        /// </summary>
        Mentions,

        /// <summary>
        /// パブリック
        /// </summary>
        PublicTimeline,

        /// <summary>
        /// 自分のリツイート
        /// </summary>
        RetweetedByMe,

        /// <summary>
        /// フォローしている人のリツイート
        /// </summary>
        RetweetedToMe,

        /// <summary>
        /// リツイートされた自分のツイート
        /// </summary>
        RetweetsOfMe,

        /// <summary>
        /// ユーザーのツイート
        /// </summary>
        UserTimeline,

        /// <summary>
        /// 指定したユーザーがフォローしている人のリツイート
        /// </summary>
        RetweetedToUser,

        /// <summary>
        /// 指定したユーザーのリツイート
        /// </summary>
        RetweetedByUser
    }
}
