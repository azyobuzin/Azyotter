﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Livet;

namespace Azyobuzi.Azyotter.Models
{
    public class Model : NotificationObject
    {
        public void Init()
        {
            this.Tabs = new ObservableCollection<Tab>();
            UserStreams.Twitter = this.twitter;
            Settings.Instance.PropertyChanged += this.Settings_PropertyChanged;
            Settings.Instance.Tabs.CollectionChanged += this.Settings_Tabs_CollectionChanged;
            Settings.Instance.Tabs.Select(Tuple.Create<TabSetting, int>)
                .ForEach(tab => this.Settings_Tabs_CollectionChanged(
                    Settings.Instance.Tabs,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        tab.Item1,
                        tab.Item2
                    )
                ));
            this.Settings_PropertyChanged(Settings.Instance, new PropertyChangedEventArgs("UseUserStream"));
        }

        private TwitterContext twitter = new TwitterContext()
        {
            AuthorizedClient = new Authorizer()
            {
                Credentials = new InMemoryCredentials()
                {
                    ConsumerKey = Settings.Instance.ConsumerKey,
                    ConsumerSecret = Settings.Instance.ConsumerSecret,
                    OAuthToken = Settings.Instance.Accounts.First().OAuthToken,
                    AccessToken = Settings.Instance.Accounts.First().OAuthTokenSecret
                }
            }
        };

        public ObservableCollection<Tab> Tabs { get; private set; }

        private void Settings_Tabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddTab(e.NewItems.Cast<TabSetting>().FirstOrDefault(),
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

        public void AddTab(TabSetting settings, int index)
        {
            var tab = new Tab(settings, this.twitter);
            this.Tabs.Insert(index, tab);
            tab.Refresh();
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UseUserStream")
            {
                if (Settings.Instance.UseUserStream)
                    UserStreams.Start();
                else
                    UserStreams.Stop();
            }
        }

        public Authorizer GetTwitterAuthorizer()
        {
            return this.twitter.AuthorizedClient as Authorizer;
        }

        public void SaveOAuthToken()
        {
            var auth = this.GetTwitterAuthorizer();
            Settings.Instance.Accounts.First().OAuthToken = auth.Credentials.OAuthToken;
            Settings.Instance.Accounts.First().OAuthTokenSecret = auth.Credentials.AccessToken;
            Settings.Instance.Save();
        }

        public void CloseUserStream()
        {
            UserStreams.Stop();
        }

        public Task<Status> Post(string text, string inReplyToStatusId)
        {
            return Task.Factory.StartNew(() => this.twitter.UpdateStatus(text, inReplyToStatusId));
        }
    }
}
