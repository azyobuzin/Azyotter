using System.Windows;
using System.Windows.Controls;
using Azyobuzi.Azyotter.Models;

namespace Azyobuzi.Azyotter.Views
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void deleteTabButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Instance.Tabs.Remove((TabSetting)((Button)sender).DataContext);
        }

        private void addTabButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Instance.Tabs.Add(new TabSetting());
        }
    }
}