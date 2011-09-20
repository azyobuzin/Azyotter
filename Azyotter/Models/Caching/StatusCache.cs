using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using Azyobuzi.Azyotter.Models.TwitterDataModels;
using System.IO;
using System.Linq;

namespace Azyobuzi.Azyotter.Models.Caching
{
    public class StatusCache
        : IEnumerable<Status>, INotifyCollectionChanged
    {
        private StatusCache() { }

        private static StatusCache instance = new StatusCache();
        public static StatusCache Instance
        {
            get
            {
                return instance;
            }
        }

        private ConcurrentDictionary<string, Status> collection = new ConcurrentDictionary<string, Status>();

        public IEnumerator<Status> GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, e);
        }

        public Status this[string id]
        {
            get
            {
                return this.collection[id];
            }
        }

        public bool ContainsId(string id)
        {
            return this.collection.ContainsKey(id);
        }

        public bool Remove(string id)
        {
            Status tmp;
            bool result = this.collection.TryRemove(id, out tmp);

            if (result)
            {
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        tmp
                    )
                );
            }

            return result;
        }

        public Status AddOrMerge(LinqToTwitter.Status status, bool forAllTab)
        {
            Status target;

            if (!this.collection.TryGetValue(status.StatusID, out target))
            {
                target = this.collection.AddOrUpdate(status.StatusID, new Status(), (k, v) => v);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, target));
            }

            target.ForAllTab = forAllTab || target.ForAllTab;
            target.Id = status.StatusID;
            target.CreatedAt = status.CreatedAt.ToLocalTime();
            target.Text = this.CreateStatusText(status);
            target.From = UserCache.Instance.AddOrMerge(status.User);
            target.InReplyToStatusId = status.InReplyToStatusID;
            target.Source = Source.Create(status.Source);

            return target;
        }

        private IEnumerable<StatusTextParts.StatusTextPartBase> CreateStatusText(LinqToTwitter.Status status)
        {
            if (status.Entities == null)
                return new[] { new StatusTextParts.Normal() { Text = status.Text } };

            var re = new List<StatusTextParts.StatusTextPartBase>();

            IEnumerable<LinqToTwitter.MentionBase> entities = status.Entities.UrlMentions;
            using (var sr = new StringReader(status.Text))
            {
                int i = 0;
                string buffer = string.Empty;
                while (sr.Peek() != -1)
                {
                    var sEntity = entities.FirstOrDefault(_ => _.Start == i);
                    if (sEntity == null)
                    {
                        buffer += (char)sr.Read();
                        i++;
                    }
                    else
                    {
                        re.Add(new StatusTextParts.Normal() { Text = buffer });
                        buffer = string.Empty;
                        var urlEntity = sEntity as LinqToTwitter.UrlMention;
                        string urlBuffer = string.Empty;
                        while (i != urlEntity.End)
                        {
                            urlBuffer += (char)sr.Read();
                            i++;
                        }
                        re.Add(new StatusTextParts.Url() { Text = urlBuffer });
                    }
                }
                re.Add(new StatusTextParts.Normal() { Text = buffer });
                buffer = string.Empty;
            }

            return re;
        }
    }
}
