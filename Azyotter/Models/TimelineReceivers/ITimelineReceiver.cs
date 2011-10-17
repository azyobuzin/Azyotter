using System;
using Azyobuzi.TaskingTwLib;

namespace Azyobuzi.Azyotter.Models.TimelineReceivers
{
    public interface ITimelineReceiver : IDisposable
    {
        bool UseUserStream { get; }
        Token Token { get; set; }
        string Args { get; set; }
        void GetFirst();
        void Receive(int count, int page);
        bool IsRefreshing { get; }
        event EventHandler<ReceivedTimelineEventArgs> ReceivedTimeline;
        event EventHandler<ErrorEventArgs> Error;
        event EventHandler IsRefreshingChanged;
    }
    
    public class ReceivedTimelineEventArgs : EventArgs
    {
        public ReceivedTimelineEventArgs(ITimelineItem[] receivedItems)
        {
            this.ReceivedItems = receivedItems;
        }

        public ITimelineItem[] ReceivedItems { get; private set; }
    }

    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; private set; }
    }
}
