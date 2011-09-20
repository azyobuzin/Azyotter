namespace Azyobuzi.Azyotter.Models.TwitterDataModels
{
    public class Status : TimelineItemBase
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
