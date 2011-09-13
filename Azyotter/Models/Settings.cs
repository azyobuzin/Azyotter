using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Xaml;
using Livet;

namespace Azyobuzi.Azyotter.Models
{
    public class Settings : NotificationObject
    {
        private static readonly string SettingsFileName = Assembly.GetExecutingAssembly() != null
            ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Settings.xaml"
            : "Settings.xaml";

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
                        instance.Tabs.Add(new TabSettings() { Name = "Home", Type = TimelineTypes.Home });
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
        
        #region OAuthToken変更通知プロパティ
        string _OAuthToken;

        public string OAuthToken
        {
            get
            { return _OAuthToken; }
            set
            {
                if (_OAuthToken == value)
                    return;
                _OAuthToken = value;
                RaisePropertyChanged("OAuthToken");
            }
        }
        #endregion
        
        #region OAuthTokenSecret変更通知プロパティ
        string _OAuthTokenSecret;

        public string OAuthTokenSecret
        {
            get
            { return _OAuthTokenSecret; }
            set
            {
                if (_OAuthTokenSecret == value)
                    return;
                _OAuthTokenSecret = value;
                RaisePropertyChanged("OAuthTokenSecret");
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
        ObservableCollection<TabSettings> _Tabs = new ObservableCollection<TabSettings>();

        public ObservableCollection<TabSettings> Tabs
        {
            get
            { return _Tabs; }
            set
            {
                if (_Tabs == value)
                    return;
                _Tabs = value;
                RaisePropertyChanged("Tabs");
            }
        }
        #endregion
      
    }
}
