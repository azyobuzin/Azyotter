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
            this.IsRefreshing = true;
            TwitterApi.Favorites.FavoritesApi
                .Create(count:count,page:page)
                .CallApi(this.Token)
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

                    this.IsRefreshing = false;
                });
        }
    }
}
