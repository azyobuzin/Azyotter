using System;
using Azyobuzi.Azyotter.Models;
using Livet;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class TimelineItemViewModel : ViewModel
    {
        public TimelineItemViewModel(TimelineItem model)
        {
            this.Model = model;
        }

        public TimelineItem Model { private set; get; }

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
                return this.Model.Text;
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
                return this.Model.From.Identifier.ScreenName;
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
