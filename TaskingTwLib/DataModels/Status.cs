using System;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Azyobuzi.TaskingTwLib.DataModels.Geo;
using Azyobuzi.TaskingTwLib.DataModels.TweetEntities;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.DataModels
{
    /// <summary>
    /// ツイート
    /// </summary>
    public class Status
    {
        /*
         * 注意
         * GeoはCoordinatesと同じようなので入れてない
         * 仕様変更で追加する可能性あり
         */

        /// <summary>
        /// 投稿日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// ツイートするのに使ったクライアントアプリ
        /// </summary>
        public Source Source { get; set; }

        /// <summary>
        /// 省略されているかどうか
        /// </summary>
        public bool Truncated { get; set; }

        /// <summary>
        /// お気に入りに登録済みかどうか
        /// </summary>
        public bool Favorited { get; set; }

        /// <summary>
        /// 返信先のID
        /// </summary>
        public ulong InReplyToStatusId { get; set; }

        /// <summary>
        /// 返信先のユーザーID
        /// </summary>
        public long InReplyToUserId { get; set; }

        /// <summary>
        /// 返信先の表示名
        /// </summary>
        public string InReplyToScreenName { get; set; }

        /// <summary>
        /// リツイートされた回数
        /// </summary>
        public string RetweetCount { get; set; }

        /// <summary>
        /// リツイート済みかどうか
        /// </summary>
        public bool Retweeted { get; set; }

        /// <summary>
        /// 投稿したユーザー
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// ツイートした場所（座標）
        /// </summary>
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// ツイートした場所・地域
        /// </summary>
        public Place Place { get; set; }

        /// <summary>
        /// リツイート元のツイート
        /// </summary>
        public Status RetweetedStatus { get; set; }

        /// <summary>
        /// TweetEntities
        /// </summary>
        public Entities Entities { get; set; }

        
        public ulong[] Contributors { get; set; }
        
        /// <summary>
        /// ツイートの内容を格納したJSON文字列から<see cref="T:Status"/>を作成します。
        /// </summary>
        /// <param name="json">JSON形式の文字列</param>
        /// <returns>作成された<see cref="T:Status"/></returns>
        public static Status Create(string json)
        {
            return Create(SerializationHelper.JsonToXml(json));
        }

        /// <summary>
        /// status要素から<see cref="T:Status"/>を作成します。
        /// </summary>
        /// <param name="json">status要素</param>
        /// <returns>作成された<see cref="T:Status"/></returns>
        public static Status Create(XElement json)
        {
            var re = new Status();
            re.CreatedAt = DateTimeUtil.Parse(json.Element("created_at").Value);
            re.Id = (ulong)json.Element("id");
            re.Text = HttpUtility.HtmlDecode((string)json.Element("text"));
            re.Source = Source.Create(json.Element("source"));
            re.Truncated = (bool)json.Element("truncated");
            re.Favorited = (bool)json.Element("favorited");
            re.InReplyToStatusId =
                !string.IsNullOrEmpty(json.Element("in_reply_to_status_id").Value)
                ? (ulong)json.Element("in_reply_to_status_id")
                : 0;
            re.InReplyToUserId =
                !string.IsNullOrEmpty(json.Element("in_reply_to_user_id").Value)
                ? (long)json.Element("in_reply_to_user_id")
                : 0;
            re.InReplyToScreenName = (string)json.Element("in_reply_to_screen_name");
            re.RetweetCount = (string)json.Element("retweet_count");
            re.Retweeted = (bool)json.Element("retweeted");
            re.RetweetedStatus =
                json.Element("retweeted_status") != null
                ? Create(json.Element("retweeted_status"))
                : null;

            if (json.Element("user") != null)
                re.User = User.Create(json.Element("user"));

            if (json.Element("coordinates").HasElements)
                re.Coordinates = Coordinates.Create(json.Element("coordinates"));

            if (json.Element("place").HasElements)
                re.Place = Place.Create(json.Element("place"));

            re.Entities = json.Element("entities") != null
                ? Entities.Create(json.Element("entities"))
                : null;

            if (json.Element("contributors").HasElements)
                re.Contributors = json.Element("contributors")
                    .Elements()
                    .Select(elm => (ulong)elm)
                    .ToArray();

            return re;
        }
    }
}
