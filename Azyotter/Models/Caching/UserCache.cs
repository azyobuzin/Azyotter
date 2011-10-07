using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Azyobuzi.Azyotter.Models.TwitterDataModels;

namespace Azyobuzi.Azyotter.Models.Caching
{
    public class UserCache
        : IEnumerable<User>, INotifyCollectionChanged
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

        private ConcurrentBag<User> collection = new ConcurrentBag<User>();

        public IEnumerator<User> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, e);
        }

        public User AddOrMerge(TaskingTwLib.DataModels.User user)
        {
            var target = this.collection.FirstOrDefault(_ =>
                _.Id == user.Id || _.ScreenName == user.ScreenName);

            bool isNew = false;
            if (target == null)
            {
                target = new User();
                this.collection.Add(target);
                isNew = true;
            }

            target.Id = user.Id;
            target.ScreenName = user.ScreenName;
            target.Name = user.Name;
            target.CreatedAt = user.CreatedAt.ToLocalTime();
            target.Description = user.Description;
            target.Location = user.Location;
            target.Url = user.Url;
            target.FriendsCount = user.FriendsCount;
            target.FollowersCount = user.FollowersCount;
            target.StatusesCount = user.StatusesCount;
            target.FavouritesCount = user.FavouritesCount;
            target.ListedCount = user.ListedCount;
            target.Protected = user.Protected;
            target.ProfileImageUrl = user.ProfileImageUrl;

            if (isNew)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, target));

            return target;
        }
    }
}
