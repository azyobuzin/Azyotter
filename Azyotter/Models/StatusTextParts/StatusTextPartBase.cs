using Livet;

namespace Azyobuzi.Azyotter.Models.StatusTextParts
{
    public class StatusTextPartBase : NotificationObject
    {
        
        #region Text変更通知プロパティ
        string _Text;

        public string Text
        {
            get
            { return _Text; }
            set
            {
                if (_Text == value)
                    return;
                _Text = value;
                RaisePropertyChanged("Text");
            }
        }
        #endregion
      
    }
}
