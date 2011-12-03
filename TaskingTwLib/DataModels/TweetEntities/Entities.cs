using System.Linq;
using System.Xml.Linq;

namespace Azyobuzi.TaskingTwLib.DataModels.TweetEntities
{
    /// <summary>
    /// TweetEntities
    /// </summary>
    public class Entities
    {
        /// <summary>
        /// 画像など
        /// </summary>
        public MediaEntity[] Media { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public UrlEntity[] Urls { get; set; }

        /// <summary>
        /// 返信
        /// </summary>
        public UserMentionEntity[] UserMentions { get; set; }

        /// <summary>
        /// ハッシュタグ
        /// </summary>
        public HashtagEntity[] Hashtags { get; set; }

        /// <summary>
        /// entities要素から<see cref="T:Entities"/>を作成します。
        /// </summary>
        /// <param name="json">entities要素</param>
        /// <returns>作成された<see cref="T:Entities"/></returns>
        public static Entities Create(XElement json)
        {
            var re = new Entities();
            re.Media = json.Element("media") != null
                ? json.Element("media").Elements().Select(MediaEntity.Create).ToArray()
                : new MediaEntity[] { };
            re.Urls = json.Element("urls").Elements().Select(UrlEntity.Create).ToArray();
            re.UserMentions = json.Element("user_mentions").Elements().Select(UserMentionEntity.Create).ToArray();
            re.Hashtags = json.Element("hashtags").Elements().Select(HashtagEntity.Create).ToArray();

            return re;
        }
    }
}
