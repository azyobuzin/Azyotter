﻿using System;
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
        event EventHandler<ReceivedTimelineEventArgs> ReceivedTimeline;
        event EventHandler<ErrorEventArgs> Error;
        event EventHandler IsRefreshingChanged;
    }
    
    public class ReceivedTimelineEventArgs : EventArgs
    {
        public ReceivedTimelineEventArgs(TimelineItem[] receivedItems)
        {
            this.ReceivedItems = receivedItems;
        }

        public TimelineItem[] ReceivedItems { get; private set; }
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
