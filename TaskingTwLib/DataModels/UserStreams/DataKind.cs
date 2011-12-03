namespace Azyobuzi.TaskingTwLib.DataModels.UserStreams
{
    /// <summary>
    /// UserStreamで流れてきたデータの種類
    /// </summary>
    public enum DataKind
    {
        /// <summary>
        /// 不明
        /// </summary>
        Unknown,

        /// <summary>
        /// フォローしているアカウントのID
        /// </summary>
        Friends,

        /// <summary>
        /// ツイート
        /// </summary>
        Status,

        /// <summary>
        /// ダイレクトメッセージ
        /// </summary>
        DirectMessage,

        /// <summary>
        /// ツイートが削除された
        /// </summary>
        DeleteStatus,

        /// <summary>
        /// ダイレクトメッセージが削除された
        /// </summary>
        DeleteDirectMessage,

        /// <summary>
        /// イベント
        /// </summary>
        Event
    }
}
