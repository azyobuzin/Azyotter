using System;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public interface ITimelineReceiver : IDisposable
    {
        bool UseUserStream { get; }
        LinqToTwitter.TwitterContext Twitter { get; set; }
        string Args { get; set; }
        void Receive(int count, int page);
        event ReceivedTimelineEventHandler ReceivedTimeline;
        event ErrorEventHandler Error;
    }

    public delegate void ReceivedTimelineEventHandler(object sender, TimelineItem[] gotItems);
    public delegate void ErrorEventHandler(object sender, string errorMessage);
}
