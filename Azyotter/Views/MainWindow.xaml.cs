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
    }
}
