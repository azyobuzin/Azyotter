using System.Linq;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.DataModels.UserStreams
{
    /// <summary>
    /// ツイートまたはダイレクトメッセージが削除された情報
    /// </summary>
    public class DeleteData : RawData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="json">一行のJSON形式の文字列</param>
        public DeleteData(string json)
            : base(json)
        {
            var elm = SerializationHelper.JsonToXml(json)
                .Element("delete")
                .Elements()
                .First();

            this.Kind = elm.Name.LocalName == "status" ? DataKind.DeleteStatus : DataKind.DeleteDirectMessage;

            this.Id = (ulong)elm.Element("id");
            this.UserId = (long)elm.Element("user_id");
        }

        /// <summary>
        /// 削除されたツイート・ダイレクトメッセージのID
        /// </summary>
        public ulong Id { get; private set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        public long UserId { get; private set; }
    }
}
