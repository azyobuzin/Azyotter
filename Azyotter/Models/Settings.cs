using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Livet;
using System.Reflection;
using System.IO;

namespace Azyobuzi.Azyotter.Models
{
    public class Settings : NotificationObject
    {
        private static readonly string SettingsFileName = Assembly.GetExecutingAssembly() != null
            ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "Settings.xaml"
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
      
    }
}
