using System.Xml.Linq;

namespace Azyobuzi.Azyotter.Models.TwitterDataModels
{
    public class Source
    {
        public string Name { get; set; }
        public string Href { get; set; }

        public static Source Create(string source)
        {
            try
            {
                var xml = XElement.Parse(source);
                return new Source()
                {
                    Name = xml.Value,
                    Href = xml.Attribute("href").Value,
                };
            }
            catch
            {
                return new Source()
                {
                    Name = source,
                    Href = "http://twitter.com/"
                };
            }
        }
    }
}
