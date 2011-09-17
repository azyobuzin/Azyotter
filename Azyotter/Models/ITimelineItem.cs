using System;
using System.ComponentModel;

namespace Azyobuzi.Azyotter.Models
{
    public interface ITimelineItem
        : IEquatable<ITimelineItem>, INotifyPropertyChanged
    {
        string Id { get; set; }
        DateTime CreatedAt { get; set; }
        string Text { get; set; }
        TwitterDataModel.User From { get; set; }
        TwitterDataModel.User To { get; set; }
        TwitterDataModel.Status InReplyToStatus { get; set; }//必ず取得してくる
        TwitterDataModel.Source Source { get; set; }
    }
}
