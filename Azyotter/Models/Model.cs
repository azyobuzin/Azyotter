﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;
using Livet;
using System.Collections.ObjectModel;
using Azyobuzi.Azyotter.LinqToTwitter;
using System.Threading.Tasks;

namespace Azyobuzi.Azyotter.Models
{
    public class Model : NotificationObject
    {
        public void Init()
        {
            this.Tabs = new ObservableCollection<Tab>();
            UserStreamManager.Twitter = this.twitter;
            Settings.Instance.Tabs.ForEach(tab => AddTab(tab));
            this.Tabs.ForEach(tab => tab.Refresh());
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

        public Tab AddTab(TabSettings settings)
        {
            var tab = new Tab(settings, this.twitter);
            this.Tabs.Add(tab);
            return tab;
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
