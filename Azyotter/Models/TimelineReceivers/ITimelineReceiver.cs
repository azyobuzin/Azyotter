using System;
using LinqToTwitter;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public interface ITimelineReceiver : IDisposable
    {
        bool UseUserStream { get; }
        TwitterContext Twitter { get; set; }
        string Args { get; set; }
        void Receive(int count, int page);
        bool IsRefreshing { get; }
        event ReceivedTimelineEventHandler ReceivedTimeline;
        event ErrorEventHandler Error;
        event EventHandler IsRefreshingChanged;
    }

    public delegate void ReceivedTimelineEventHandler(object sender, TimelineItem[] gotItems);
    public delegate void ErrorEventHandler(object sender, string errorMessage);
}
