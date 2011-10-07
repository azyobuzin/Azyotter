using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.TaskingTwLib;
using Azyobuzi.TaskingTwLib.DataModels.UserStreams;
using TwitterApi = Azyobuzi.TaskingTwLib.Methods;

namespace Azyobuzi.Azyotter.Models
{
    public static class UserStreams
    {
        private static List<IDisposable> disposable = new List<IDisposable>();
        public static Token Token { get; set; }

        public static void Start()
        {
            Stop();

            var obs = TwitterApi.UserStreams.UserStreamApi
                .Create()
                .CallApi(Token)
                .AsObservable();

            disposable.Add(
                obs.OfType<StatusData>()
                    .Subscribe(data => StatusCache.Instance.AddOrMerge(data.Status, true))
            );

            disposable.Add(
                obs.OfType<DeleteData>()
                    .Where(data => data.Kind == DataKind.DeleteStatus)
                    .Subscribe(data => StatusCache.Instance.Remove(data.Id))
            );
        }

        public static void Stop()
        {
            disposable.ForEach(d => d.Dispose());
            disposable.Clear();
        }
    }
}
