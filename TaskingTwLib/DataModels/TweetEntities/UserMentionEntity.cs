using System.Xml.Linq;

namespace Azyobuzi.TaskingTwLib.DataModels.TweetEntities
{
    /// <summary>
    /// ユーザーへの返信を表すTweetEntity
    /// </summary>
    public class UserMentionEntity : Entity
    {
        /// <summary>
        /// ユーザーID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 表示名
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// user_mentions要素の子要素から<see cref="T:UserMentionEntity"/>を作成します。
        /// </summary>
        /// <param name="json">user_mentions要素の子要素</param>
        /// <returns>作成された<see cref="T:UserMentionEntity"/></returns>
        public static UserMentionEntity Create(XElement json)
        {
            var re = new UserMentionEntity();
            re.Id = (int)json.Element("id");
            re.ScreenName = json.Element("screen_name").Value;
            re.Name = json.Element("name").Value;
            re.Indices = EntityIndices.Create(json.Element("indices"));

            return re;
        }
    }
}
