using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Livet;

namespace Azyobuzi.Azyotter
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DispatcherHelper.UIDispatcher = Dispatcher;
            
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif

            //アップデーターを削除
            e.Args.Where(arg => arg.StartsWith("/u:"))
                .Select(arg => arg.TrimStart("/u:".ToCharArray()))
                .ForEach(dir => Directory.Delete(dir, true));
        }

        private string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetCustomAttributes(false)
                .OfType<AssemblyInformationalVersionAttribute>()
                .Select(_ => _.InformationalVersion)
                .Single();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var tempFile = Path.GetTempFileName();
            using (var sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("Error Report " + DateTime.Now.ToString());
                sw.WriteLine("Azyotter " + this.GetVersion());
                sw.WriteLine("OS:" + Environment.OSVersion.VersionString);
                sw.WriteLine("64bit:" + Environment.Is64BitOperatingSystem.ToString());
                sw.WriteLine();
                sw.WriteLine("Exception.ToString() ->");
                sw.WriteLine(e.ExceptionObject.ToString());
            }

            Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ErrorReporter.exe",
                string.Format(@"""{0}"" ""{1}""", Process.GetCurrentProcess().MainModule.FileName, tempFile));

            Environment.Exit(1);
        }
    }
}
