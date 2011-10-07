using System.Collections.Specialized;
using System.Linq;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.Azyotter.Util;
using Azyobuzi.TaskingTwLib;
using TwitterApi = Azyobuzi.TaskingTwLib.Methods;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public class MentionsTimelineReceiver : TimelineReceiver
    {
        public MentionsTimelineReceiver()
        {
            StatusCache.Instance.CollectionChanged += this.StatusCache_CollectionChanged;
        }

        public override bool UseUserStream
        {
            get { return true; }
        }

        public override void Receive(int count, int page)
        {
            this.IsRefreshing = true;
            TwitterApi.Tweets.TimelinesApi
                .Create(TwitterApi.Tweets.TimelineType.Mentions, count: count, page: page)
                .CallApi(this.Token)
                .ContinueWith(t =>
                {
                    if (t.Exception == null)
                        t.Result.ForEach(status => StatusCache.Instance.AddOrMerge(status, true));
                    else
                        this.OnError(t.Exception.InnerException.GetMessage());

                    this.IsRefreshing = false;
                });
        }

        private void StatusCache_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                e.NewItems.Cast<ITimelineItem>()
                    .Where(status => status.IsTweet)
                    .Where(status => Settings.Instance.Accounts
                        .Any(account => new string(status.Text.SelectMany(_ => _.Text).ToArray())
                            .Contains("@" + account.ScreenName)
                        )
                    )
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
