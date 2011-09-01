using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using LinqToTwitter;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public class HomeTimelineReceiver : TimelineReceiverBase
    {
        public HomeTimelineReceiver()
        {
            Settings.Instance.PropertyChanged += this.Settings_PropertyChanged;
            this.Settings_PropertyChanged(Settings.Instance, new PropertyChangedEventArgs("UseUserStream"));
        }

        public override bool UseUserStream
        {
            get { return true; }
        }

        private UserStreamManager userStream;

        private void SubscribeEvents()
        {
            this.userStream.UserStreamChanging += this.userStream_UserStreamChanging;
            this.userStream.UserStreamChanged += this.userStream_UserStreamChanged;
            this.userStream.UserStream.ReceivedStatus += this.userStream_ReceivedStatus;
        }

        private void UnsubscribeEvents()
        {
            this.userStream.UserStreamChanging -= this.userStream_UserStreamChanging;
            this.userStream.UserStreamChanged -= this.userStream_UserStreamChanged;
            this.userStream.UserStream.ReceivedStatus -= this.userStream_ReceivedStatus;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UseUserStream")
            {
                if (Settings.Instance.UseUserStream)
                {
                    if (this.userStream != null)
                    {
                        this.UnsubscribeEvents();
                        this.userStream.Unregister(this);
                        this.userStream = null;
                    }
                }
                else
                {
                    this.userStream = UserStreamManager.Register(this);
                    this.SubscribeEvents();
                }
            }
        }

        private void userStream_UserStreamChanging(object sender, EventArgs e)
        {
            this.UnsubscribeEvents();
        }

        private void userStream_UserStreamChanged(object sender, EventArgs e)
        {
            this.SubscribeEvents();
        }

        public override void Receive(int count, int page)
        {
            var t = new Thread(() =>
            {
                try
                {
                    this.OnReceivedTimeline(
                        this.Twitter.Status
                            .Where(_ => _.Type == StatusType.Home
                                && _.Count == count
                                && _.Page == page)
                            .Select(TimelineItem.FromStatus)
                            .ToArray()
                    );
                }
                catch (TwitterQueryException ex)
                {
                    this.OnError(ex.Response.Error);
                }
                catch (Exception ex)
                {
                    this.OnError(ex.Message);
                }
            });
        }

        private void userStream_ReceivedStatus(object sender, UserStreamReceivedStatusEventArgs e)
        {
            this.OnReceivedTimeline(new TimelineItem[] { TimelineItem.FromStatus(e.Status) });
        }

        public override void Dispose()
        {
            if (this.userStream != null)
            {
                this.UnsubscribeEvents();
                this.userStream.Unregister(this);
                this.userStream = null;
            }

            base.Dispose();
        }
    }
}
