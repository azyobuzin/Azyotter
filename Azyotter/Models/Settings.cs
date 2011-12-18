using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xaml;
using Livet;

namespace Azyobuzi.Azyotter.Models
{
    public class Settings : NotificationObject
    {
        private static readonly string SettingsFileName =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Settings.xaml";

        public Settings()
        {
        }

        private static Settings instance;
        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    try
                    {
                        instance = (Settings)XamlServices.Load(SettingsFileName);
                    }
                    catch
                    {
                        instance = new Settings();
                        instance.Tabs.Add(new TabSetting() { Name = "Home", Type = TimelineTypes.Home });
                        instance.Tabs.Add(new TabSetting() { Name = "Mentions", Type = TimelineTypes.Mentions });
                    }
                }

                return instance;
            }
        }

        public void Save()
        {
            XamlServices.Save(SettingsFileName, this);
        }
        
        #region ConsumerKey変更通知プロパティ
        string _ConsumerKey = "xx0RBhnHRa0FYfPuDLlBNg";

        public string ConsumerKey
        {
            get
            { return _ConsumerKey; }
            set
            {
                if (_ConsumerKey == value)
                    return;
                _ConsumerKey = value;
                RaisePropertyChanged("ConsumerKey");
            }
        }
        #endregion
        
        #region ConsumerSecret変更通知プロパティ
        string _ConsumerSecret = "TSMrMmc6uIxT2l8o7p9aC3mxHVMDzp17eXInyuSZgk";

        public string ConsumerSecret
        {
            get
            { return _ConsumerSecret; }
            set
            {
                if (_ConsumerSecret == value)
                    return;
                _ConsumerSecret = value;
                RaisePropertyChanged("ConsumerSecret");
            }
        }
        #endregion

        #region Accounts変更通知プロパティ
        ObservableSynchronizedCollection<Account> _Accounts = new ObservableSynchronizedCollection<Account>();

        public ObservableSynchronizedCollection<Account> Accounts
        {
            get
            { return _Accounts; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (_Accounts == value)
                    return;
                _Accounts = value;
                RaisePropertyChanged("Accounts");
            }
        }
        #endregion

        #region AutoUpdate変更通知プロパティ
        private bool _AutoUpdate = true;

        public bool AutoUpdate
        {
            get
            { return _AutoUpdate; }
            set
            {
                if (EqualityComparer<bool>.Default.Equals(_AutoUpdate, value))
                    return;
                _AutoUpdate = value;
                RaisePropertyChanged("AutoUpdate");
            }
        }
        #endregion

        #region UseUserStream変更通知プロパティ
        bool _UseUserStream = true;

        public bool UseUserStream
        {
            get
            { return _UseUserStream; }
            set
            {
                if (_UseUserStream == value)
                    return;
                _UseUserStream = value;
                RaisePropertyChanged("UseUserStream");
            }
        }
        #endregion
        
        #region Tabs変更通知プロパティ
        ObservableCollection<TabSetting> _Tabs = new ObservableCollection<TabSetting>();

        public ObservableCollection<TabSetting> Tabs
        {
            get
            { return _Tabs; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (_Tabs == value)
                    return;
                _Tabs = value;
                RaisePropertyChanged("Tabs");
            }
        }
        #endregion
        
        #region ShorcutKeys変更通知プロパティ
        ShortcutKeys.ShortcutKeysSetting _ShorcutKeys = new ShortcutKeys.ShortcutKeysSetting();

        public ShortcutKeys.ShortcutKeysSetting ShorcutKeys
        {
            get
            { return _ShorcutKeys; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (_ShorcutKeys == value)
                    return;
                _ShorcutKeys = value;
                RaisePropertyChanged("ShorcutKeys");
            }
        }
        #endregion

        #region Footer変更通知プロパティ
        private string _Footer;

        public string Footer
        {
            get
            { return _Footer; }
            set
            { 
                if (EqualityComparer<string>.Default.Equals(_Footer, value))
                    return;
                _Footer = value;
                RaisePropertyChanged("Footer");
            }
        }
        #endregion

        #region UsingAccount変更通知プロパティ
        private long _UsingAccount;

        public long UsingAccount
        {
            get
            { return _UsingAccount; }
            set
            { 
                if (EqualityComparer<long>.Default.Equals(_UsingAccount, value))
                    return;
                _UsingAccount = value;
                RaisePropertyChanged("UsingAccount");
            }
        }
        #endregion

        public Account GetUsingAccount()
        {
            return this.Accounts.FirstOrDefault(a => a.UserId == this.UsingAccount)
                ?? this.Accounts.FirstOrDefault();
        }

        public void SetUsingAccount(Account account)
        {
            this.UsingAccount = account.UserId;
        }
    }
}
