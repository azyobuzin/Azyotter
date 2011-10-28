namespace Azyobuzi.Azyotter.Models.TwitterDataModels
{
    public class Tweet : TimelineItemBase
    {
        public override bool IsTweet
        {
            get
            {
                return true;
            }
        }
    }
}
