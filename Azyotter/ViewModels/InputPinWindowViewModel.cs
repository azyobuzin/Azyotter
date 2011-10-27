using System;
using System.Linq;
using System.Windows;
using Livet;
using Livet.Commands;
using Livet.Messaging;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class InputPinWindowViewModel : ViewModel
    {
        #region IsBusy変更通知プロパティ
        bool _IsBusy;

        public bool IsBusy
        {
            get
            { return _IsBusy; }
            set
            {
                if (_IsBusy == value)
                    return;
                _IsBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }
        #endregion
        
        #region IsCanceled変更通知プロパティ
        bool _IsCanceled = true;

        public bool IsCanceled
        {
            get
            { return _IsCanceled; }
            set
            {
                if (_IsCanceled == value)
                    return;
                _IsCanceled = value;
                RaisePropertyChanged("IsCanceled");
            }
        }
        #endregion
        
        #region Pin変更通知プロパティ
        string _Pin;

        public string Pin
        {
            get
            { return _Pin; }
            set
            {
                if (_Pin == value)
                    return;
                _Pin = value.Trim();
                RaisePropertyChanged("Pin");
                this.OkCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion
        
        #region OkCommand
        ViewModelCommand _OkCommand;

        public ViewModelCommand OkCommand
        {
            get
            {
                if (_OkCommand == null)
                    _OkCommand = new ViewModelCommand(Ok, CanOk);
                return _OkCommand;
            }
        }

        private bool CanOk()
        {
            return !string.IsNullOrEmpty(this.Pin)
                && this.Pin.All(c => "0123456789".Contains(c));
        }

        private void Ok()
        {
            this.IsCanceled = false;
            this.IsBusy = true;
            this.OnComplete(EventArgs.Empty);
        }
        #endregion
        
        #region CancelCommand
        ViewModelCommand _CancelCommand;

        public ViewModelCommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                    _CancelCommand = new ViewModelCommand(Cancel);
                return _CancelCommand;
            }
        }

        private void Cancel()
        {
            this.IsBusy = true;
            this.OnComplete(EventArgs.Empty);
        }
        #endregion
        
        #region Completeイベント
        public event EventHandler<EventArgs> Complete;
        private Notificator<EventArgs> _CompleteEvent;
        public Notificator<EventArgs> CompleteEvent
        {
            get
            {
                if (_CompleteEvent == null) _CompleteEvent = new Notificator<EventArgs>();
                return _CompleteEvent;
            }
            set { _CompleteEvent = value; }
        }

        protected void OnComplete(EventArgs e)
        {
            var threadSafeHandler = System.Threading.Interlocked.CompareExchange(ref Complete, null, null);
            if (threadSafeHandler != null) threadSafeHandler(this, e);
            CompleteEvent.Raise(e);
        }
        #endregion

        public void CloseRequest()
        {
            this.Messenger.Raise(new InteractionMessage("Close"));
        }

        public void InvalidPin()
        {
            this.IsCanceled = true;
            this.Messenger.Raise(new InformationMessage("認証できませんでした。", "失敗", MessageBoxImage.Error, "ShowInformation"));
        }
    }
}
