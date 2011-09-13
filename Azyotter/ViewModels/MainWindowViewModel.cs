using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Azyobuzi.Azyotter.Models;
using Azyobuzi.Azyotter.Util;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.Windows;

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
        
        #region SelectedTab変更通知プロパティ
        TabViewModel _SelectedTab;

        public TabViewModel SelectedTab
        {
            get
            { return _SelectedTab; }
            set
            {
                if (_SelectedTab == value)
                    return;
                _SelectedTab = value;
                RaisePropertyChanged("SelectedTab");

                this.ReplyCommand.RaiseCanExecuteChanged();
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

        private TabViewModel CreateTabViewModel(Tab model)
        {
            var re = new TabViewModel(model);
            ViewModelHelper.BindNotifyChanged(re, this, (sender, e) =>
            {
                if (e.PropertyName == "SelectedItems")
                {
                    this.ReplyCommand.RaiseCanExecuteChanged();
                }
            });
            return re;
        }

        private void Loaded2()
        {
            this.model.Init();

            this.Tabs = ViewModelHelper.CreateReadOnlyNotificationDispatcherCollection(
                this.model.Tabs,
                this.CreateTabViewModel,
                DispatcherHelper.UIDispatcher
            );

            this.SelectedTab = this.Tabs.FirstOrDefault();
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

        public string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetCustomAttributes(false)
                    .OfType<AssemblyInformationalVersionAttribute>()
                    .Select(_ => _.InformationalVersion)
                    .Single();
            }
        }
        
        #region PostText変更通知プロパティ
        string _PostText;

        public string PostText
        {
            get
            { return _PostText; }
            set
            {
                if (_PostText == value)
                    return;
                _PostText = value;
                RaisePropertyChanged("PostText");
                this.PostCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion
        
        #region ReplyToStatus変更通知プロパティ
        TimelineItemViewModel _ReplyToStatus;

        public TimelineItemViewModel ReplyToStatus
        {
            get
            { return _ReplyToStatus; }
            set
            {
                if (_ReplyToStatus == value)
                    return;
                _ReplyToStatus = value;
                RaisePropertyChanged("ReplyToStatus");
            }
        }
        #endregion
      
        #region IsPosting変更通知プロパティ
        bool _IsPosting;

        public bool IsPosting
        {
            get
            { return _IsPosting; }
            set
            {
                if (_IsPosting == value)
                    return;
                _IsPosting = value;
                RaisePropertyChanged("IsPosting");
            }
        }
        #endregion
      
        #region PostCommand
        ViewModelCommand _PostCommand;

        public ViewModelCommand PostCommand
        {
            get
            {
                if (_PostCommand == null)
                    _PostCommand = new ViewModelCommand(Post, CanPost);
                return _PostCommand;
            }
        }

        private bool CanPost()
        {
            return !string.IsNullOrWhiteSpace(this.PostText);
        }

        private void Post()
        {
            this.IsPosting = true;
            this.model.Post(this.PostText, this.ReplyToStatus != null ? this.ReplyToStatus.Model.Id : null)
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        this.Messenger.Raise(new InformationMessage(
                            t.Exception.InnerException.GetMessage(),
                            "投稿失敗",
                            "ShowInfomation"));
                    }
                    else
                    {
                        this.PostText = string.Empty;
                        this.ReplyToStatus = null;
                    }

                    this.IsPosting = false;
                });
        }
        #endregion
        
        #region ReplyCommand
        ViewModelCommand _ReplyCommand;

        public ViewModelCommand ReplyCommand
        {
            get
            {
                if (_ReplyCommand == null)
                    _ReplyCommand = new ViewModelCommand(Reply, CanReply);
                return _ReplyCommand;
            }
        }

        private bool CanReply()
        {
            return this.SelectedTab != null
                && this.SelectedTab.SelectedItems != null
                && this.SelectedTab.SelectedItems.Cast<TimelineItemViewModel>()
                    .Any(item => item.Model.Base is global::LinqToTwitter.Status);
        }

        private void Reply()
        {
            var replyTo = this.SelectedTab.SelectedItems
                .Cast<TimelineItemViewModel>()
                .Where(item => item.Model.Base is global::LinqToTwitter.Status);
            this.ReplyToStatus = replyTo.FirstOrDefault();
            this.PostText = string.Join(" ", replyTo.Select(status => "@" + status.FromScreenName))
                + " " + this.PostText;
        }
        #endregion
      
    }
}
