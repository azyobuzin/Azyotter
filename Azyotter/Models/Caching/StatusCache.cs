using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azyobuzi.Azyotter.Models.TwitterDataModels;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;

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

        public Status AddOrMerge(LinqToTwitter.Status status)
        {
            throw new NotImplementedException(); //TODO:やる気でない
        }
    }
}
