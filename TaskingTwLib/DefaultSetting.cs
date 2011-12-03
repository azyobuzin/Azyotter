using System.Net;

namespace Azyobuzi.TaskingTwLib
{
    /// <summary>
    /// 既定の通信設定を行います。
    /// </summary>
    public static class DefaultSetting
    {
        static DefaultSetting()
        {
            Proxy = WebRequest.GetSystemWebProxy();
            Timeout = System.Threading.Timeout.Infinite;
            UserAgent = "TaskingTwLib (This library is testing now)";
        }

        /// <summary>
        /// プロキシ
        /// </summary>
        public static IWebProxy Proxy { get; set; }

        /// <summary>
        /// タイムアウト
        /// </summary>
        public static int Timeout { get; set; }

        /// <summary>
        /// ユーザーエージェント
        /// </summary>
        public static string UserAgent { get; set; }
    }
}
