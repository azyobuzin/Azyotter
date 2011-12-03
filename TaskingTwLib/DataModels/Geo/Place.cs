using System.Linq;
using System.Xml.Linq;

namespace Azyobuzi.TaskingTwLib.DataModels.Geo
{
    /// <summary>
    /// 地域・場所
    /// </summary>
    public class Place
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 地名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 詳しい地名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// <see cref="Place"/>の種類
        /// </summary>
        public string PlaceType { get; set; }

        /// <summary>
        /// この<see cref="T:Place"/>の詳しい情報を取得できるAPIのURL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 地域のボーダー
        /// </summary>
        public Coordinates[] BoundingBox { get; set; }

        /// <summary>
        /// 国名
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 短い国名
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// place要素から<see cref="T:Place"/>を作成します。
        /// </summary>
        /// <param name="json">place要素</param>
        /// <returns>作成された<see cref="T:Place"/></returns>
        public static Place Create(XElement json)
        {
            var re = new Place();
            re.Id = (string)json.Element("id");
            re.Name = (string)json.Element("name");
            re.FullName = (string)json.Element("full_name");
            re.PlaceType = (string)json.Element("place_type");
            re.Url = (string)json.Element("url");
            re.Country = (string)json.Element("country");
            re.CountryCode = (string)json.Element("country_code");
            re.BoundingBox = Coordinates.CreateRange(json.Element("bounding_box")).ToArray();

            return re;
        }
    }
}
