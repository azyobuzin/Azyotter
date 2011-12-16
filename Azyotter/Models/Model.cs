using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.Azyotter.Updater;
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

            //TODO
            //UserStreams.Error += (sender, e) =>
            //{
            //    var wex = e.Error as WebException;
            //    if (wex == null || wex.Status != WebExceptionStatus.RequestCanceled)
            //        this.Status = "UserStreamで問題が発生したため再接続します";
            //};
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
            var tab = new Tab(setting);
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

        public Task Post(string text, ulong? inReplyToStatusId, bool useFooter)
        {
            //if (useFooter && !string.IsNullOrEmpty(Settings.Instance.Footer))
            //    text += " " + Settings.Instance.Footer;

            //var cancellation = new CancellationTokenSource();
            //RunningTask runningTask = null;

            //var reTask = TwitterApi.Tweets.UpdateApi
            //    .Create(text, inReplyToStatusId)
            //    .CallApi(this.token, cancellation.Token)
            //    .ContinueWith(t =>
            //    {
            //        RunningTasks.Instance.Remove(runningTask);

            //        if (t.Exception == null)
            //        {
            //            TimelineItemCache.Instance.AddOrMergeTweet(t.Result, true);
            //        }
            //        else
            //        {
            //            //TODO:再試行できるようにする
            //        }
            //    });

            //runningTask = new RunningTask("投稿中：" + text, reTask, cancellation);
            //RunningTasks.Instance.Add(runningTask);

            //return reTask;
            return null;
        }

        public Task PostWithMedia(string text, string mediaFile, ulong? inReplyToStatusId, bool useFooter)
        {
            //if (!File.Exists(mediaFile))
            //{
            //    //TODO:エラー通知
            //}

            //if (useFooter && !string.IsNullOrEmpty(Settings.Instance.Footer))
            //    text += " " + Settings.Instance.Footer;

            //var cancellation = new CancellationTokenSource();
            //RunningTask runningTask = null;

            //var reTask = TwitterApi.Tweets.UpdateApi
            //    .Create(text, new[] { mediaFile }, inReplyToStatusId: inReplyToStatusId)
            //    .CallApi(this.token, cancellation.Token)
            //    .ContinueWith(t =>
            //    {
            //        RunningTasks.Instance.Remove(runningTask);

            //        if (t.Exception == null)
            //        {
            //            TimelineItemCache.Instance.AddOrMergeTweet(t.Result, true);
            //        }
            //        else
            //        {
            //            //TODO:再試行できるようにする
            //        }
            //    });

            //runningTask = new RunningTask("画像投稿中：" + text, reTask, cancellation);
            //RunningTasks.Instance.Add(runningTask);

            //return reTask;
            return null;
        }
    }
}
