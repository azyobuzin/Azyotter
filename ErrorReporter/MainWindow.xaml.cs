using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Xml.Linq;

namespace Azyobuzi.Azyotter.ErrorReporter
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

        private string azyotterExePath;
        private string logFile;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length != 3)
            {
                MessageBox.Show("引数が正しくありません。",
                    "エラーレポート ツール",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Environment.Exit(1);
            }

            this.azyotterExePath = args[1];
            this.logFile = args[2];
        }

        private bool SendReport()
        {
            if (!this.sendReportCheckBox.IsChecked.IsTrue())
                return true;

            using (var sw = File.AppendText(this.logFile))
            {
                sw.WriteLine();
                sw.WriteLine("Comment ->");
                sw.WriteLine(this.commentTextBox.Text);
            }

            using (var wc = new WebClient())
            {
                try
                {
                    wc.UploadFile(
                        "http://www.azyobuzi.net/projects/azyotter/error_report.cgi",
                        this.logFile
                    );
                    return true;
                }
                catch (WebException ex)
                {
                    string message;
                    try
                    {
                        message = XElement
                            .Load(ex.Response.GetResponseStream())
                            .Value;
                    }
                    catch
                    {
                        message = ex.Message;
                    }
                    MessageBox.Show(message, "送信失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.SendReport())
                return;

            Process.Start(this.azyotterExePath);
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.SendReport())
                return;

            this.Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad", @"""" + logFile + @"""");
        }
    }
}
