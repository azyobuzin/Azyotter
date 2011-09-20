using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Azyobuzi.Azyotter.Models.TwitterDataModels;

namespace Azyobuzi.Azyotter.Models.Caching
{
    public class UserCache : INotifyCollectionChanged
    {
        private UserCache() { }

        private static UserCache instance = new UserCache();
        public static UserCache Instance
        {
            get
            {
                return instance;
            }
        }

        private List<User> collection = new List<User>();
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, e);
        }

        private readonly object lockObj = new object();

        public TResult Query<TResult>(Func<IEnumerable<User>, TResult> action)
        {
            lock (this.lockObj)
            {
                return action(EnumerableEx.Create(() => this.collection.GetEnumerator()));
            }
        }

        public User AddOrMerge(LinqToTwitter.User user)
        {
            var target = this.Query(enm =>
                enm.FirstOrDefault(_ => _.Id == user.UserID || _.ScreenName == user.Identifier.ScreenName)
            );

            if (target == null)
            {
                target = new User();
                lock (this.lockObj)
                    this.collection.Add(target);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, target));
            }

            target.Id = user.UserID;
            target.ScreenName = user.Identifier.ScreenName;
            target.Name = user.Name;
            target.CreatedAt = user.CreatedAt.ToLocalTime();
            target.Description = user.Description;
            target.Location = user.Location;
            target.Url = user.URL;
            target.FriendsCount = user.FriendsCount;
            target.FollowersCount = user.FollowersCount;
            target.StatusesCount = user.StatusesCount;
            target.FavoritesCount = user.FavoritesCount;
            target.ListedCount = user.ListedCount;
            target.Protected = user.Protected;
            target.ProfileImageUrl = user.ProfileImageUrl;

            return target;
        }
    }
}
