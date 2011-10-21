using System;
using System.ComponentModel;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.TaskingTwLib;
using Azyobuzi.TaskingTwLib.DataModels.UserStreams;
using TwitterApi = Azyobuzi.TaskingTwLib.Methods;

namespace Azyobuzi.Azyotter.Models
{
    public static class UserStreams
    {
        private static IDisposable disposable;
        public static Token Token { get; set; }

        public static void Start()
        {
            Stop();

            disposable = TwitterApi.UserStreams.UserStreamApi
                .Create()
                .CallApi(Token)
                .Subscribe(
                    (dynamic data) =>
                    {
                        if (data.Kind == DataKind.Status)
                        {
                            StatusCache.Instance.AddOrMerge(data.Status, true);
                        }

                        if (data.Kind == DataKind.DeleteStatus)
                        {
                            StatusCache.Instance.Remove(data.Id);
                        }
                    },
                    ex =>
                    {
                        var e = new UserStreamsErrorEventArgs(ex);
                        if (Error != null)
                            Error(null, e);
                        if (!e.Cancel)
                            Start();
                    },
                    () =>
                    {
                        Start();
                    }
                );
        }

        public static void Stop()
        {
            if (disposable != null)
            {
                disposable.Dispose();
                disposable = null;
            }
        }

        public static event EventHandler<UserStreamsErrorEventArgs> Error;
    }

    public class UserStreamsErrorEventArgs : CancelEventArgs
    {
        public UserStreamsErrorEventArgs(Exception ex)
        {
            this.Error = ex;
        }

        public Exception Error { get; private set; }
    }
}
