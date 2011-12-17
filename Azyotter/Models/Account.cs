using System.Collections.Generic;
using Livet;

namespace Azyobuzi.Azyotter.Models
{
    public class Account : NotificationObject
    {

        #region OAuthToken変更通知プロパティ
        private string _OAuthToken;

        public string OAuthToken
        {
            get
            { return _OAuthToken; }
            set
            { 
                if (EqualityComparer<string>.Default.Equals(_OAuthToken, value))
                    return;
                _OAuthToken = value;
                RaisePropertyChanged("OAuthToken");
            }
        }
        #endregion

        #region OAuthTokenSecret変更通知プロパティ
        private string _OAuthTokenSecret;

        public string OAuthTokenSecret
        {
            get
            { return _OAuthTokenSecret; }
            set
            { 
                if (EqualityComparer<string>.Default.Equals(_OAuthTokenSecret, value))
                    return;
                _OAuthTokenSecret = value;
                RaisePropertyChanged("OAuthTokenSecret");
            }
        }
        #endregion

        #region ScreenName変更通知プロパティ
        private string _ScreenName;

        public string ScreenName
        {
            get
            { return _ScreenName; }
            set
            { 
                if (EqualityComparer<string>.Default.Equals(_ScreenName, value))
                    return;
                _ScreenName = value;
                RaisePropertyChanged("ScreenName");
            }
        }
        #endregion

        #region UserId変更通知プロパティ
        private long _UserId;

        public long UserId
        {
            get
            { return _UserId; }
            set
            { 
                if (EqualityComparer<long>.Default.Equals(_UserId, value))
                    return;
                _UserId = value;
                RaisePropertyChanged("UserId");
            }
        }
        #endregion

    }
}
