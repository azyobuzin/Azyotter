using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using Azyobuzi.Azyotter.Models.TwitterDataModels;
using Livet;

namespace Azyobuzi.Azyotter.Models.Caching
{
    public class TimelineItemCache
        : IEnumerable<ITimelineItem>, INotifyCollectionChanged
    {
        private TimelineItemCache() { }

        private static TimelineItemCache instance = new TimelineItemCache();
        public static TimelineItemCache Instance
        {
            get
            {
                return instance;
            }
        }

        private ObservableSynchronizedCollection<ITimelineItem> collection = new ObservableSynchronizedCollection<ITimelineItem>();

        public IEnumerator<ITimelineItem> GetEnumerator()
        {
            return collection.GetEnumerator();
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

        public ITimelineItem GetTweet(ulong id)
        {
            return this.collection
                .FirstOrDefault(item => item.IsTweet && item.Id == id);
        }

        public ITimelineItem GetDirectMessage(ulong id)
        {
            return this.collection
                .FirstOrDefault(item => item.IsDirectMessage && item.Id == id);
        }
        
        public bool ContainsTweet(ulong id)
        {
            return this.GetTweet(id) != null;
        }

        public bool ContainsDirectMessage(ulong id)
        {
            return this.GetDirectMessage(id) != null;
        }

        public bool Remove(ITimelineItem item)
        {
            var result = this.collection.Remove(item);
            if (result)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove, item));
            return result;
        }

        public bool RemoveTweet(ulong id)
        {
            return this.Remove(this.GetTweet(id));
        }

        public bool RemoveDirectMessage(ulong id)
        {
            return this.Remove(this.GetDirectMessage(id));
        }

        public ITimelineItem AddOrMergeTweet(TaskingTwLib.DataModels.Status status, bool forAllTab)
        {
            ITimelineItem target = this.GetTweet(status.Id);

            bool isNew = false;
            if (target == null)
            {
                target = new Tweet();
                this.collection.Add(target);
                isNew = true;
            }

            target.ForAllTab = forAllTab || target.ForAllTab;
            target.Id = status.Id;
            target.CreatedAt = status.CreatedAt;
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

        public ITimelineItem AddOrMergeDirectMessage(TaskingTwLib.DataModels.DirectMessage directMessage)
        {
            ITimelineItem target = this.GetDirectMessage(directMessage.Id);

            bool isNew = false;
            if (target == null)
            {
                target = new DirectMessage();
                this.collection.Add(target);
                isNew = true;
            }

            target.ForAllTab = true;
            target.Id = directMessage.Id;
            target.CreatedAt = directMessage.CreatedAt;
            target.Text = this.CreateDirectMessageText(directMessage);
            target.From = UserCache.Instance.AddOrMerge(directMessage.Sender);
            target.To = UserCache.Instance.AddOrMerge(directMessage.Recipient);

            //if (directMessage.Entities != null)
            //{
            //    target.ImageThumbnails = directMessage.Entities.Media
            //        .Select(media => media.MediaUrl + ":thumb")
            //        .ToArray();
            //}

            if (isNew)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, target));

            return target;
        }

        private IEnumerable<StatusTextParts.StatusTextPartBase> CreateStatusText(TaskingTwLib.DataModels.Status status)
        {
            if (status.Entities == null)
                return new[] { new StatusTextParts.Normal() { Text = status.Text } };

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

        private IEnumerable<StatusTextParts.StatusTextPartBase> CreateDirectMessageText(TaskingTwLib.DataModels.DirectMessage directMessage)
        {
            var sts = new TaskingTwLib.DataModels.Status()
            {
                Text = directMessage.Text,
                Entities = directMessage.Entities
            };

            return this.CreateStatusText(sts);
        }
    }
}
