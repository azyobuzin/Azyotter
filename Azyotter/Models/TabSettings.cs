using System;
using Livet;

namespace Azyobuzi.Azyotter.Models
{
    public class TabSettings : NotificationObject
    {
        #region Name変更通知プロパティ
        string _Name;

        public string Name
        {
            get
            { return _Name; }
            set
            {
                if (_Name == value)
                    return;
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }
        #endregion
        
        #region Type変更通知プロパティ
        TimelineTypes _Type;

        public TimelineTypes Type
        {
            get
            { return _Type; }
            set
            {
                if (_Type == value)
                    return;
                _Type = value;
                RaisePropertyChanged("Type");
            }
        }
        #endregion
        
        #region Args変更通知プロパティ
        string _Args;

        public string Args
        {
            get
            { return _Args; }
            set
            {
                if (_Args == value)
                    return;
                _Args = value;
                RaisePropertyChanged("Args");
            }
        }
        #endregion
                
        #region RefreshSpan変更通知プロパティ
        int _RefreshSpan;

        public int RefreshSpan
        {
            get
            { return _RefreshSpan; }
            set
            {
                if (_RefreshSpan == value)
                    return;
                _RefreshSpan = value;
                RaisePropertyChanged("RefreshSpan");
            }
        }
        #endregion
        
        #region GetCount変更通知プロパティ
        int _GetCount;

        public int GetCount
        {
            get
            { return _GetCount; }
            set
            {
                if (_GetCount == value)
                    return;
                _GetCount = value;
                RaisePropertyChanged("GetCount");
            }
        }
        #endregion
      
    }
}
