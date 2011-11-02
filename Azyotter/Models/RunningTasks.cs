using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Livet;

namespace Azyobuzi.Azyotter.Models
{
    public class RunningTasks : ObservableSynchronizedCollection<RunningTask>
    {
        private RunningTasks() { }

        private static RunningTasks instance = new RunningTasks();
        public static RunningTasks Instance
        {
            get
            {
                return instance;
            }
        }
    }

    public class RunningTask : NotificationObject
    {
        public RunningTask(string description, Task task, CancellationTokenSource cancellationTokenSource)
        {
            this.Description = description;
            this.Task = task;
            this.CancellationTokenSource = cancellationTokenSource;
        }

        #region Description変更通知プロパティ
        private string _Description;

        public string Description
        {
            get
            { return _Description; }
            set
            { 
                if (EqualityComparer<string>.Default.Equals(_Description, value))
                    return;
                _Description = value;
                RaisePropertyChanged("Description");
            }
        }
        #endregion

        public Task Task { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }
    }
}
