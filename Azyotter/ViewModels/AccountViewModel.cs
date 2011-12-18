using System;
using Azyobuzi.Azyotter.Models;
using Livet;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class AccountViewModel : ViewModel
    {
        public AccountViewModel(Account model)
        {
            this.Model = model;

            ViewModelHelper.BindNotifyChanged(model, this, (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case "ScreenName":
                        this.RaisePropertyChanged(() => this.ScreenName);
                        this.RaisePropertyChanged(() => this.ProfileImageUri);
                        break;
                    case "UserId":
                        this.RaisePropertyChanged(() => this.UserId);
                        break;
                }
            });
        }

        public Account Model { get; private set; }

        public string ScreenName
        {
            get
            {
                return this.Model.ScreenName;
            }
        }

        public Uri ProfileImageUri
        {
            get
            {
                return new Uri(string.Format("http://api.twitter.com/1/users/profile_image?screen_name={0}", this.ScreenName));
            }
        }

        public long UserId
        {
            get
            {
                return this.Model.UserId;
            }
        }

        #region Removedイベント
        public event EventHandler<EventArgs> Removed;
        private Notificator<EventArgs> _RemovedEvent;
        public Notificator<EventArgs> RemovedEvent
        {
            get
            {
                if (_RemovedEvent == null)
                {
                    _RemovedEvent = new Notificator<EventArgs>();
                }
                return _RemovedEvent;
            }
            set { _RemovedEvent = value; }
        }

        protected virtual void OnRemoved(EventArgs e)
        {
            var threadSafeHandler = System.Threading.Interlocked.CompareExchange(ref Removed, null, null);
            if (threadSafeHandler != null)
            {
                threadSafeHandler(this, e);
            }
            RemovedEvent.Raise(e);
        }
        #endregion

        #region RemoveCommand
        private Livet.Commands.ViewModelCommand _RemoveCommand;

        public Livet.Commands.ViewModelCommand RemoveCommand
        {
            get
            {
                if (_RemoveCommand == null)
                {
                    _RemoveCommand = new Livet.Commands.ViewModelCommand(Remove);
                }
                return _RemoveCommand;
            }
        }

        public void Remove()
        {
            this.OnRemoved(EventArgs.Empty);
        }
        #endregion

    }
}
