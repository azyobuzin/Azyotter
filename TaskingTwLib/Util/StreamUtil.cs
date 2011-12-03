using System.IO;
using System.Text;

namespace Azyobuzi.TaskingTwLib.Util
{
    static class StreamUtil
    {
        public static void WriteAll(this Stream target, byte[] buffer)
        {
            target.Write(buffer, 0, buffer.Length);
        }

        public static void WriteText(this Stream target, string text)
        {
            target.WriteAll(Encoding.UTF8.GetBytes(text));
        }
    }
}
