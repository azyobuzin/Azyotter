using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Azyobuzi.Azyotter.Models;
using Livet;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class TimelineItemViewModel : ViewModel
    {
        public TimelineItemViewModel(ITimelineItem model)
        {
            this.Model = model;
        }

        public ITimelineItem Model { private set; get; }

        public bool IsTweet
        {
            get
            {
                return this.Model.IsTweet;
            }
        }

        public bool IsDirectMessage
        {
            get
            {
                return this.Model.IsDirectMessage;
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                return this.Model.CreatedAt;
            }
        }

        public IEnumerable<Inline> Text
        {
            get
            {
                return this.Model.Text
                    .Where(part => part != null && part.Text != null)
                    .Select(part =>
                    {
                        var url = part as Models.StatusTextParts.Url;
                        if (url != null)
                        {
                            var re = new Hyperlink();
                            re.TextDecorations.Add(TextDecorations.Underline);
                            re.Cursor = Cursors.Hand;
                            re.Foreground = Brushes.Blue;
                            re.Inlines.Add(url.Text);
                            re.ToolTip = url.ExpandedUrl;
                            re.Click += (sender, e) => Process.Start(url.ShortenedUrl);
                            return (Inline)re;
                        }

                        var mention = part as Models.StatusTextParts.UserName;
                        if (mention != null)
                        {
                            var re = new Hyperlink();
                            re.TextDecorations.Add(TextDecorations.Underline);
                            re.Cursor = Cursors.Hand;
                            re.Foreground = Brushes.Blue;
                            re.Inlines.Add(mention.Text);
                            return (Inline)re;
                        }

                        var hashtag = part as Models.StatusTextParts.Hashtag;
                        if (hashtag != null)
                        {
                            var re = new Hyperlink();
                            re.TextDecorations.Add(TextDecorations.Underline);
                            re.Cursor = Cursors.Hand;
                            re.Foreground = Brushes.Blue;
                            re.Inlines.Add(hashtag.Text);
                            return (Inline)re;
                        }

                        return new Run(part.Text);
                    });
            }
        }

        public IEnumerable<string> ImageThumbnails
        {
            get
            {
                return this.Model.ImageThumbnails;
            }
        }

        public string FromUserName
        {
            get
            {
                return this.Model.From.Name;
            }
        }

        public string FromScreenName
        {
            get
            {
                return this.Model.From.ScreenName;
            }
        }

        public string ToUserName
        {
            get
            {
                return this.Model.To.Name;
            }
        }

        public string ToScreenName
        {
            get
            {
                return this.Model.To.ScreenName;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.FromScreenName + "/" + this.FromUserName
                    + (this.Model.To != null
                        ? " → " + this.ToScreenName + "/" + this.ToUserName
                        : string.Empty
                      );
            }
        }

        public string FromProfileImageUrl
        {
            get
            {
                return this.Model.From.ProfileImageUrl;
            }
        }
        
        #region IsSelected変更通知プロパティ
        bool _IsSelected;

        public bool IsSelected
        {
            get
            { return _IsSelected; }
            set
            {
                if (_IsSelected == value)
                    return;
                _IsSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        #endregion
      
    }
}
