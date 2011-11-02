using Azyobuzi.Azyotter.Models;
using Livet;
using Livet.Commands;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class RunningTaskViewModel : ViewModel
    {
        public RunningTaskViewModel(RunningTask model)
        {
            this.model = model;

            ViewModelHelper.BindNotifyChanged(model, this, (sender, e) =>
            {
                if (e.PropertyName == "Description")
                    this.RaisePropertyChanged(() => this.Description);
            });
        }

        private RunningTask model;

        public string Description
        {
            get
            {
                return this.model.Description;
            }
        }

        #region CancelCommand
        private ViewModelCommand _CancelCommand;

        public ViewModelCommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                {
                    _CancelCommand = new ViewModelCommand(Cancel, CanCancel);
                }
                return _CancelCommand;
            }
        }

        public bool CanCancel()
        {
            return this.model.CancellationTokenSource != null;
        }

        public void Cancel()
        {
            this.model.CancellationTokenSource.Cancel();
        }
        #endregion

    }
}
