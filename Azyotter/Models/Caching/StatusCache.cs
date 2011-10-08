using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
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

            if (status.Entities != null)
            {
                target.ImageThumbnails = status.Entities.Media
                    .Select(media => media.MediaUrl + ":thumb")
                    .ToArray();
            }

            if (isNew)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, target));

            return target;
        }

        private IEnumerable<StatusTextParts.StatusTextPartBase> CreateStatusText(TaskingTwLib.DataModels.Status status)
        {
            if (status.Entities == null)
                return new[] { new StatusTextParts.Normal() { Text = HttpUtility.HtmlDecode(status.Text) } };

            var re = new List<StatusTextParts.StatusTextPartBase>();

            var entities = status.Entities.Media
                .Cast<TaskingTwLib.DataModels.TweetEntities.Entity>()
                .Concat(status.Entities.Urls)
                .Concat(status.Entities.UserMentions)
                .Concat(status.Entities.Hashtags);
            using (var sr = new StringReader(status.Text))
            {
                int i = 0;
                var buffer = new StringBuilder();
                while (sr.Peek() != -1)
                {
                    var sEntity = entities.FirstOrDefault(_ => _.Indices.StartIndex == i);
                    if (sEntity == null)
                    {
                        buffer.Append((char)sr.Read());
                        i++;
                    }
                    else
                    {
                        if (buffer.Length != 0)
                        {
                            re.Add(new StatusTextParts.Normal() { Text = buffer.ToString() });
                            buffer.Clear();
                        }

                        var entityContentBuffer = new StringBuilder();
                        while (i != sEntity.Indices.EndIndex)
                        {
                            entityContentBuffer.Append((char)sr.Read());
                            i++;
                        }

                        var url = sEntity as TaskingTwLib.DataModels.TweetEntities.UrlEntity;
                        var mention = sEntity as TaskingTwLib.DataModels.TweetEntities.UserMentionEntity;
                        var hashtag = sEntity as TaskingTwLib.DataModels.TweetEntities.HashtagEntity;
                        if (url != null)
                        {
                            re.Add(new StatusTextParts.Url()
                            {
                                Text = url.DisplayUrl,
                                ShortenedUrl = url.Url,
                                ExpandedUrl = url.ExpandedUrl
                            });
                        }
                        else if (mention != null)
                        {
                            re.Add(new StatusTextParts.UserName()
                            {
                                Text = entityContentBuffer.ToString()
                            });
                        }
                        else if (hashtag != null)
                        {
                            re.Add(new StatusTextParts.Hashtag()
                            {
                                Text = entityContentBuffer.ToString()
                            });
                        }
                    }
                }
                re.Add(new StatusTextParts.Normal() { Text = buffer.ToString() });
                buffer.Clear();
            }

            return re;
        }
    }
}
