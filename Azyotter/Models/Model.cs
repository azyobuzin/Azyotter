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
                        TimelineItemCache.Instance.AddOrMerge(t.Result, true);
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

        #region CanUpdate変更通知プロパティ
        private bool _CanUpdate;

        public bool CanUpdate
        {
            get
            { return _CanUpdate; }
            set
            {
                if (EqualityComparer<bool>.Default.Equals(_CanUpdate, value))
                    return;
                _CanUpdate = value;
                RaisePropertyChanged("CanUpdate");
            }
        }
        #endregion

        #region LatestVersion変更通知プロパティ
        private Update _LatestVersion;

        public Update LatestVersion
        {
            get
            { return _LatestVersion; }
            set
            {
                if (EqualityComparer<Update>.Default.Equals(_LatestVersion, value))
                    return;
                _LatestVersion = value;
                RaisePropertyChanged("LatestVersion");
            }
        }
        #endregion

        public bool GetCanUpdate()
        {
            bool result;

            try
            {
                this.LatestVersion = Updates.GetUpdates().Latest();
                result = this.LatestVersion.Version > Version.Parse(AssemblyUtil.GetInformationalVersion());
            }
            catch
            {
                return false;
            }

            this.CanUpdate = result;

            return result;
        }

        public void Update()
        {
            var asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tmpDir = Directory.CreateDirectory(Path.GetTempPath() + Guid.NewGuid().ToString());

            File.Copy(Path.Combine(asmDir, "Updater.exe"), Path.Combine(tmpDir.FullName, "Updater.exe"), true);
            File.Copy(Path.Combine(asmDir, "Updater.pdb"), Path.Combine(tmpDir.FullName, "Updater.pdb"), true);
            File.Copy(Path.Combine(asmDir, "System.Windows.Interactivity.dll"), Path.Combine(tmpDir.FullName, "System.Windows.Interactivity.dll"), true);
            File.Copy(Path.Combine(asmDir, "Ionic.Zip.dll"), Path.Combine(tmpDir.FullName, "Ionic.Zip.dll"), true);

            Process.Start(Path.Combine(tmpDir.FullName, "Updater.exe"),
                string.Format(@"""{0}"" ""{1}""",
                    Process.GetCurrentProcess().MainModule.FileName,
                    this.LatestVersion.Id));

            this.OnExitRequest(EventArgs.Empty);
        }

        #region ExitRequestイベント
        public event EventHandler<EventArgs> ExitRequest;
        private Notificator<EventArgs> _ExitRequestEvent;
        public Notificator<EventArgs> ExitRequestEvent
        {
            get
            {
                if (_ExitRequestEvent == null)
                {
                    _ExitRequestEvent = new Notificator<EventArgs>();
                }
                return _ExitRequestEvent;
            }
        }

        protected virtual void OnExitRequest(EventArgs e)
        {
            var threadSafeHandler = System.Threading.Interlocked.CompareExchange(ref ExitRequest, null, null);
            if (threadSafeHandler != null)
            {
                threadSafeHandler(this, e);
            }
            ExitRequestEvent.Raise(e);
        }
        #endregion

    }
}
