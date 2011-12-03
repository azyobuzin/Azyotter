using System.Collections.Generic;
using System.Xml.Linq;

namespace Azyobuzi.TaskingTwLib.DataModels.TweetEntities
{
    /// <summary>
    /// 画像などを表すTweetEntity
    /// </summary>
    public class MediaEntity : UrlEntity
    {
        /// <summary>
        /// メディアID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// メディアファイルのURL
        /// </summary>
        public string MediaUrl { get; set; }

        /// <summary>
        /// メディアファイルのURL（SSL）
        /// </summary>
        public string MediaUrlHttps { get; set; }

        /// <summary>
        /// サポートしている画像サイズ
        /// </summary>
        public Dictionary<string, PhotoSize> Sizes { get; set; }

        /// <summary>
        /// メディアの種類
        /// <para>現在は<c>"photo"</c>のみ</para>
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// media要素の子要素から<see cref="T:MediaEntity"/>を作成します。
        /// </summary>
        /// <param name="json">media要素の子要素</param>
        /// <returns>作成された<see cref="T:MediaEntity"/></returns>
        public static new MediaEntity Create(XElement json)
        {
            var re = new MediaEntity();
            re.Id = (long)json.Element("id");
            re.MediaUrl = json.Element("media_url").Value;
            re.MediaUrlHttps = json.Element("media_url_https").Value;
            re.Url = json.Element("url").Value;
            re.DisplayUrl = json.Element("display_url") != null
                ? json.Element("display_url").Value
                : null;
            re.ExpandedUrl = json.Element("expanded_url") != null
                ? json.Element("expanded_url").Value
                : null;
            re.Sizes = PhotoSize.CreateRange(json.Element("sizes"));
            re.Type = json.Element("type").Value;
            re.Indices = EntityIndices.Create(json.Element("indices"));

            return re;
        }
    }
}
