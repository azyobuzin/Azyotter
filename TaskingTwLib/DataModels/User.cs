using System;
using System.Web;
using System.Xml.Linq;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.DataModels
{
    /// <summary>
    /// ユーザー
    /// </summary>
    public class User : UserId
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 場所
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 自己紹介
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// アイコン
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// アイコン（SSL）
        /// </summary>
        public string ProfileImageUrlHttps { get; set; }

        /// <summary>
        /// ユーザー指定のウェブサイト
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 非公開かどうか
        /// </summary>
        public bool Protected { get; set; }

        /// <summary>
        /// フォロワー数
        /// </summary>
        public int FollowersCount { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public string ProfileBackgroundColor { get; set; }

        /// <summary>
        /// 文字色
        /// </summary>
        public string ProfileTextColor { get; set; }

        /// <summary>
        /// リンク文字色
        /// </summary>
        public string ProfileLinkColor { get; set; }

        /// <summary>
        /// サイドバーの背景色
        /// </summary>
        public string ProfileSidebarFillColor { get; set; }

        /// <summary>
        /// サイドバーの輪郭
        /// </summary>
        public string ProfileSidebarBorderColor { get; set; }

        /// <summary>
        /// フォロー数
        /// </summary>
        public int FriendsCount { get; set; }

        /// <summary>
        /// アカウント作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// お気に入りツイート数
        /// </summary>
        public int FavouritesCount { get; set; }

        /// <summary>
        /// ユーザが指定したタイムゾーンの UTC (世界協定時刻) からのオフセット
        /// </summary>
        public int UtcOffset { get; set; }

        /// <summary>
        /// タイムゾーン
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// 背景画像
        /// </summary>
        public string ProfileBackgroundImageUrl { get; set; }

        /// <summary>
        /// 背景画像（SSL）
        /// </summary>
        public string ProfileBackgroundImageUrlHttps { get; set; }

        /// <summary>
        /// 背景画像をタイルするかどうか
        /// </summary>
        public bool ProfileBackgroundTile { get; set; }

        /// <summary>
        /// 背景画像を使うかどうか
        /// </summary>
        public bool ProfileUseBackgroundImage { get; set; }

        /// <summary>
        /// デバイス通知が有効かどうか
        /// </summary>
        public bool Notifications { get; set; }

        /// <summary>
        /// 位置情報設定が有効かどうか
        /// </summary>
        public bool GeoEnabled { get; set; }

        /// <summary>
        /// 認証済みアカウントかどうか
        /// </summary>
        public bool Verified { get; set; }

        /// <summary>
        /// このユーザーをフォローしているかどうか
        /// </summary>
        public bool Following { get; set; }

        /// <summary>
        /// ツイート数
        /// </summary>
        public int StatusesCount { get; set; }

        /// <summary>
        /// ユーザーの言語
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// ライター機能を使用しているかどうか
        /// </summary>
        public bool ContributorsEnabled { get; set; }

        /// <summary>
        /// フォローリクエストを送ったかどうか
        /// </summary>
        public bool FollowRequestSent { get; set; }

        /// <summary>
        /// リストに登録されてる数
        /// </summary>
        public int ListedCount { get; set; }

         /// <summary>
         /// 外部コンテンツ表示設定ですべて表示するようになっているかどうか
         /// </summary>
        public bool ShowAllInlineMedia { get; set; }


        public bool DefaultProfile { get; set; }


        public bool DefaultProfileImage { get; set; }

        /// <summary>
        /// Twitterの翻訳者かどうか
        /// </summary>
        public bool IsTranslator { get; set; }

        /// <summary>
        /// 最新のツイート
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// ユーザーの内容を格納したJSON文字列から<see cref="T:User"/>を作成します。
        /// </summary>
        /// <param name="json">JSON形式の文字列</param>
        /// <returns>作成された<see cref="T:User"/></returns>
        public static User Create(string json)
        {
            return Create(SerializationHelper.JsonToXml(json));
        }
        
        /// <summary>
        /// user要素から<see cref="T:User"/>を作成します。
        /// </summary>
        /// <param name="json">user要素</param>
        /// <returns>作成された<see cref="T:User"/></returns>
        public static User Create(XElement json)
        {
            var re = new User();
            re.Id = (long)json.Element("id");
            re.Name = HttpUtility.HtmlDecode((string)json.Element("name"));
            re.ScreenName = (string)json.Element("screen_name");
            re.Location = HttpUtility.HtmlDecode((string)json.Element("location"));
            re.Description = HttpUtility.HtmlDecode((string)json.Element("description"));
            re.ProfileImageUrl = (string)json.Element("profile_image_url");
            re.ProfileImageUrlHttps = (string)json.Element("profile_image_url_https");
            re.Url = (string)json.Element("url");
            re.Protected = (bool)json.Element("protected");
            re.FollowersCount = (int)json.Element("followers_count");
            re.ProfileBackgroundColor = (string)json.Element("profile_background_color");
            re.ProfileTextColor = (string)json.Element("profile_text_color");
            re.ProfileLinkColor = (string)json.Element("profile_link_color");
            re.ProfileSidebarFillColor = (string)json.Element("profile_sidebar_fill_color");
            re.ProfileSidebarBorderColor = (string)json.Element("profile_sidebar_border_color");
            re.FriendsCount = (int)json.Element("friends_count");
            re.FavouritesCount = (int)json.Element("favourites_count");
            if (!string.IsNullOrEmpty(json.Element("utc_offset").Value))
                re.UtcOffset = (int)json.Element("utc_offset");
            re.TimeZone = json.Element("time_zone").Value;
            re.ProfileBackgroundImageUrl = (string)json.Element("profile_background_image_url");
            re.ProfileBackgroundImageUrlHttps = (string)json.Element("profile_background_image_url_https");
            re.ProfileBackgroundTile = (bool)json.Element("profile_background_tile");
            re.ProfileUseBackgroundImage = (bool)json.Element("profile_use_background_image");
            if (!string.IsNullOrEmpty(json.Element("notifications").Value))
                re.Notifications = (bool)json.Element("notifications");
            re.GeoEnabled = (bool)json.Element("geo_enabled");
            re.Verified = (bool)json.Element("verified");
            if (!string.IsNullOrEmpty(json.Element("following").Value))
                re.Following = (bool)json.Element("following");
            re.StatusesCount = (int)json.Element("statuses_count");
            re.Lang = (string)json.Element("lang");
            re.ContributorsEnabled = (bool)json.Element("contributors_enabled");
            if (!string.IsNullOrEmpty(json.Element("follow_request_sent").Value))
                re.FollowRequestSent = (bool)json.Element("follow_request_sent");
            re.ListedCount = (int)json.Element("listed_count");
            re.ShowAllInlineMedia = (bool)json.Element("show_all_inline_media");
            re.DefaultProfile = (bool)json.Element("default_profile");
            re.DefaultProfileImage = (bool)json.Element("default_profile_image");
            re.IsTranslator = (bool)json.Element("is_translator");
            re.CreatedAt = DateTimeUtil.Parse(json.Element("created_at").Value);

            if (json.Element("status") != null)
                re.Status = Status.Create(json.Element("status"));

            return re;
        }
    }
}
