using System.Collections.Generic;
using Azyobuzi.Azyotter.Models;
using Livet;
using Livet.Commands;
using Livet.Messaging;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class SettingsWindowViewModel : ViewModel
    {
        public SettingsWindowViewModel()
        {
            this.AutoUpdate = Settings.Instance.AutoUpdate;
            this.UseUserStream = Settings.Instance.UseUserStream;
            this.Footer = Settings.Instance.Footer;
        }

        #region AutoUpdate変更通知プロパティ
        private bool _AutoUpdate;

        public bool AutoUpdate
        {
            get
            { return _AutoUpdate; }
            set
            { 
                if (EqualityComparer<bool>.Default.Equals(_AutoUpdate, value))
                    return;
                _AutoUpdate = value;
                RaisePropertyChanged("AutoUpdate");
            }
        }
        #endregion

        #region UseUserStream変更通知プロパティ
        private bool _UseUserStream;

        public bool UseUserStream
        {
            get
            { return _UseUserStream; }
            set
            { 
                if (EqualityComparer<bool>.Default.Equals(_UseUserStream, value))
                    return;
                _UseUserStream = value;
                RaisePropertyChanged("UseUserStream");
            }
        }
        #endregion

        #region Footer変更通知プロパティ
        private string _Footer;

        public string Footer
        {
            get
            { return _Footer; }
            set
            { 
                if (EqualityComparer<string>.Default.Equals(_Footer, value))
                    return;
                _Footer = value.Trim();
                RaisePropertyChanged("Footer");
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
            Settings.Instance.AutoUpdate = this.AutoUpdate;
            Settings.Instance.UseUserStream = this.UseUserStream;
            Settings.Instance.Footer = this.Footer;
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
