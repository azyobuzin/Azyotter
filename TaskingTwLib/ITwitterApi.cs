using System.Collections.Generic;

namespace Azyobuzi.TaskingTwLib
{
    /// <summary>
    /// TwitterAPIのリクエストを表すインターフェイス
    /// </summary>
    /// <typeparam name="TResult">APIからの戻り値の型</typeparam>
    public interface ITwitterApi<TResult>
    {
        /// <summary>
        /// APIのURI
        /// </summary>
        string RequestUri { get; }

        /// <summary>
        /// HTTPメソッド
        /// </summary>
        HttpMethodType MethodType { get; }

        /// <summary>
        /// パラメータ
        /// </summary>
        IEnumerable<FormData> Parameters { get; }

        /// <summary>
        /// メディアタイプ
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// OAuth Verifier
        /// </summary>
        string Verifier { get; }

        /// <summary>
        /// 認証のコールバック先
        /// </summary>
        string Callback { get; }

        /// <summary>
        /// レスポンスを<typeparamref name="TResult"/>に変換します。
        /// </summary>
        /// <param name="response">レスポンスの文字列</param>
        /// <param name="token">OAuthトークン</param>
        /// <returns>変換されたデータ</returns>
        TResult Parse(string response, Token token);
    }
}
