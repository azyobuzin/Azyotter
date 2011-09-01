using System;
using LinqToTwitter;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public abstract class TimelineReceiverBase : ITimelineReceiver
    {
        public abstract bool UseUserStream { get; }

        public TwitterContext Twitter { get; set; }

        public string Args { get; set; }

        public abstract void Receive(int count, int page);

        public event ReceivedTimelineEventHandler ReceivedTimeline;

        public event ErrorEventHandler Error;

        protected virtual void OnReceivedTimeline(TimelineItem[] gotItems)
        {
            if (this.ReceivedTimeline != null)
                this.ReceivedTimeline(this, gotItems);
        }

        protected virtual void OnError(string errorMessage)
        {
            if (this.Error != null)
                this.Error(this, errorMessage);
        }

        public virtual void Dispose()
        {
            this.Twitter = null;
            GC.SuppressFinalize(this);
        }

        public static ITimelineReceiver CreateTimelineReceiver(TimelineTypes type)
        {
            switch (type)
            {
                case TimelineTypes.Home:
                    return new HomeTimelineReceiver();
                default:
                    throw new ArgumentException("対応していないタイプです。");
            }
        }
    }
}
