using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Azyobuzi.Azyotter.Models;
using Azyobuzi.Azyotter.Util;
using Livet;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class TimelineItemViewModel : ViewModel
    {
        public TimelineItemViewModel(ITimelineItem model)
        {
            this.model = model;
        }

        private ITimelineItem model;

        public bool IsTweet
        {
            get
            {
                return this.model.IsTweet;
            }
        }

        public bool IsDirectMessage
        {
            get
            {
                return this.model.IsDirectMessage;
            }
        }

        public ulong Id
        {
            get
            {
                return this.model.Id;
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                return this.model.CreatedAt;
            }
        }

        public IEnumerable<Inline> Text
        {
            get
            {
                return this.model.Text
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
                            re.Click += (sender, e) => ProcessHelper.Start(url.ShortenedUrl);
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

        public string TextString
        {
            get
            {
                return string.Join("",
                    this.model.Text
                        .Where(part => part != null && part.Text != null)
                        .Select(part => part.Text)
                );
            }
        }

        public IEnumerable<string> ImageThumbnails
        {
            get
            {
                return this.model.ImageThumbnails;
            }
        }

        public string FromUserName
        {
            get
            {
                return this.model.From.Name;
            }
        }

        public string FromScreenName
        {
            get
            {
                return this.model.From.ScreenName;
            }
        }

        public string ToUserName
        {
            get
            {
                return this.model.To.Name;
            }
        }

        public string ToScreenName
        {
            get
            {
                return this.model.To.ScreenName;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.FromScreenName + "/" + this.FromUserName
                    + (this.model.To != null
                        ? " → " + this.ToScreenName + "/" + this.ToUserName
                        : string.Empty
                      );
            }
        }

        public string FromProfileImageUrl
        {
            get
            {
                return this.model.From.ProfileImageUrl;
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
