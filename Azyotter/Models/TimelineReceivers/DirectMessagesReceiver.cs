using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.Azyotter.Util;
using Azyobuzi.TaskingTwLib;
using TwitterApi = Azyobuzi.TaskingTwLib.Methods;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public class DirectMessagesReceiver : TimelineReceiver
    {
        public DirectMessagesReceiver()
        {
            TimelineItemCache.Instance.CollectionChanged += this.TimelineItemCache_CollectionChanged;
        }

        public override bool UseUserStream
        {
            get { return true; }
        }

        private IEnumerable<ITimelineItem> Query(IEnumerable<ITimelineItem> source)
        {
            return source.Where(item => item.IsDirectMessage && item.ForAllTab);
        }

        public override void GetFirst()
        {
            Task.Factory.StartNew(() =>
                this.Query(TimelineItemCache.Instance)
                    .ForEach(item => this.OnReceivedTimeline(new[] { item }))
            );
        }

        public override void Receive(int count, int page)
        {
            int i = 0;
            this.IsRefreshing = true;
            new[] { TwitterApi.DirectMessages.DirectMessagesApiType.Received, TwitterApi.DirectMessages.DirectMessagesApiType.Sent }
                .ForEach(type =>
                    TwitterApi.DirectMessages.DirectMessagesApi
                        .Create(type, count: count, page: page)
                        .CallApi(this.Token)
                        .ContinueWith(t =>
                        {
                            if (t.Exception == null)
                                t.Result.ForEach(directMessage => TimelineItemCache.Instance.AddOrMergeDirectMessage(directMessage));
                            else
                                this.OnError(t.Exception.InnerException.GetMessage());

                            if (++i >= 2)
                                this.IsRefreshing = false;
                        })
                );
        }

        private void TimelineItemCache_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.Query(e.NewItems.Cast<ITimelineItem>())
                    .ForEach(item => this.OnReceivedTimeline(new[] { item }));
            }
        }

        public override void Dispose()
        {
            TimelineItemCache.Instance.CollectionChanged -= this.TimelineItemCache_CollectionChanged;
            base.Dispose();
        }
    }
}
