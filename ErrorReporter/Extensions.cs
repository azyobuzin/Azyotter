namespace Azyobuzi.Azyotter.ErrorReporter
{
    static class Extensions
    {
        public static bool IsTrue(this bool? source)
        {
            return source.HasValue && source.Value;
        }
    }
}
