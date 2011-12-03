using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Azyobuzi.TaskingTwLib.Util
{
    /// <summary>
    /// JSONのパース関連
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// JSON文字列から<see cref="XElement"/>を作成します。
        /// </summary>
        /// <param name="json">JSON形式の文字列</param>
        /// <returns>作成された<see cref="XElement"/></returns>
        public static XElement JsonToXml(string json)
        {
            using (var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(json), XmlDictionaryReaderQuotas.Max))
                return XElement.Load(reader);
        }
    }
}
