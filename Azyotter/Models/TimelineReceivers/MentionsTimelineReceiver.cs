using System;
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
    public class MentionsTimelineReceiver : TimelineReceiver
    {
        public MentionsTimelineReceiver()
        {
            TimelineItemCache.Instance.CollectionChanged += this.TimelineItemCache_CollectionChanged;
        }

        public override bool UseUserStream
        {
            get { return true; }
        }

        private IEnumerable<ITimelineItem> Query(IEnumerable<ITimelineItem> source)
        {
            return source
                .Where(status => status.IsTweet)
                .Where(status => Settings.Instance.Accounts
                    .Any(account => status.Text.OfType<StatusTextParts.UserName>()
                        .Any(entity => entity.Text.TrimStart('@').Equals(account.ScreenName, StringComparison.InvariantCultureIgnoreCase))
                    )
                );
        }

        public override void GetFirst()
        {
            Task.Factory.StartNew(() =>
                this.Query(TimelineItemCache.Instance)
                    .ForEach(status => this.OnReceivedTimeline(new[] { status }))
            );
        }

        public override void Receive(int count, int page)
        {
            int i = 0;
            this.IsRefreshing = true;
            Settings.Instance.Accounts.ForEach(a =>
                TwitterApi.Tweets.TimelinesApi
                    .Create(TwitterApi.Tweets.TimelineType.Mentions, count: count, page: page)
                    .CallApi(new Token()
                    {
                        ConsumerKey = Settings.Instance.ConsumerKey,
                        ConsumerSecret = Settings.Instance.ConsumerSecret,
                        OAuthToken = a.OAuthToken,
                        OAuthTokenSecret = a.OAuthTokenSecret
                    })
                    .ContinueWith(t =>
                    {
                        if (t.Exception == null)
                            t.Result.ForEach(status => TimelineItemCache.Instance.AddOrMergeTweet(status, true));
                        else
                            this.OnError(t.Exception.InnerException.GetMessage());

                        if (++i >= Settings.Instance.Accounts.Count)
                            this.IsRefreshing = false;
                    })
            );
        }

        private void TimelineItemCache_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.Query(e.NewItems.Cast<ITimelineItem>())
                    .ForEach(status => this.OnReceivedTimeline(new[] { status }));
            }
        }

        public override void Dispose()
        {
            TimelineItemCache.Instance.CollectionChanged -= this.TimelineItemCache_CollectionChanged;
            base.Dispose();
        }
    }
}
