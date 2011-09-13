using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Azyobuzi.Azyotter.LinqToTwitter;
using LinqToTwitter;
using Livet;

namespace Azyobuzi.Azyotter.Models
{
    public class Model : NotificationObject
    {
        public void Init()
        {
            this.Tabs = new ObservableCollection<Tab>();
            UserStreamManager.Twitter = this.twitter;
            Settings.Instance.Tabs.CollectionChanged += this.Settings_Tabs_CollectionChanged;
            Settings.Instance.Tabs.Select(Tuple.Create<TabSettings, int>)
                .ForEach(tab => this.Settings_Tabs_CollectionChanged(
                    Settings.Instance.Tabs,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        tab.Item1,
                        tab.Item2
                    )
                ));
        }

        private TwitterContext twitter = new TwitterContext()
        {
            AuthorizedClient = new Authorizer()
            {
                Credentials = new InMemoryCredentials()
                {
                    ConsumerKey = Settings.Instance.ConsumerKey,
                    ConsumerSecret = Settings.Instance.ConsumerSecret,
                    OAuthToken = Settings.Instance.OAuthToken,
                    AccessToken = Settings.Instance.OAuthTokenSecret
                }
            }
        };

        public ObservableCollection<Tab> Tabs { get; private set; }

        private void Settings_Tabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddTab(e.NewItems.Cast<TabSettings>().FirstOrDefault(),
                        e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.Tabs.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.Tabs.RemoveAt(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.Tabs.Clear();
                    break;
            }
        }

        public void AddTab(TabSettings settings, int index)
        {
            var tab = new Tab(settings, this.twitter);
            this.Tabs.Insert(index, tab);
            tab.Refresh();
        }

        public Authorizer GetTwitterAuthorizer()
        {
            return this.twitter.AuthorizedClient as Authorizer;
        }

        public void SaveOAuthToken()
        {
            var auth = this.GetTwitterAuthorizer();
            Settings.Instance.OAuthToken = auth.Credentials.OAuthToken;
            Settings.Instance.OAuthTokenSecret = auth.Credentials.AccessToken;
            Settings.Instance.Save();
        }

        public void CloseUserStream()
        {
            if (UserStreamManager.SubscribersCount != 0)
            {
                UserStreamManager.Register(null).UserStream.Close();
            }
        }

        public Task<Status> Post(string text, string inReplyToStatusId)
        {
            return Task.Factory.StartNew(() => this.twitter.UpdateStatus(text, inReplyToStatusId));
        }
    }
}
