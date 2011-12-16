using System;
using System.Collections.Specialized;
using System.Reactive.Linq;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.TaskingTwLib;
using Azyobuzi.TaskingTwLib.DataModels.UserStreams;
using TwitterApi = Azyobuzi.TaskingTwLib.Methods;

namespace Azyobuzi.Azyotter.Models
{
    public static class UserStreams
    {
        private static IDisposable disposable;

        public static void Start()
        {
            Stop();

            disposable = Settings.Instance.Accounts.ToObservable()
                .SelectMany(a => TwitterApi.UserStreams.UserStreamApi
                    .Create()
                    .CallApi(new Token()
                    {
                        ConsumerKey = Settings.Instance.ConsumerKey,
                        ConsumerSecret = Settings.Instance.ConsumerSecret,
                        OAuthToken = a.OAuthToken,
                        OAuthTokenSecret = a.OAuthTokenSecret
                    })
                    .Catch((Exception ex) =>
                    {
                        if (Error != null)
                            Error(null, new UserStreamsErrorEventArgs(ex));
                        return Observable.Empty<RawData>();
                    })
                    .Repeat()
                )
                .Subscribe(
                    (dynamic data) =>
                    {
                        if (data.Kind == DataKind.Status)
                        {
                            TimelineItemCache.Instance.AddOrMergeTweet(data.Status, true);
                        }

                        if (data.Kind == DataKind.DeleteStatus)
                        {
                            TimelineItemCache.Instance.RemoveTweet(data.Id);
                        }

                        if (data.Kind == DataKind.DeleteDirectMessage)
                        {
                            TimelineItemCache.Instance.RemoveDirectMessage(data.Id);
                        }

                        if (data.Kind == DataKind.Event)
                        {
                            TimelineItemCache.Instance.AddUserStreamEvent(data);
                        }
                    }
                );

            Settings.Instance.Accounts.CollectionChanged += Accounts_CollectionChanged;
        }

        public static void Stop()
        {
            if (disposable != null)
            {
                Settings.Instance.Accounts.CollectionChanged -= Accounts_CollectionChanged;
                disposable.Dispose();
                disposable = null;
            }
        }

        public static event EventHandler<UserStreamsErrorEventArgs> Error;

        private static void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Start();
        }
    }

    public class UserStreamsErrorEventArgs : EventArgs
    {
        public UserStreamsErrorEventArgs(Exception ex)
        {
            this.Error = ex;
        }

        public Exception Error { get; private set; }
    }
}
