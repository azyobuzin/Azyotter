using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.Azyotter.Util;
using LinqToTwitter;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public class HomeTimelineReceiver : TimelineReceiver
    {
        public HomeTimelineReceiver()
        {
            StatusCache.Instance.CollectionChanged += this.StatusCache_CollectionChanged;
        }

        public override bool UseUserStream
        {
            get { return true; }
        }

        public override void Receive(int count, int page)
        {
            var t = new Thread(() =>
            {
                this.IsRefreshing = true;

                try
                {
                    this.Twitter.Status
                        .Where(_ => _.Type == StatusType.Home
                            && _.Count == count
                            && _.Page == page
                            && _.IncludeEntities == true)
                        .ForEach(status => StatusCache.Instance.AddOrMerge(status, true));
                }
                catch (Exception ex)
                {
                    this.OnError(ex.GetMessage());
                }
                finally
                {
                    this.IsRefreshing = false;
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        private void StatusCache_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                e.NewItems.Cast<ITimelineItem>()
                    .Where(status => status.IsTweet && status.ForAllTab)
                    .ForEach(status => this.OnReceivedTimeline(new[] { status }));
            }
        }

        public override void Dispose()
        {
            StatusCache.Instance.CollectionChanged -= this.StatusCache_CollectionChanged;
            base.Dispose();
        }
    }
}
