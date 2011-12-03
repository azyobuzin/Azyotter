namespace Azyobuzi.TaskingTwLib.DataModels.UserStreams
{
    /// <summary>
    /// UserStreamで流れてきたツイート
    /// </summary>
    public class StatusData : RawData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="json">一行のJSON形式の文字列</param>
        public StatusData(string json)
            : base(json)
        {
            this.Kind = DataKind.Status;
            this.Status = Status.Create(json);
        }

        /// <summary>
        /// ツイート
        /// </summary>
        public Status Status { get; private set; }
    }
}
