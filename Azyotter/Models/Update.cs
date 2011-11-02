using System;
using System.Diagnostics;
using System.IO;
using Azyobuzi.Azyotter.Util;

namespace Azyobuzi.Azyotter.Models
{
    static class Update
    {
        private static Updater.Update latest;
        public static Updater.Update Latest
        {
            get
            {
                return latest;
            }
            private set
            {
                if (latest != value)
                {
                    latest = value;

                    if (LatestChanged != null)
                        LatestChanged(null, EventArgs.Empty);
                }
            }
        }

        public static event EventHandler LatestChanged;

        private static bool canUpdate;
        public static bool CanUpdate
        {
            get
            {
                return canUpdate;
            }
            private set
            {
                if (canUpdate != value)
                {
                    canUpdate = value;

                    if (CanUpdateChanged != null)
                        CanUpdateChanged(null, EventArgs.Empty);
                }
            }
        }

        public static event EventHandler CanUpdateChanged;

        public static bool GetCanUpdate()
        {
            bool result;

            try
            {
                Latest = Updater.Updates.GetUpdates().Latest();
                result = Latest.Version > Version.Parse(AssemblyUtil.GetInformationalVersion());
            }
            catch
            {
                return false;
            }

            CanUpdate = result;

            return result;
        }

        public static void ExecuteUpdate()
        {
            var asmDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var tmpDir = Directory.CreateDirectory(Path.GetTempPath() + Guid.NewGuid().ToString());

            File.Copy(Path.Combine(asmDir, "Updater.exe"), Path.Combine(tmpDir.FullName, "Updater.exe"), true);
            File.Copy(Path.Combine(asmDir, "Updater.pdb"), Path.Combine(tmpDir.FullName, "Updater.pdb"), true);
            File.Copy(Path.Combine(asmDir, "System.Windows.Interactivity.dll"), Path.Combine(tmpDir.FullName, "System.Windows.Interactivity.dll"), true);
            File.Copy(Path.Combine(asmDir, "Ionic.Zip.dll"), Path.Combine(tmpDir.FullName, "Ionic.Zip.dll"), true);

            Process.Start(Path.Combine(tmpDir.FullName, "Updater.exe"),
                string.Format(@"""{0}"" ""{1}""",
                    Process.GetCurrentProcess().MainModule.FileName,
                    Latest.Id));

            if (ExitRequest != null)
                ExitRequest(null, EventArgs.Empty);
        }

        public static event EventHandler ExitRequest;
    }
}
