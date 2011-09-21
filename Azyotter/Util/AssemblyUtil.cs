using System.Linq;
using System.Reflection;

namespace Azyobuzi.Azyotter.Util
{
    public static class AssemblyUtil
    {
        public static string GetInformationalVersion()
        {
            return Assembly.GetCallingAssembly().GetCustomAttributes(false)
                .OfType<AssemblyInformationalVersionAttribute>()
                .Select(_ => _.InformationalVersion)
                .Single();
        }
    }
}
