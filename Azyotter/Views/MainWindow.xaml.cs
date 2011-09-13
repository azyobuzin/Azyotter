using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Azyobuzi.Azyotter.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TabItem_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            var tabItem = sender as TabItem;
            if (e.LeftButton == MouseButtonState.Pressed && tabItem != null)
            {
                tabItem.IsSelected = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SystemMenuManager.AddSeparator(this);
            SystemMenuManager.AddMenuItem(this, 1, "設定(&S)...", () =>
            {
                var w = Application.Current.Windows.OfType<SettingsWindow>().FirstOrDefault();
                if (w != null)
                {
                    w.Activate();
                }
                else
                {
                    w = new SettingsWindow();
                    w.Owner = this;
                    w.Show();
                }
            });
        }
    }
}
