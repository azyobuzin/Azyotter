using System.Linq;
using Azyobuzi.Azyotter.Models.Caching;
using LinqToTwitter;

namespace Azyobuzi.Azyotter.Models
{
    public static class UserStreams
    {
        public static TwitterContext Twitter { get; set; }

        private static UserStreamParser userstream;

        public static void Start()
        {
            Stop();

            userstream = Twitter.UserStream
                .Where(us => us.Type == UserStreamType.User)
                .CreateParser();

            userstream.ReceivedStatus += userstream_ReceivedStatus;
            userstream.ReceivedDeletedStatus += userstream_ReceivedDeletedStatus;
        }

        public static void Stop()
        {
            if (userstream != null)
            {
                userstream.Close();
                userstream.ReceivedStatus -= userstream_ReceivedStatus;
                userstream.ReceivedDeletedStatus -= userstream_ReceivedDeletedStatus;
                userstream = null;
            }
        }

        private static void userstream_ReceivedStatus(object sender, UserStreamReceivedStatusEventArgs e)
        {
            StatusCache.Instance.AddOrMerge(e.Status, true);
        }

        private static void userstream_ReceivedDeletedStatus(object sender, UserStreamReceivedDeletedEventArgs e)
        {
            StatusCache.Instance.Remove(e.Id);
        }
    }
}
