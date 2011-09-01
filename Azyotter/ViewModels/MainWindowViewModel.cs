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
            if (string.IsNullOrEmpty(Settings.Instance.OAuthToken)
                || string.IsNullOrEmpty(Settings.Instance.OAuthTokenSecret))
            {
                this.model.StartAuthorize(uri =>
                {
                    Process.Start(uri);

                    var vm = new InputPinWindowViewModel();
                    ViewModelHelper.BindNotification(vm.CompleteEvent, this, (sender, e) =>
                    {
                        if (vm.IsCanceled)
                        {
                            vm.CloseRequest();
                            return;//TODO:終了させる
                        }
                        else
                        {
                            this.model.InputPin(vm.Pin, _ =>
                            {
                                if (_.Error != null)
                                {
                                    vm.IsBusy = false;
                                }
                                else
                                {
                                    this.model.SaveOAuthToken();
                                    vm.CloseRequest();
                                }
                            });
                        }
                    });
                    this.Messenger.Raise(new TransitionMessage(vm, "ShowInputPinWindow"));
                });
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
