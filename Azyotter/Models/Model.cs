using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.Azyotter.Util;
using Azyobuzi.TaskingTwLib;
using Livet;
using TwitterApi = Azyobuzi.TaskingTwLib.Methods;

namespace Azyobuzi.Azyotter.Models
{
    public class Model : NotificationObject
    {
        public Model()
        {
            DefaultSetting.UserAgent = "Azyotter v" + AssemblyUtil.GetInformationalVersion();
            DefaultSetting.Timeout = 20 * 1000;
            this.token = new Token()
            {
                ConsumerKey = Settings.Instance.ConsumerKey,
                ConsumerSecret = Settings.Instance.ConsumerSecret,
                OAuthToken = Settings.Instance.Accounts.First().OAuthToken,
                OAuthTokenSecret = Settings.Instance.Accounts.First().OAuthTokenSecret
            };
            UserStreams.Token = this.token;

            UserStreams.Error += (sender, e) =>
            {
                var wex = e.Error as WebException;
                if (wex == null || wex.Status != WebExceptionStatus.RequestCanceled)
                    this.Status = "UserStreamで問題が発生したため再接続します";
            };
        }

        public void Init()
        {
            this.Tabs = new ObservableCollection<Tab>();
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

        private Token token;

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

        public void AddTab(TabSetting setting, int index)
        {
            var tab = new Tab(setting, this.token);
            this.Tabs.Insert(index, tab);
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

        public void SaveOAuthToken(Token token, long userId, string screenName)
        {
            this.token = token;
            UserStreams.Token = token;
            Settings.Instance.Accounts.First().OAuthToken = token.OAuthToken;
            Settings.Instance.Accounts.First().OAuthTokenSecret = token.OAuthTokenSecret;
            Settings.Instance.Accounts.First().UserId = userId;
            Settings.Instance.Accounts.First().ScreenName = screenName;
            Settings.Instance.Save();
        }

        public void CloseUserStream()
        {
            UserStreams.Stop();
        }

        public Task<bool> Post(string text, ulong? inReplyToStatusId, bool useFooter)
        {
            if (useFooter && !string.IsNullOrEmpty(Settings.Instance.Footer))
                text += " " + Settings.Instance.Footer;

            this.Status = "投稿中：" + text;

            return TwitterApi.Tweets.UpdateApi
                .Create(text, inReplyToStatusId)
                .CallApi(this.token)
                .ContinueWith(t =>
                {
                    if (t.Exception == null)
                    {
                        StatusCache.Instance.AddOrMerge(t.Result, true);
                        Status = "投稿完了：" + text;
                        return true;
                    }
                    else
                    {
                        Status = "投稿失敗（" + t.Exception.InnerException.GetMessage() + "）：" + text;
                        return false;
                    }
                });
        }

        #region Status変更通知プロパティ
        private string _Status;

        public string Status
        {
            get
            { return _Status; }
            set
            { 
                if (EqualityComparer<string>.Default.Equals(_Status, value))
                    return;
                _Status = value;
                RaisePropertyChanged("Status");
            }
        }
        #endregion

    }
}
