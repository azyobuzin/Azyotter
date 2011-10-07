﻿using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using Azyobuzi.Azyotter.Models.TwitterDataModels;

namespace Azyobuzi.Azyotter.Models.Caching
{
    public class StatusCache
        : IEnumerable<ITimelineItem>, INotifyCollectionChanged
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

        private ConcurrentDictionary<ulong, ITimelineItem> collection = new ConcurrentDictionary<ulong, ITimelineItem>();

        public IEnumerator<ITimelineItem> GetEnumerator()
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

        public ITimelineItem this[ulong id]
        {
            get
            {
                return this.collection[id];
            }
        }

        public bool ContainsId(ulong id)
        {
            return this.collection.ContainsKey(id);
        }

        public bool Remove(ulong id)
        {
            ITimelineItem tmp;
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

        public ITimelineItem AddOrMerge(TaskingTwLib.DataModels.Status status, bool forAllTab)
        {
            ITimelineItem target;

            bool isNew = false;
            if (!this.collection.TryGetValue(status.Id, out target))
            {
                target = this.collection.AddOrUpdate(status.Id, new Status(), (k, v) => v);
                isNew = true;
            }

            target.ForAllTab = forAllTab || target.ForAllTab;
            target.Id = status.Id;
            target.CreatedAt = status.CreatedAt.ToLocalTime();
            target.Text = this.CreateStatusText(status);
            target.From = UserCache.Instance.AddOrMerge(status.User);
            target.InReplyToStatusId = status.InReplyToStatusId;
            target.Source = status.Source;

            if (isNew)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, target));

            return target;
        }

        private IEnumerable<StatusTextParts.StatusTextPartBase> CreateStatusText(TaskingTwLib.DataModels.Status status)
        {
            //if (status.Entities == null)
                return new[] { new StatusTextParts.Normal() { Text = HttpUtility.HtmlDecode(status.Text) } };

            //var re = new List<StatusTextParts.StatusTextPartBase>();

            //IEnumerable<LinqToTwitter.MentionBase> entities = status.Entities.UrlMentions;
            //using (var sr = new StringReader(HttpUtility.HtmlDecode(status.Text)))
            //{
            //    int i = 0;
            //    string buffer = string.Empty;
            //    while (sr.Peek() != -1)
            //    {
            //        var sEntity = entities.FirstOrDefault(_ => _.Start == i);
            //        if (sEntity == null)
            //        {
            //            buffer += (char)sr.Read();
            //            i++;
            //        }
            //        else
            //        {
            //            re.Add(new StatusTextParts.Normal() { Text = buffer });
            //            buffer = string.Empty;
            //            var urlEntity = sEntity as LinqToTwitter.UrlMention;
            //            string urlBuffer = string.Empty;
            //            while (i != urlEntity.End)
            //            {
            //                urlBuffer += (char)sr.Read();
            //                i++;
            //            }
            //            re.Add(new StatusTextParts.Url() { Text = urlBuffer });
            //        }
            //    }
            //    re.Add(new StatusTextParts.Normal() { Text = buffer });
            //    buffer = string.Empty;
            //}

            //return re;
        }
    }
}
