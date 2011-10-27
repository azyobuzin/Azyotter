using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Azyobuzi.Azyotter.Util;

namespace Azyobuzi.Azyotter.Updater
{
    public class Updates : IEnumerable<Update>
    {
        private Updates(List<Update> updates)
        {
            this.updates = updates;
        }

        private List<Update> updates;

        public IEnumerator<Update> GetEnumerator()
        {
            return this.updates.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Update Latest()
        {
            return this.updates
                .OrderByDescending(update => update.Version)
                .FirstOrDefault();
        }

        public Update PackageId(string id)
        {
            return this.updates
                .FirstOrDefault(update => update.Id == id);
        }

        public static Updates GetUpdates(string uri = "http://www.azyobuzi.net/projects/azyotter/updates/updates1.xml")
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.UserAgent, "Azyotter.Updater v" + AssemblyUtil.GetInformationalVersion());

                return new Updates(
                    XElement.Parse(wc.DownloadString(uri))
                        .Elements("update")
                        .Select(elm => new Update()
                        {
                            Id = (string)elm.Attribute("id"),
                            Version = Version.Parse((string)elm.Attribute("version")),
                            Uri = new Uri((string)elm.Attribute("uri"))
                        })
                        .ToList()
                );
            }
        }
    }

    public class Update
    {
        public string Id { get; set; }
        public Version Version { get; set; }
        public Uri Uri { get; set; }
    }
}
