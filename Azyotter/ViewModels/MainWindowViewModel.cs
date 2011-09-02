using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using System.Threading.Tasks;
using Azyobuzi.Azyotter.Models;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private Model model = new Model();
                
        #region Tabs変更通知プロパティ
        ReadOnlyNotificationDispatcherCollection<TabViewModel> _Tabs;

        public ReadOnlyNotificationDispatcherCollection<TabViewModel> Tabs
        {
            get
            { return _Tabs; }
            private set
            {
                if (_Tabs == value)
                    return;
                _Tabs = value;
                RaisePropertyChanged("Tabs");
            }
        }
        #endregion
      
        #region LoadedCommand
        ViewModelCommand _LoadedCommand;

        public ViewModelCommand LoadedCommand
        {
            get
            {
                if (_LoadedCommand == null)
                    _LoadedCommand = new ViewModelCommand(Loaded);
                return _LoadedCommand;
            }
        }

        private void Loaded()
        {
            var auth = this.model.GetTwitterAuthorizer();
            if (!auth.IsAuthorized)
            {
                string token;
                Process.Start(auth.GetAuthorizationLink(out token));

                var vm = new InputPinWindowViewModel();
                ViewModelHelper.BindNotification(vm.CompleteEvent, this, (sender, e) =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (vm.IsCanceled)
                        {
                            vm.CloseRequest();
                            this.Messenger.Raise(new WindowActionMessage("WindowAction", WindowAction.Close));
                        }
                        else
                        {
                            try
                            {
                                auth.GetAccessToken(token, vm.Pin);
                                this.model.SaveOAuthToken();
                                vm.CloseRequest();
                                DispatcherHelper.BeginInvoke(this.Loaded2);
                            }
                            catch
                            {
                                vm.InvalidPin();
                                vm.IsBusy = false;
                            }
                        }
                    });
                });
                this.Messenger.Raise(new TransitionMessage(vm, "ShowInputPinWindow"));
            }
            else
            {
                Loaded2();
            }
        }
        #endregion

        private void Loaded2()
        {
            this.model.Init();

            this.Tabs = ViewModelHelper.CreateReadOnlyNotificationDispatcherCollection(
                this.model.Tabs,
                item => new TabViewModel(item),
                DispatcherHelper.UIDispatcher
            );
        }
        
        #region ClosingCommand
        ViewModelCommand _ClosingCommand;

        public ViewModelCommand ClosingCommand
        {
            get
            {
                if (_ClosingCommand == null)
                    _ClosingCommand = new ViewModelCommand(Closing);
                return _ClosingCommand;
            }
        }

        private void Closing()
        {
            this.model.CloseUserStream();
            Settings.Instance.Save();
        }
        #endregion
      
    }
}
