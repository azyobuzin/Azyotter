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
        public User User { get; set; }

        public static TimelineItem FromStatus(Status status)
        {
            throw new NotImplementedException();
        }
    }
}
