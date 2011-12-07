using System;
using System.IO;
using System.Net;
using System.Threading;
using Ionic.Zip;

namespace Azyobuzi.Azyotter.Updater
{
    public static class UpdateCore
    {
        public static string Download(Update update, Action<int> progressChanged)
        {
            var tmpFile = Path.GetTempFileName();

            var waiter = new ManualResetEvent(false);
            Exception ex = null;

            var wc = new WebClient();

            wc.DownloadProgressChanged += (sender, e) => progressChanged(e.ProgressPercentage);
            wc.DownloadFileCompleted += (sender, e) =>
            {
                ex = e.Error;
                waiter.Set();
            };

            wc.Headers.Add(HttpRequestHeader.UserAgent, "Azyotter.Updater v" + AssemblyUtil.GetInformationalVersion());
            wc.DownloadFileAsync(update.Uri, tmpFile);

            waiter.WaitOne();

            if (ex != null)
            {
                File.Delete(tmpFile);
                throw ex;
            }

            return tmpFile;
        }

        public static void Apply(string zipFile, string targetDir, Action<int> progressChanged)
        {
            using (var zip = ZipFile.Read(zipFile))
            {
                zip.ExtractProgress += (sender, e) =>
                {
                    if (e.EntriesExtracted > 0 && e.EntriesTotal > 0)
                        progressChanged((int)(((double)e.EntriesExtracted / (double)e.EntriesTotal) * 100));
                };

                zip.ExtractAll(targetDir, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}
