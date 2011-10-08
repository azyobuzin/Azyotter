namespace Azyobuzi.Azyotter.Models.StatusTextParts
{
    public class Url : StatusTextPartBase
    {

        #region ShortenedUrl変更通知プロパティ
        string _ShortenedUrl;

        public string ShortenedUrl
        {
            get
            { return _ShortenedUrl; }
            set
            {
                if (_ShortenedUrl == value)
                    return;
                _ShortenedUrl = value;
                RaisePropertyChanged("Url");
            }
        }
        #endregion
        
        #region ExpandedUrl変更通知プロパティ
        string _ExpandedUrl;

        public string ExpandedUrl
        {
            get
            { return _ExpandedUrl; }
            set
            {
                if (_ExpandedUrl == value)
                    return;
                _ExpandedUrl = value;
                RaisePropertyChanged("ExpandedUrl");
            }
        }
        #endregion
      
    }
}
