using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
