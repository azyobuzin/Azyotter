using System.Linq;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.Azyotter.Util;
using Azyobuzi.TaskingTwLib;
using TwitterApi = Azyobuzi.TaskingTwLib.Methods;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public class FavoritesTimelineReceiver : TimelineReceiver
    {
        public override bool UseUserStream
        {
            get { return false; }//unfavoriteに対応できない
        }

        public override void GetFirst()
        {
            //何もしない
        }

        public override void Receive(int count, int page)
        {
            int i = 0;
            this.IsRefreshing = true;
            Settings.Instance.Accounts.ForEach(a =>
                TwitterApi.Favorites.FavoritesApi
                    .Create(count: count, page: page)
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
                            this.OnReceivedTimeline(
                                t.Result
                                    .Select(status => TimelineItemCache.Instance.AddOrMergeTweet(status, true))
                                    .ToArray()
                            );
                        else
                            this.OnError(t.Exception.InnerException.GetMessage());

                        if (++i >= Settings.Instance.Accounts.Count)
                            this.IsRefreshing = false;
                    })
            );
        }
    }
}
