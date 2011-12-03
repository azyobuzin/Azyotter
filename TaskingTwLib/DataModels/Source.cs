using System.Web;
using System.Xml.Linq;

namespace Azyobuzi.TaskingTwLib.DataModels
{
    /// <summary>
    /// ツイートするのに使ったクライアントアプリ
    /// </summary>
    public class Source
    {
        /// <summary>
        /// アプリケーション名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// アプリケーションの説明サイトのURL（とは限らない）
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// source要素から<see cref="T:Source"/>を作成します。
        /// </summary>
        /// <param name="json">source要素</param>
        /// <returns>作成された<see cref="T:Source"/></returns>
        public static Source Create(XElement json)
        {
            try
            {
                var xml = XElement.Parse(json.Value);
                return new Source()
                {
                    Name = HttpUtility.HtmlDecode(xml.Value),
                    Href = xml.Attribute("href").Value,
                };
            }
            catch
            {
                return new Source()
                {
                    Name = (string)json,
                    Href = "http://twitter.com/"
                };
            }
        }
    }
}
