using LinqToTwitter;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public abstract class TimelineReceiverBase : ITimelineReceiver
    {

        public TwitterContext Twitter { get; set; }

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
        }
    }
}
