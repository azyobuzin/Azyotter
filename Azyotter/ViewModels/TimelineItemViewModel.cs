using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
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
                    .Select(part =>
                    {
                        var url = part as Models.StatusTextParts.Url;
                        if (url != null)
                        {
                            var re = new Hyperlink();
                            re.Inlines.Add(url.Text);
                            re.NavigateUri = new Uri(url.Text);
                            return (Inline)re;
                        }
                        else
                        {
                            return (Inline)new Run(part.Text);
                        }
                    });
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
