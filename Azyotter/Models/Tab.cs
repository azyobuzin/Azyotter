using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Azyobuzi.Azyotter.Models.TimelineReceivers;
using Livet;

namespace Azyobuzi.Azyotter.Models
{
    public class Tab : NotificationObject, IDisposable
    {
        public Tab(TabSettings settings)
        {
            this.Items = new ObservableCollection<TimelineItem>();
            this.Settings = settings;
            settings.PropertyChanged += this.Settings_PropertyChanged;
            this.timer = new Timer(_ => { if (!this.receiver.UseUserStream) this.Refresh(); });
            this.RaiseSettingsPropertyChanged();
            this.Refresh();
        }

        public TabSettings Settings { get; private set; }
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
                    this.receiver = TimelineReceiverBase.CreateTimelineReceiver(this.Settings.Type);
                    this.receiver.Args = this.Settings.Args;
                    this.receiver.ReceivedTimeline += this.receiver_ReceivedTimeline;
                    if (oldReceiver != null)
                    {
                        oldReceiver.ReceivedTimeline -= this.receiver_ReceivedTimeline;
                        oldReceiver.Dispose();
                    }
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
            typeof(TabSettings).GetProperties()
                .ForEach(p => this.Settings_PropertyChanged(this.Settings, new PropertyChangedEventArgs(p.Name)));
        }

        public void Dispose()
        {
            this.timer.Dispose();
            this.timer = null;
            this.receiver.ReceivedTimeline -= this.receiver_ReceivedTimeline;
            this.receiver.Dispose();
            this.receiver = null;
            this.Items = null;
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

        public ObservableCollection<TimelineItem> Items { get; private set; }

        private void receiver_ReceivedTimeline(object sender, TimelineItem[] gotItems)
        {
            gotItems.Where(item => !Items.Contains(item))
                .ForEach(this.Items.Add);
        }

        public void Refresh()
        {
            this.receiver.Receive(1, this.Settings.GetCount);
        }
    }
}
