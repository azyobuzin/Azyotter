using System.Windows;
using Azyobuzi.Azyotter.Models;
using Livet;
using Livet.Commands;
using Livet.Messaging;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class StatusBarViewModel : ViewModel
    {
        public StatusBarViewModel()
        {
            this.RunningTaskViewModels = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                RunningTasks.Instance,
                (RunningTask model) => new RunningTaskViewModel(model),
                DispatcherHelper.UIDispatcher
            );
        }

        public StatusBarViewModel(InteractionMessenger messenger)
            : this()
        {
            this.Messenger = messenger;
        }

        public ReadOnlyDispatcherCollection<RunningTaskViewModel> RunningTaskViewModels { get; private set; }

        #region CheckUpdateCommand
        private ViewModelCommand _CheckUpdateCommand;

        public ViewModelCommand CheckUpdateCommand
        {
            get
            {
                if (_CheckUpdateCommand == null)
                {
                    _CheckUpdateCommand = new ViewModelCommand(() => CheckUpdate(false));
                }
                return _CheckUpdateCommand;
            }
        }

        public void CheckUpdate(bool auto)
        {
            var result = Update.GetCanUpdate();
            if (!result)
            {
                if (!auto)
                {
                    this.Messenger.Raise(new InformationMessage(
                        "アップデートは見つかりませんでした。",
                        "アップデート確認",
                        MessageBoxImage.Information,
                        "ShowInformation"));
                }
            }
            else
            {
                var message = this.Messenger.GetResponse(new ConfirmationMessage(
                    "Azyotter " + Update.Latest.Version.ToString() + " があります。アップデートしますか？",
                    "アップデート確認",
                    MessageBoxImage.Information,
                    MessageBoxButton.OKCancel,
                    "Confirmation"));

                if (message.Response.HasValue && message.Response.Value)
                {
                    Update.ExecuteUpdate();
                }
            }
        }
        #endregion
    }
}
