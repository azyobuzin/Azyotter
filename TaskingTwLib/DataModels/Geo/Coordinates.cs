using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Azyobuzi.TaskingTwLib.DataModels.Geo
{
    /// <summary>
    /// 座標
    /// </summary>
    public class Coordinates
    {
        /// <summary>
        /// 緯度
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// 経度
        /// </summary>
        public double Long { get; set; }

        /// <summary>
        /// coordinates要素から<see cref="T:Coordinates"/>を作成します。
        /// </summary>
        /// <param name="json">coordinates要素</param>
        /// <returns>作成された<see cref="T:Coordinates"/></returns>
        public static Coordinates Create(XElement json)
        {
            var elm = json.Element("coordinates");

            return new Coordinates()
            {
                Lat = (double)elm.Elements().Last(),
                Long = (double)elm.Elements().First()
            };
        }

        /// <summary>
        /// bounding_box要素から<see cref="IEnumerable{Coordinates}"/>を作成します。
        /// </summary>
        /// <param name="json">bounding_box要素</param>
        /// <returns>作成された<see cref="IEnumerable{Coordinates}"/></returns>
        public static IEnumerable<Coordinates> CreateRange(XElement json)
        {
            return json.Element("coordinates")
                .Elements()
                .SelectMany(_ => _.Elements())
                .Select(_ => _.Elements())
                .Select(_ => new Coordinates()
                {
                    Lat = (double)_.Last(),
                    Long = (double)_.First()
                });
        }
    }
}
