using System;
using Azyobuzi.TaskingTwLib;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public abstract class TimelineReceiver : ITimelineReceiver
    {
        public abstract bool UseUserStream { get; }

        public Token Token { get; set; }

        public string Args { get; set; }

        public abstract void GetFirst();

        public abstract void Receive(int count, int page);

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get
            {
                return this.isRefreshing;
            }
            protected set
            {
                if (this.isRefreshing != value)
                {
                    this.isRefreshing = value;

                    if (this.IsRefreshingChanged != null)
                        this.IsRefreshingChanged(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler<ReceivedTimelineEventArgs> ReceivedTimeline;

        public event EventHandler<ErrorEventArgs> Error;

        public event EventHandler IsRefreshingChanged;

        protected virtual void OnReceivedTimeline(ITimelineItem[] receivedItems)
        {
            if (this.ReceivedTimeline != null)
                this.ReceivedTimeline(this, new ReceivedTimelineEventArgs(receivedItems));
        }

        protected virtual void OnError(string errorMessage)
        {
            if (this.Error != null)
                this.Error(this, new ErrorEventArgs(errorMessage));
        }

        public virtual void Dispose()
        {
            this.Token = null;
            GC.SuppressFinalize(this);
        }

        public static ITimelineReceiver CreateTimelineReceiver(TimelineTypes type)
        {
            switch (type)
            {
                case TimelineTypes.Home:
                    return new HomeTimelineReceiver();
                case TimelineTypes.Mentions:
                    return new MentionsTimelineReceiver();
                case TimelineTypes.DirectMessages:
                    return new DirectMessagesReceiver();
                case TimelineTypes.Favorites:
                    return new FavoritesTimelineReceiver();
                case TimelineTypes.UserStreamEvents:
                    return new UserStreamEventsReceiver();
                default:
                    throw new ArgumentException("対応していないタイプです。");
            }
        }
    }
}
