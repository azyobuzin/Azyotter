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
                            //TODO:終了させる
                        }
                        else
                        {
                            try
                            {
                                auth.GetAccessToken(token, vm.Pin);
                                this.model.SaveOAuthToken();
                                vm.CloseRequest();
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
        }
        #endregion
        
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
            Settings.Instance.Save();
        }
        #endregion
      
    }
}
