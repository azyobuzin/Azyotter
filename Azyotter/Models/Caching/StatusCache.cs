using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using Azyobuzi.Azyotter.Models.TwitterDataModels;

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
            target.CreatedAt = status.CreatedAt;
            target.Text = this.CreateStatusText(status);
            target.From = UserCache.Instance.AddOrMerge(status.User);
            target.InReplyToStatusId = status.InReplyToStatusID;
            target.Source = Source.Create(status.Source);

            return target;
        }

        private IEnumerable<StatusTextParts.StatusTextPartBase> CreateStatusText(LinqToTwitter.Status status)
        {
            //LinqToTwitterがもう少し安定してから実装する
            return new[] { new StatusTextParts.Normal() { Text = status.Text } };
        }
    }
}
