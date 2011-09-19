using System;
using System.ComponentModel;

namespace Azyobuzi.Azyotter.Models
{
    public interface ITimelineItem
        : IEquatable<ITimelineItem>, INotifyPropertyChanged
    {
        bool IsTweet { get; }
        string Id { get; set; }
        DateTime CreatedAt { get; set; }
        string Text { get; set; }
        LinqToTwitter.User From { get; set; }
        LinqToTwitter.User To { get; set; }
        TwitterDataModels.Status InReplyToStatus { get; set; }//必ず取得してくる
        TwitterDataModels.Source Source { get; set; }
    }
}
