using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Azyobuzi.Azyotter.Models;
using Azyobuzi.Azyotter.Util;
using Azyobuzi.TaskingTwLib;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.Windows;
using TwitterApi = Azyobuzi.TaskingTwLib.Methods;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel()
        {
            ViewModelHelper.BindNotifyChanged(Settings.Instance, this, (sender, e) =>
            {
                if (e.PropertyName == "ShorcutKeys")
                    this.RaisePropertyChanged(() => this.ShortcutKeys);
            });

            ViewModelHelper.BindNotifyChanged(this.model, this, (sender, e) =>
            {
                if (e.PropertyName == "Status")
                    this.RaisePropertyChanged(() => this.Status);
            });
        }

        private Model model = new Model();
                
        #region Tabs変更通知プロパティ
        ReadOnlyDispatcherCollection<TabViewModel> _Tabs;

        public ReadOnlyDispatcherCollection<TabViewModel> Tabs
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
            if (string.IsNullOrEmpty(Settings.Instance.Accounts.First().OAuthToken)||string.IsNullOrEmpty(Settings.Instance.Accounts.First().OAuthTokenSecret))
            {
                Token token = new Token()
                {
                    ConsumerKey = Settings.Instance.ConsumerKey,
                    ConsumerSecret = Settings.Instance.ConsumerSecret
                };
                TwitterApi.OAuth.RequestTokenApi.Create()
                    .CallApi(token)
                    .ContinueWith(t =>
                    {
                        token = t.Result;

                        Process.Start("https://api.twitter.com/oauth/authorize?oauth_token=" + token.OAuthToken);

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
                                        var id = TwitterApi.OAuth.AccessTokenApi
                                            .Create(vm.Pin)
                                            .CallApi(token)
                                            .Result;
                                        this.model.SaveOAuthToken(id.Item1, id.Item2.Id, id.Item2.ScreenName);                                            
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
                    });

                
            }
            else
            {
                Loaded2();
            }
        }
        #endregion

        private TabViewModel CreateTabViewModel(Tab model)
        {
            var re = new TabViewModel(model, this.Messenger);
            ViewModelHelper.BindNotifyCollectionChanged(re.SelectedItems, this, (sender, e) =>
            {
                this.ReplyCommand.RaiseCanExecuteChanged();
            });
            return re;
        }

        private void Loaded2()
        {
            this.model.Init();

            this.Tabs = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                this.model.Tabs,
                (Tab model) => this.CreateTabViewModel(model),
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
                return AssemblyUtil.GetInformationalVersion();
            }
        }

        public Models.ShortcutKeys.ShortcutKeysSetting ShortcutKeys
        {
            get
            {
                return Settings.Instance.ShorcutKeys;
            }
        }

        public string Status
        {
            get
            {
                return this.model.Status;
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
            this.model.Post(this.PostText, this.ReplyToStatus != null ? (ulong?)this.ReplyToStatus.Model.Id : null, true)
                .ContinueWith(t =>
                {
                    if(t.Result)
                    {
                        this.PostText = string.Empty;
                        this.ReplyToStatus = null;
                    }

                    this.IsPosting = false;
                });
        }
        #endregion

        #region PostWithoutFooterCommand
        private Livet.Commands.ViewModelCommand _PostWithoutFooterCommand;

        public Livet.Commands.ViewModelCommand PostWithoutFooterCommand
        {
            get
            {
                if (_PostWithoutFooterCommand == null)
                {
                    _PostWithoutFooterCommand = new Livet.Commands.ViewModelCommand(PostWithoutFooter, CanPostWithoutFooter);
                }
                return _PostWithoutFooterCommand;
            }
        }

        public bool CanPostWithoutFooter()
        {
            return !string.IsNullOrWhiteSpace(this.PostText);
        }

        public void PostWithoutFooter()
        {
            this.IsPosting = true;
            this.model.Post(this.PostText, this.ReplyToStatus != null ? (ulong?)this.ReplyToStatus.Model.Id : null, false)
                .ContinueWith(t =>
                {
                    if (t.Result)
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
                    .Any(item => item.Model is Models.TwitterDataModels.Status);
        }

        private void Reply()
        {
            var replyTo = this.SelectedTab.SelectedItems
                .Cast<TimelineItemViewModel>()
                .Where(item => item.Model is Models.TwitterDataModels.Status);
            this.ReplyToStatus = replyTo.FirstOrDefault();
            this.PostText = string.Join(" ", replyTo.Select(status => "@" + status.FromScreenName))
                + " " + this.PostText;
        }
        #endregion

        #region AddTabCommand
        private ViewModelCommand _AddTabCommand;

        public ViewModelCommand AddTabCommand
        {
            get
            {
                if (_AddTabCommand == null)
                {
                    _AddTabCommand = new ViewModelCommand(AddTab);
                }
                return _AddTabCommand;
            }
        }

        public void AddTab()
        {
            var tab = new TabSetting();
            Settings.Instance.Tabs.Add(tab);
            Settings.Instance.Save();
        }
        #endregion

    }
}
