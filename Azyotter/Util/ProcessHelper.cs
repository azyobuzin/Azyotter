using System.Diagnostics;

namespace Azyobuzi.Azyotter.Util
{
    public static class ProcessHelper
    {
        public static void Start(string fileName)
        {
            try
            {
                Process.Start(fileName);
            }
            catch { }
        }
    }
}
