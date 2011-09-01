using System;
using System.IO;
using System.Reflection;
using System.Windows.Markup;
using Livet;

namespace Azyobuzi.Azyotter.Models
{
    public class Settings : NotificationObject
    {
        private static readonly string SettingsFileName = Assembly.GetExecutingAssembly() != null
            ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Settings.xaml"
            : "Settings.xaml";

        private static bool createdInstance = false;
        public Settings()
        {
            if (createdInstance)
            {
                throw new InvalidOperationException("既にインスタンスが作成されています。");
            }

            createdInstance = true;
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
                        using (var fs = new FileStream(SettingsFileName, FileMode.Open, FileAccess.Read))
                        {
                            instance = (Settings)XamlReader.Load(fs);
                        }
                    }
                    catch { }
                }

                return instance = instance ?? new Settings();
            }
        }

        public void Save()
        {
            using (var fs = new FileStream(SettingsFileName, FileMode.Create, FileAccess.Write))
            {
                XamlWriter.Save(this, fs);
            }
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
      
    }
}
