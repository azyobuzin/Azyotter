using System.Linq;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.Azyotter.Util;
using LinqToTwitter;

namespace Azyobuzi.Azyotter.Models
{
    public static class UserStreams
    {
        private static TwitterContext CreateTwitterContext()
        {
            return new TwitterContext()
            {
                AuthorizedClient = new Authorizer()
                {
                    Credentials = new InMemoryCredentials()
                    {
                        ConsumerKey = Settings.Instance.ConsumerKey,
                        ConsumerSecret = Settings.Instance.ConsumerSecret,
                        OAuthToken = Settings.Instance.Accounts.First().OAuthToken,
                        AccessToken = Settings.Instance.Accounts.First().OAuthTokenSecret
                    },
                    UserAgent = "Azyotter v" + AssemblyUtil.GetInformationalVersion()
                }
            };
        }
        
        private static UserStreamParser userstream;

        public static void Start()
        {
            Stop();

            userstream = CreateTwitterContext()
                .UserStream
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
