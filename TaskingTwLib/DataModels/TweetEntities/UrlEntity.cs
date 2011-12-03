using System.Xml.Linq;

namespace Azyobuzi.TaskingTwLib.DataModels.TweetEntities
{
    /// <summary>
    /// URLを表すTweetEntity
    /// </summary>
    public class UrlEntity : Entity
    {
        /// <summary>
        /// 実際に使うURL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 表示用URL
        /// </summary>
        public string DisplayUrl { get; set; }

        /// <summary>
        /// 展開したURL
        /// </summary>
        public string ExpandedUrl { get; set; }

        /// <summary>
        /// urls要素の子要素から<see cref="T:UrlEntity"/>を作成します。
        /// </summary>
        /// <param name="json">urls要素の子要素</param>
        /// <returns>作成された<see cref="T:UrlEntity"/></returns>
        public static UrlEntity Create(XElement json)
        {
            var re = new UrlEntity();
            re.Url = json.Element("url").Value;
            re.DisplayUrl = json.Element("display_url") != null
                ? json.Element("display_url").Value
                : null;
            re.ExpandedUrl = json.Element("expanded_url") != null
                ? json.Element("expanded_url").Value
                : null;
            re.Indices = EntityIndices.Create(json.Element("indices"));

            return re;
        }
    }
}
