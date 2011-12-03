namespace Azyobuzi.TaskingTwLib.DataModels.UserStreams
{
    /// <summary>
    /// UserStreamで流れてきた生のデータ
    /// </summary>
    public class RawData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="json">一行のJSON形式の文字列</param>
        public RawData(string json)
        {
            this.Json = json;
            this.Kind = DataKind.Unknown;
        }

        /// <summary>
        /// 一行のJSON形式の文字列
        /// </summary>
        public string Json { get; private set; }

        /// <summary>
        /// UserStreamで流れてきたデータの種類
        /// </summary>
        public DataKind Kind { get; protected set; }
    }
}
