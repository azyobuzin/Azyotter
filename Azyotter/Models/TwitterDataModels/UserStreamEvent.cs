namespace Azyobuzi.Azyotter.Models.TwitterDataModels
{
    public class UserStreamEvent : TimelineItemBase
    {
        public override bool IsTweet
        {
            get { return false; }
        }

        public override bool IsDirectMessage
        {
            get { return false; }
        }
    }
}
