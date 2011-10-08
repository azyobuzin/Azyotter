using System;
using System.Collections.Generic;
using System.ComponentModel;
using Azyobuzi.Azyotter.Models.TwitterDataModels;
using Livet;

namespace Azyobuzi.Azyotter.Models
{
    public interface ITimelineItem : INotifyPropertyChanged
    {
        bool IsTweet { get; }
        bool ForAllTab { get; set; }
        ulong Id { get; set; }
        DateTime CreatedAt { get; set; }
        IEnumerable<StatusTextParts.StatusTextPartBase> Text { get; set; }
        IEnumerable<string> ImageThumbnails { get; set; }
        User From { get; set; }
        User To { get; set; }
        ulong InReplyToStatusId { get; set; }
        TaskingTwLib.DataModels.Source Source { get; set; }
    }

    public abstract class TimelineItemBase
        : NotificationObject, ITimelineItem
    {
        public abstract bool IsTweet { get; }

        #region ForAllTab変更通知プロパティ
        bool _ForAllTab;

        public bool ForAllTab
        {
            get
            { return _ForAllTab; }
            set
            {
                if (_ForAllTab == value)
                    return;
                _ForAllTab = value;
                RaisePropertyChanged("ForAllTab");
            }
        }
        #endregion

        #region Id変更通知プロパティ
        ulong _Id;

        public ulong Id
        {
            get
            { return _Id; }
            set
            {
                if (_Id == value)
                    return;
                _Id = value;
                RaisePropertyChanged("Id");
            }
        }
        #endregion

        #region CreatedAt変更通知プロパティ
        DateTime _CreatedAt;

        public DateTime CreatedAt
        {
            get
            { return _CreatedAt; }
            set
            {
                if (_CreatedAt == value)
                    return;
                _CreatedAt = value;
                RaisePropertyChanged("CreatedAt");
            }
        }
        #endregion

        #region Text変更通知プロパティ
        IEnumerable<StatusTextParts.StatusTextPartBase> _Text;

        public IEnumerable<StatusTextParts.StatusTextPartBase> Text
        {
            get
            { return _Text; }
            set
            {
                if (_Text == value)
                    return;
                _Text = value;
                RaisePropertyChanged("Text");
            }
        }
        #endregion
        
        #region ImageThumbnails変更通知プロパティ
        IEnumerable<string> _ImageThumbnails;

        public IEnumerable<string> ImageThumbnails
        {
            get
            { return _ImageThumbnails; }
            set
            {
                if (_ImageThumbnails == value)
                    return;
                _ImageThumbnails = value;
                RaisePropertyChanged("ImageThumbnails");
            }
        }
        #endregion
      
        #region From変更通知プロパティ
        User _From;

        public User From
        {
            get
            { return _From; }
            set
            {
                if (_From == value)
                    return;
                _From = value;
                RaisePropertyChanged("From");
            }
        }
        #endregion

        #region To変更通知プロパティ
        User _To;

        public User To
        {
            get
            { return _To; }
            set
            {
                if (_To == value)
                    return;
                _To = value;
                RaisePropertyChanged("To");
            }
        }
        #endregion
        
        #region InReplyToStatusId変更通知プロパティ
        ulong _InReplyToStatusId;

        public ulong InReplyToStatusId
        {
            get
            { return _InReplyToStatusId; }
            set
            {
                if (_InReplyToStatusId == value)
                    return;
                _InReplyToStatusId = value;
                RaisePropertyChanged("InReplyToStatusId");
            }
        }
        #endregion
        
        #region Source変更通知プロパティ
        TaskingTwLib.DataModels.Source _Source;

        public TaskingTwLib.DataModels.Source Source
        {
            get
            { return _Source; }
            set
            {
                if (_Source == value)
                    return;
                _Source = value;
                RaisePropertyChanged("Source");
            }
        }
        #endregion
      
    }
}
