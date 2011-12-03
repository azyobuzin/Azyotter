using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Azyobuzi.TaskingTwLib.DataModels.TweetEntities
{
    /// <summary>
    /// <see cref="T:MediaEntity"/>の画像サイズ
    /// </summary>
    public class PhotoSize
    {
        /// <summary>
        /// 幅
        /// </summary>
        public int W { get; set; }

        /// <summary>
        /// 高さ
        /// </summary>
        public int H { get; set; }

        /// <summary>
        /// リサイズ方法
        /// <para><c>"fit"</c> or <c>"crop"</c></para>
        /// </summary>
        public string Resize { get; set; }

        /// <summary>
        /// sizes要素から<see cref="Dictionary{String, PhotoSize}"/>を作成します。
        /// </summary>
        /// <param name="json">sizes要素</param>
        /// <returns>作成された<see cref="Dictionary{String, PhotoSize}"/></returns>
        public static Dictionary<string, PhotoSize> CreateRange(XElement json)
        {
            return json.Elements()
                .ToDictionary(_ => _.Name.LocalName, _ => new PhotoSize()
                {
                    W = (int)_.Element("w"),
                    H = (int)_.Element("h"),
                    Resize = _.Element("resize").Value
                });
        }
    }
}
