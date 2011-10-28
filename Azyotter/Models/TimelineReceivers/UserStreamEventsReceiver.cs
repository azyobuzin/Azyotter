using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.Azyotter.Models.TwitterDataModels;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public class UserStreamEventsReceiver : TimelineReceiver
    {
        public UserStreamEventsReceiver()
        {
            TimelineItemCache.Instance.CollectionChanged += this.TimelineItemCache_CollectionChanged;
        }

        public override bool UseUserStream
        {
            get { return true; }
        }

        private IEnumerable<ITimelineItem> Query(IEnumerable<ITimelineItem> source)
        {
            return source.Where(item => item is UserStreamEvent);
        }

        public override void GetFirst()
        {
            Task.Factory.StartNew(() =>
                Query(TimelineItemCache.Instance)
                  .ForEach(item => this.OnReceivedTimeline(new[] { item }))
            );
        }

        public override void Receive(int count, int page)
        {
            //何もしない
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
