using System;
using System.Linq;
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

        public string Text
        {
            get
            {
                return this.Model.Text.First().Text;//TODO:分解して解決
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
    }
}
