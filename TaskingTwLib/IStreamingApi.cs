using System.Collections.Generic;

namespace Azyobuzi.TaskingTwLib
{
    /// <summary>
    /// Streaming APIのリクエストを表すインターフェイス
    /// </summary>
    /// <typeparam name="TResult">APIからの戻り値の型</typeparam>
    public interface IStreamingApi<TResult>
    {
        /// <summary>
        /// APIのURI
        /// </summary>
        string RequestUri { get; }

        /// <summary>
        /// パラメータ
        /// </summary>
        IEnumerable<FormData> Parameters { get; }

        /// <summary>
        /// レスポンスの一行を<typeparamref name="TResult"/>に変換します。
        /// </summary>
        /// <param name="line">レスポンスの一行</param>
        /// <returns>変換されたデータ</returns>
        TResult Parse(string line);
    }
}
