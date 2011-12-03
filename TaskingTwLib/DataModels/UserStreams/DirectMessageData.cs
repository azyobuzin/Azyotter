using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.DataModels.UserStreams
{
    /// <summary>
    /// UserStreamで流れてきたダイレクトメッセージ
    /// </summary>
    public class DirectMessageData : RawData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="json">一行のJSON形式の文字列</param>
        public DirectMessageData(string json)
            : base(json)
        {
            this.Kind = DataKind.DirectMessage;
            this.DirectMessage = DirectMessage.Create(
                SerializationHelper.JsonToXml(json)
                    .Element("direct_message")
            );
        }

        /// <summary>
        /// ダイレクトメッセージ
        /// </summary>
        public DirectMessage DirectMessage { get; private set; }
    }
}
