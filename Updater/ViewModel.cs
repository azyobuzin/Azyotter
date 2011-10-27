using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Azyobuzi.Azyotter.Updater
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(
                    ((MemberExpression)property.Body).Member.Name
                ));
        }

        private string message = "アップデーターを準備しています...";
        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.OnPropertyChanged(() => this.Message);
                }
            }
        }

        private int progressPercentage;
        public int ProgressPercentage
        {
            get
            {
                return this.progressPercentage;
            }
            set
            {
                if (this.progressPercentage != value)
                {
                    this.progressPercentage = value;
                    this.OnPropertyChanged(() => this.ProgressPercentage);
                }
            }
        }

        private AlwaysExecutableCommand beginUpdateCommand;
        public AlwaysExecutableCommand BeginUpdateCommand
        {
            get
            {
                return this.beginUpdateCommand = this.beginUpdateCommand ?? new AlwaysExecutableCommand(() =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        var disp = Application.Current.Dispatcher;

                        string azyotterExePath;
                        string packageId;
                        Update targetUpdate;
                        string tmpFile;

                        try
                        {
                            azyotterExePath = Environment.GetCommandLineArgs()[1];
                            packageId = Environment.GetCommandLineArgs()[2];

                            if (!File.Exists(azyotterExePath))
                                throw new Exception();
                        }
                        catch
                        {
                            this.Failed(disp, "引数が正しくないためアップデートを実行できませんでした。");
                            return;
                        }

                        DispatcherInvoke(disp, () => this.Message = "アップデート情報を取得しています...");

                        try
                        {
                            targetUpdate = Updates.GetUpdates().PackageId(packageId);
                        }
                        catch
                        {
                            this.Failed(disp, "アップデート情報の取得に失敗しました。");
                            return;
                        }

                        DispatcherInvoke(disp, () => this.Message = "ダウンロードしています...");

                        try
                        {
                            tmpFile = UpdateCore.Download(targetUpdate, percentage =>
                                DispatcherInvoke(disp, () => this.ProgressPercentage = percentage));
                        }
                        catch
                        {
                            this.Failed(disp, "ダウンロードに失敗しました。");
                            return;
                        }

                        DispatcherInvoke(disp, () =>
                        {
                            this.ProgressPercentage = 0;
                            this.Message = "Azyotterの終了を待機しています...";
                        });

                        while (true)
                        {
                            var running = Process.GetProcesses()
                                .Where(process =>
                                {
                                    try
                                    {
                                        return process.MainModule.FileName == azyotterExePath;
                                    }
                                    catch
                                    {
                                        return false;
                                    }
                                })
                                .Any();

                            if (!running)
                                break;

                            Thread.Sleep(500);
                        }

                        DispatcherInvoke(disp, () => this.Message = "アップデートを適用しています...");

                        try
                        {
                            UpdateCore.Apply(tmpFile, Path.GetDirectoryName(azyotterExePath), percentage =>
                                DispatcherInvoke(disp, () => this.ProgressPercentage = percentage));
                        }
                        catch
                        {
                            this.Failed(disp, "アップデートの適用に失敗しました。");
                            return;
                        }
                        finally
                        {
                            File.Delete(tmpFile);
                        }
                        
                        DispatcherInvoke(disp, () => this.Message = "新しいAzyotterを起動しています...");

                        Process.Start(azyotterExePath, string.Format(@"/u:{0}",
                            Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)));

                        ThreadPool.QueueUserWorkItem((state) =>
                        {
                            Thread.Sleep(2000);
                            disp.BeginInvoke(new Action(Application.Current.Shutdown));
                        });
                    });
                });
            }
        }

        private void Failed(Dispatcher disp, string message)
        {
            DispatcherInvoke(disp, () =>
            {
                this.Message = message;
                this.ProgressPercentage = 100;
            });

            ThreadPool.QueueUserWorkItem((state) =>
            {
                Thread.Sleep(5000);
                disp.BeginInvoke(new Action(Application.Current.Shutdown));
            });
        }

        private void DispatcherInvoke(Dispatcher disp, Action action)
        {
            disp.Invoke(action);
        }
    }
}
