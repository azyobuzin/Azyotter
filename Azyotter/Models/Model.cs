using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;
using Livet;
using System.Collections.ObjectModel;

namespace Azyobuzi.Azyotter.Models
{
    public class Model : NotificationObject
    {
        public Model()
        {
            UserStreamManager.Twitter = this.twitter;
        }

        private TwitterContext twitter = new TwitterContext()
        {
            AuthorizedClient = new PinAuthorizer()
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
            var tab = new Tab(settings);
            this.Tabs.Add(tab);
            return tab;
        }

        public void StartAuthorize(Action<string> goToTwitterAuthorization)
        {
            var auth = (PinAuthorizer)this.twitter.AuthorizedClient;
            auth.GoToTwitterAuthorization = goToTwitterAuthorization;
            auth.BeginAuthorize(null);
        }

        public void InputPin(string pin, Action<TwitterAsyncResponse<UserIdentifier>> callback)
        {
            var auth = (PinAuthorizer)this.twitter.AuthorizedClient;
            auth.CompleteAuthorize(pin, callback);
        }

        public void SaveOAuthToken()
        {
            var auth = (PinAuthorizer)this.twitter.AuthorizedClient;
            Settings.Instance.OAuthToken = auth.OAuthTwitter.OAuthToken;
            Settings.Instance.OAuthTokenSecret = auth.OAuthTwitter.OAuthTokenSecret;
            Settings.Instance.Save();
        }
    }
}
