using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Azyobuzi.Azyotter.Models.Caching;
using Azyobuzi.Azyotter.Models.TimelineReceivers;
using Azyobuzi.TaskingTwLib;
using Livet;

namespace Azyobuzi.Azyotter.Models
{
    public class Tab : NotificationObject, IDisposable
    {
        public Tab(TabSetting settings, Token token)
        {
            this.token = token;
            this.Items = new ObservableCollection<ITimelineItem>();
            StatusCache.Instance.CollectionChanged += this.StatusCache_CollectionChanged;
            this.Settings = settings;
            settings.PropertyChanged += this.Settings_PropertyChanged;
            this.timer = new Timer(_ =>
            {
                if (!Models.Settings.Instance.UseUserStream)
                    this.Refresh();
            });
            this.RaiseSettingsPropertyChanged();
        }

        public TabSetting Settings { get; private set; }
        private Token token;
        private Timer timer;

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    this.RaisePropertyChanged(e.PropertyName);
                    break;
                case "Type":
                    var oldReceiver = this.receiver;//UserStream再接続防止
                    this.receiver = TimelineReceiver.CreateTimelineReceiver(this.Settings.Type);
                    this.receiver.Token = this.token;
                    this.receiver.Args = this.Settings.Args;
                    this.receiver.ReceivedTimeline += this.receiver_ReceivedTimeline;
                    this.receiver.IsRefreshingChanged += this.receiver_IsRefreshingChanged;
                    this.receiver.Error += this.receiver_Error;
                    if (oldReceiver != null)
                    {
                        oldReceiver.ReceivedTimeline -= this.receiver_ReceivedTimeline;
                        oldReceiver.IsRefreshingChanged -= this.receiver_IsRefreshingChanged;
                        oldReceiver.Error -= this.receiver_Error;
                        oldReceiver.Dispose();
                    }
                    this.Items.Clear();
                    this.Refresh();
                    break;
                case "Args":
                    if (this.receiver != null)
                        this.receiver.Args = this.Settings.Args;
                    break;
                case "RefreshSpan":
                    this.timer.Change(0, this.Settings.RefreshSpan * 1000);
                    break;
            }
        }

        /// <summary>
        /// TabSettingsの全メンバー分のSettings_PropertyChangedメソッドを呼ぶ
        /// </summary>
        private void RaiseSettingsPropertyChanged()
        {
            typeof(TabSetting).GetProperties()
                .ForEach(p => this.Settings_PropertyChanged(this.Settings, new PropertyChangedEventArgs(p.Name)));
        }

        public void Dispose()
        {
            this.timer.Dispose();
            this.timer = null;
            this.receiver.ReceivedTimeline -= this.receiver_ReceivedTimeline;
            this.receiver.IsRefreshingChanged -= this.receiver_IsRefreshingChanged;
            this.receiver.Error -= this.receiver_Error;
            this.receiver.Dispose();
            this.receiver = null;
            this.Items = null;
            StatusCache.Instance.CollectionChanged -= this.StatusCache_CollectionChanged;
            this.Settings.PropertyChanged -= this.Settings_PropertyChanged;
            this.Settings = null;
            GC.SuppressFinalize(this);
        }

        public string Name
        {
            get
            {
                return this.Settings.Name;
            }
            set
            {
                this.Settings.Name = value;
            }
        }

        private ITimelineReceiver receiver;

        public ObservableCollection<ITimelineItem> Items { get; private set; }

        private void receiver_ReceivedTimeline(object sender, ReceivedTimelineEventArgs e)
        {
            e.ReceivedItems
                .Where(item => !Items.Contains(item))
                .ForEach(this.Items.Add);
        }

        public void Refresh()
        {
            this.receiver.Receive(this.Settings.GetCount, 1);
        }

        public bool IsRefreshing
        {
            get
            {
                return this.receiver != null
                    ? this.receiver.IsRefreshing
                    : false;
            }
        }

        private void receiver_IsRefreshingChanged(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(() => this.IsRefreshing);
        }
        
        #region LastErrorMessage変更通知プロパティ
        string _LastErrorMessage;

        public string LastErrorMessage
        {
            get
            { return _LastErrorMessage; }
            private set
            {
                if (_LastErrorMessage == value)
                    return;
                _LastErrorMessage = value;
                RaisePropertyChanged("LastErrorMessage");
            }
        }
        #endregion

        private void receiver_Error(object sender, ErrorEventArgs e)
        {
            this.LastErrorMessage = e.ErrorMessage;
        }

        public void ClearErrorMessage()
        {
            this.LastErrorMessage = null;
        }

        private void StatusCache_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                e.OldItems.Cast<ITimelineItem>()
                    .ForEach(item => this.Items.Remove(item));
            }
        }
    }
}
