namespace Azyobuzi.Azyotter.Models.TwitterDataModels
{
    public class DirectMessage : TimelineItemBase
    {
        public override bool IsTweet
        {
            get { return false; }
        }

        public override bool IsDirectMessage
        {
            get { return true; }
        }
    }
}
