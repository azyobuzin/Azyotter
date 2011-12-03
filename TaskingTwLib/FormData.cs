namespace Azyobuzi.TaskingTwLib
{
    /// <summary>
    /// フォームのパラメータ
    /// </summary>
    public class FormData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">パラメータ名</param>
        /// <param name="content">内容 or ファイル名</param>
        /// <param name="isFile">ファイルかどうか</param>
        /// <param name="mimeType">メディアタイプ</param>
        public FormData(string name, string content, bool isFile = false, string mimeType = null)
        {
            this.Name = name;
            this.Content = content;
            this.IsFile = isFile;
        }

        /// <summary>
        /// パラメータ名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 内容 or ファイル名
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// ファイルかどうか
        /// </summary>
        public bool IsFile { get; set; }

        /// <summary>
        /// メディアタイプ
        /// </summary>
        public string ContentType { get; set; }
    }
}
