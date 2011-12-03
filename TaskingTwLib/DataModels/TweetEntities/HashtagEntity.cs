using System.Xml.Linq;

namespace Azyobuzi.TaskingTwLib.DataModels.TweetEntities
{
    /// <summary>
    /// ハッシュタグを表すTweetEntity
    /// </summary>
    public class HashtagEntity : Entity
    {
        /// <summary>
        /// ハッシュタグ（「#」は省かれます）
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// hashtags要素の子要素から<see cref="T:HashtagEntity"/>を作成します。
        /// </summary>
        /// <param name="json">hashtags要素の子要素</param>
        /// <returns>作成された<see cref="T:HashtagEntity"/></returns>
        public static HashtagEntity Create(XElement json)
        {
            var re = new HashtagEntity();
            re.Text = json.Element("text").Value;
            re.Indices = EntityIndices.Create(json.Element("indices"));

            return re;
        }
    }
}
