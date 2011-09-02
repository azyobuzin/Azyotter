using System;
using LinqToTwitter;

namespace Azyobuzi.Azyotter.Models
{
    public class TimelineItem
    {
        public object Base { get; set; }
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }
        public User From { get; set; }
        public User To { get; set; }
        public string InReplyToStatusId { get; set; }
        public string Source { get; set; }

        public static TimelineItem FromStatus(Status status)
        {
            return new TimelineItem()
            {
                Base = status,
                Id = status.StatusID,
                CreatedAt = status.CreatedAt.ToLocalTime(),
                Text = status.Text,
                From = status.User,
                InReplyToStatusId = status.InReplyToStatusID,
                Source = status.Source
            };
        }

        public override bool Equals(object obj)
        {
            var other = obj as TimelineItem;
            if (other == null) return false;

            return this.Base.GetType().FullName == other.Base.GetType().FullName
                && this.Id == other.Id
                && this.Text == other.Text;
        }

        public override int GetHashCode()
        {
            return this.Base.GetType().FullName.GetHashCode()
                ^ this.Id.GetHashCode()
                ^ this.Text.GetHashCode();
        }
    }
}
