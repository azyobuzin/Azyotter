using System.Collections.Generic;
using System.Windows;
using Azyobuzi.Azyotter.Models;
using Azyobuzi.Azyotter.Util;
using Azyobuzi.TaskingTwLib;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using TwitterApi = Azyobuzi.TaskingTwLib.Methods;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class AccountsManageWindowViewModel : ViewModel
    {
        public AccountsManageWindowViewModel()
        {
            this.Accounts = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                Settings.Instance.Accounts,
                (Account a) =>
                {
                    var vm = new AccountViewModel(a);
                    ViewModelHelper.BindNotification(vm.RemovedEvent, this, (sender, e) =>
                    {
                        if (this.Accounts.Count == 1)
                        {
                            this.Messenger.Raise(new InformationMessage(
                                "唯一のアカウントを削除することはできません。",
                                "削除できません",
                                MessageBoxImage.Error,
                                "ShowInformation"
                            ));
                        }
                        else
                        {
                            Settings.Instance.Accounts.Remove(vm.Model);
                        }
                    });

                    return vm;
                },
                DispatcherHelper.UIDispatcher
            );
        }

        #region Accounts変更通知プロパティ
        private ReadOnlyDispatcherCollection<AccountViewModel> _Accounts;

        public ReadOnlyDispatcherCollection<AccountViewModel> Accounts
        {
            get
            { return _Accounts; }
            private set
            { 
                if (EqualityComparer<ReadOnlyDispatcherCollection<AccountViewModel>>.Default.Equals(_Accounts, value))
                    return;
                _Accounts = value;
                RaisePropertyChanged("Accounts");
            }
        }
        #endregion

        #region Authenticating変更通知プロパティ
        private bool _Authenticating;

        public bool Authenticating
        {
            get
            { return _Authenticating; }
            private set
            { 
                if (EqualityComparer<bool>.Default.Equals(_Authenticating, value))
                    return;
                _Authenticating = value;
                RaisePropertyChanged("Authenticating");
            }
        }
        #endregion

        #region StartAuthenticationCommand
        private ViewModelCommand _StartAuthenticationCommand;

        public ViewModelCommand StartAuthenticationCommand
        {
            get
            {
                if (_StartAuthenticationCommand == null)
                {
                    _StartAuthenticationCommand = new ViewModelCommand(StartAuthentication, CanStartAuthentication);
                }
                return _StartAuthenticationCommand;
            }
        }

        public bool CanStartAuthentication()
        {
            return !this.GettingRequestToken;
        }

        public void StartAuthentication()
        {
            this.GettingRequestToken = true;
            TwitterApi.OAuth.RequestTokenApi.Create()
                .CallApi(new Token()
                {
                    ConsumerKey = Settings.Instance.ConsumerKey,
                    ConsumerSecret = Settings.Instance.ConsumerSecret
                })
                .ContinueWith(t =>
                {
                    this.GettingRequestToken = false;

                    if (t.Exception != null)
                    {
                        this.Messenger.Raise(new InformationMessage(
                            "リクエストトークンを取得できませんでした。",
                            "認証失敗",
                            MessageBoxImage.Error,
                            "ShowInformation"
                        ));
                        return;
                    }

                    this.requestToken = t.Result;

                    this.Authenticating = true;
                    this.Messenger.Raise(new InteractionMessage("FocusToPinCodeBox"));

                    ProcessHelper.Start("https://api.twitter.com/oauth/authorize?oauth_token=" + this.requestToken.OAuthToken);
                });
        }
        #endregion

        #region GettingRequestToken変更通知プロパティ
        private bool _GettingRequestToken;

        public bool GettingRequestToken
        {
            get
            { return _GettingRequestToken; }
            private set
            { 
                if (EqualityComparer<bool>.Default.Equals(_GettingRequestToken, value))
                    return;
                _GettingRequestToken = value;
                RaisePropertyChanged("GettingRequestToken");
                this.StartAuthenticationCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        private Token requestToken;

        #region PinCode変更通知プロパティ
        private string _PinCode;

        public string PinCode
        {
            get
            { return _PinCode; }
            set
            { 
                if (EqualityComparer<string>.Default.Equals(_PinCode, value))
                    return;
                _PinCode = value;
                RaisePropertyChanged("PinCode");
                this.SubmitAuthenticationCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region GettingAccessToken変更通知プロパティ
        private bool _GettingAccessToken;

        public bool GettingAccessToken
        {
            get
            { return _GettingAccessToken; }
            set
            { 
                if (EqualityComparer<bool>.Default.Equals(_GettingAccessToken, value))
                    return;
                _GettingAccessToken = value;
                RaisePropertyChanged("GettingAccessToken");
            }
        }
        #endregion

        #region SubmitAuthenticationCommand
        private ViewModelCommand _SubmitAuthenticationCommand;

        public ViewModelCommand SubmitAuthenticationCommand
        {
            get
            {
                if (_SubmitAuthenticationCommand == null)
                {
                    _SubmitAuthenticationCommand = new ViewModelCommand(SubmitAuthentication, CanSubmitAuthentication);
                }
                return _SubmitAuthenticationCommand;
            }
        }

        public bool CanSubmitAuthentication()
        {
            return !string.IsNullOrWhiteSpace(this.PinCode);
        }

        public void SubmitAuthentication()
        {
            this.GettingAccessToken = true;
            TwitterApi.OAuth.AccessTokenApi.Create(this.PinCode.Trim())
                .CallApi(this.requestToken)
                .ContinueWith(t =>
                {
                    this.GettingAccessToken = false;

                    if (t.Exception != null)
                    {
                        this.Messenger.Raise(new InformationMessage(
                            "アクセストークンを取得できませんでした。",
                            "認証失敗",
                            MessageBoxImage.Error,
                            "ShowInformation"
                        ));
                        return;
                    }

                    Settings.Instance.Accounts.Add(new Account()
                    {
                        OAuthToken = t.Result.Item1.OAuthToken,
                        OAuthTokenSecret = t.Result.Item1.OAuthTokenSecret,
                        ScreenName = t.Result.Item2.ScreenName,
                        UserId = t.Result.Item2.Id
                    });

                    this.CancelAuthentication();
                });
        }
        #endregion

        #region CancelAuthenticationCommand
        private ViewModelCommand _CancelAuthenticationCommand;

        public ViewModelCommand CancelAuthenticationCommand
        {
            get
            {
                if (_CancelAuthenticationCommand == null)
                {
                    _CancelAuthenticationCommand = new ViewModelCommand(CancelAuthentication);
                }
                return _CancelAuthenticationCommand;
            }
        }

        public void CancelAuthentication()
        {
            this.Authenticating = false;
            this.PinCode = string.Empty;
        }
        #endregion

    }
}
