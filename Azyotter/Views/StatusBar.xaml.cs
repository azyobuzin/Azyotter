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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Azyobuzi.Azyotter.Views
{
    /// <summary>
    /// StatusBar.xaml の相互作用ロジック
    /// </summary>
    public partial class StatusBar : UserControl
    {
        public StatusBar()
        {
            InitializeComponent();
        }

        private void mainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = (ContextMenu)this.mainMenuButton.Resources["mainContextMenu"];
            contextMenu.PlacementTarget = this.mainMenuButton;
            contextMenu.IsOpen = true;
        }
    }
}
