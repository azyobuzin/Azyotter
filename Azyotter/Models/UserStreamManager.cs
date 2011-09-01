using System;
using System.Collections.Generic;
using System.Linq;
using Azyobuzi.Azyotter.Models.TimelineReceivers;
using LinqToTwitter;

namespace Azyobuzi.Azyotter.Models
{
    public class UserStreamManager
    {
        private UserStreamManager() { }

        public static TwitterContext Twitter { get; set; }

        private UserStreamParser userStream;
        public UserStreamParser UserStream
        {
            get
            {
                return this.userStream;
            }
            private set
            {
                if (this.userStream != value)
                {
                    if (this.UserStreamChanging != null)
                        this.UserStreamChanging(this, EventArgs.Empty);

                    var oldValue = this.userStream;
                    this.userStream = value;

                    if (this.UserStreamChanged != null)
                        this.UserStreamChanged(this, EventArgs.Empty);

                    EventHandler<UserStreamReceivedDeletedEventArgs> deletedStatusHandler = (sender, e) =>
                    {
                        if (DeletedStatus != null)
                            DeletedStatus(sender, e);
                    };
                    EventHandler<UserStreamReceivedDeletedEventArgs> deletedDirectMessageHandler = (sender, e) =>
                    {
                        if (DeletedDirectMessage != null)
                            DeletedDirectMessage(sender, e);
                    };

                    if (oldValue != null)
                    {
                        oldValue.ReceivedDeletedStatus -= deletedStatusHandler;
                        oldValue.ReceivedDeletedDirectMessage -= deletedDirectMessageHandler;
                    }

                    if (value != null)
                    {
                        value.ReceivedDeletedStatus += deletedStatusHandler;
                        value.ReceivedDeletedDirectMessage += deletedDirectMessageHandler;
                    }
                }
            }
        }

        public event EventHandler UserStreamChanging;
        public event EventHandler UserStreamChanged;

        private List<ITimelineReceiver> subscribers = new List<ITimelineReceiver>();

        public int SubscribersCount
        {
            get
            {
                return this.subscribers.Count;
            }
        }

        private static UserStreamManager instance = new UserStreamManager();

        public static UserStreamManager Register(ITimelineReceiver receiver)
        {
            instance.subscribers.Add(receiver);

            if (instance.UserStream == null)
            {
                instance.UserStream = Twitter.UserStream
                    .Where(_ => _.Type == UserStreamType.User)
                    .CreateParser();
            }

            return instance;
        }

        public void Unregister(ITimelineReceiver receiver)
        {
            this.subscribers.Remove(receiver);

            if (!this.subscribers.Any())
            {
                instance.UserStream.Close();
                instance.UserStream = null;
            }
        }

        public static event EventHandler<UserStreamReceivedDeletedEventArgs> DeletedStatus;
        public static event EventHandler<UserStreamReceivedDeletedEventArgs> DeletedDirectMessage;
    }
}
