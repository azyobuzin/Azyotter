using System.Collections.Generic;
using Azyobuzi.Azyotter.Models;
using Livet;
using Livet.Commands;
using Livet.Messaging;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class TabSettingWindowViewModel : ViewModel
    {
        public TabSettingWindowViewModel(TabSetting model)
        {
            this.model = model;
            this.Name = model.Name;
            this.Type = model.Type;
            this.Args = model.Args;
            this.RefreshSpan = model.RefreshSpan;
            this.GetCount = model.GetCount;
        }

        private TabSetting model;

        #region Name変更通知プロパティ
        private string _Name;

        public string Name
        {
            get
            { return _Name; }
            set
            { 
                if (EqualityComparer<string>.Default.Equals(_Name, value))
                    return;
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }
        #endregion

        #region Type変更通知プロパティ
        private TimelineTypes _Type;

        public TimelineTypes Type
        {
            get
            { return _Type; }
            set
            { 
                if (EqualityComparer<TimelineTypes>.Default.Equals(_Type, value))
                    return;
                _Type = value;
                RaisePropertyChanged("Type");
            }
        }
        #endregion

        #region Args変更通知プロパティ
        private string _Args;

        public string Args
        {
            get
            { return _Args; }
            set
            { 
                if (EqualityComparer<string>.Default.Equals(_Args, value))
                    return;
                _Args = value;
                RaisePropertyChanged("Args");
            }
        }
        #endregion

        #region RefreshSpan変更通知プロパティ
        private int _RefreshSpan;

        public int RefreshSpan
        {
            get
            { return _RefreshSpan; }
            set
            { 
                if (EqualityComparer<int>.Default.Equals(_RefreshSpan, value))
                    return;
                _RefreshSpan = value;
                RaisePropertyChanged("RefreshSpan");
            }
        }
        #endregion

        #region GetCount変更通知プロパティ
        private int _GetCount;

        public int GetCount
        {
            get
            { return _GetCount; }
            set
            { 
                if (EqualityComparer<int>.Default.Equals(_GetCount, value))
                    return;
                _GetCount = value;
                RaisePropertyChanged("GetCount");
            }
        }
        #endregion

        #region OkCommand
        private ViewModelCommand _OkCommand;

        public ViewModelCommand OkCommand
        {
            get
            {
                if (_OkCommand == null)
                {
                    _OkCommand = new ViewModelCommand(Ok);
                }
                return _OkCommand;
            }
        }

        public void Ok()
        {
            this.model.Name = this.Name;
            this.model.Type = this.Type;
            this.model.Args = this.Args;
            this.model.RefreshSpan = this.RefreshSpan;
            this.model.GetCount = this.GetCount;
            Settings.Instance.Save();
            this.Cancel();
        }
        #endregion

        #region CancelCommand
        private ViewModelCommand _CancelCommand;

        public ViewModelCommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                {
                    _CancelCommand = new ViewModelCommand(Cancel);
                }
                return _CancelCommand;
            }
        }

        public void Cancel()
        {
            this.Messenger.Raise(new InteractionMessage("CloseWindow"));
        }
        #endregion
    }
}
