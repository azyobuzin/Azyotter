using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.OAuth
{
    /// <summary>
    /// OAuthを使ったリクエストを作成します。
    /// </summary>
    public static class RequestGenerator
    {
        static RequestGenerator()
        {
            ServicePointManager.Expect100Continue = false;
        }

        /// <summary>
        /// OAuthを使ったTwitterAPIへのリクエストを作成します。
        /// RequestStreamへの書き込みも行うので非同期推奨。
        /// </summary>
        /// <param name="uri">http(s)://から始まるURI</param>
        /// <param name="methodType">プロコトルメソッド</param>
        /// <param name="token">OAuthトークン</param>
        /// <param name="proxy">プロキシ設定。nullは勘弁。</param>
        /// <param name="timeout">タイムアウト</param>
        /// <param name="userAgent">ユーザーエージェント</param>
        /// <param name="contentType">メディアタイプ。POST・PUTのときのみ指定してください。</param>
        /// <param name="parameters">パラメータ</param>
        /// <param name="verifier">OAuth Verifier</param>
        /// <param name="callback">コールバック。基本oobでOK</param>
        /// <returns>リクエスト</returns>
        public static HttpWebRequest GenerateTwitterApiRequest(string uri, HttpMethodType methodType,
            Token token, IWebProxy proxy, int timeout, string userAgent, string contentType,
            IEnumerable<FormData> parameters, string verifier, string callback)
        {
            parameters = parameters != null ? parameters.ToArray() : new FormData[] { };//遅延実行防止

            var ub = new UriBuilder(uri);
            if ((methodType == HttpMethodType.GET || methodType == HttpMethodType.DELETE))
            {
                if (!string.IsNullOrEmpty(contentType)) throw new ArgumentException("GETまたはDELETEでcontentTypeを指定することはできません。");
                if (parameters.Any(param => param.IsFile)) throw new ArgumentException("GETまたはDELETEでファイルを送ることはできません。");

                ub.Query = string.Join("&",
                    ub.Query.TrimStart('?').Split('&').Concat(
                        parameters.Select(param => string.Format("{0}={1}", Uri.EscapeDataString(param.Name), Uri.EscapeDataString(param.Content)))
                    ));
            }

            var req = (HttpWebRequest)WebRequest.Create(ub.Uri);
            req.Method = methodType.ToString();
            req.Proxy = proxy;
            req.Timeout = timeout;
            req.UserAgent = userAgent;

            req.Headers.Add(
                HttpRequestHeader.Authorization,
                OAuthTwitter.CreateAuthorizationHeader(
                    ub.Uri,
                    null,
                    contentType == HttpContentType.MultipartFormData
                        ? null
                        : parameters.Select(_ =>
                            new KeyValuePair<string, string>(_.Name, _.Content)),
                    token,
                    verifier,
                    callback,
                    methodType));

            if ((methodType == HttpMethodType.POST || methodType == HttpMethodType.PUT) && parameters.Any())
            {
                switch (contentType)
                {
                    case HttpContentType.ApplicationXWwwFormUrlencoded:
                        if (parameters.Any(param => param.IsFile)) throw new ArgumentException("application/x-www-form-urlencodedでファイルを送ることはできません。");

                        req.ContentType = "application/x-www-form-urlencoded";

                        using (var sw = new StreamWriter(req.GetRequestStream()))
                            sw.Write(string.Join("&", parameters.Select(data => Uri.EscapeDataString(data.Name) + "=" + Uri.EscapeDataString(data.Content))));

                        break;
                    case HttpContentType.MultipartFormData:
                        var boundary = Guid.NewGuid().ToString();

                        req.ContentType = "multipart/form-data; boundary=" + boundary;

                        using (var stream = req.GetRequestStream())
                        {
                            foreach (var param in parameters)
                            {
                                stream.WriteText("--" + boundary + "\r\n");

                                var contentDisposition = new List<string>();
                                contentDisposition.Add("form-data");
                                contentDisposition.Add(string.Format(@"name=""{0}""", param.Name));
                                if (param.IsFile)
                                    contentDisposition.Add(string.Format(@"filename=""{0}""", Path.GetFileName(param.Content)));
                                stream.WriteText("Content-Disposition: " + string.Join("; ", contentDisposition));

                                if (!string.IsNullOrEmpty(param.ContentType))
                                    stream.WriteText("Content-Type: " + param.ContentType);

                                stream.WriteText("\r\n\r\n");

                                if (!param.IsFile)
                                {
                                    stream.WriteText(param.Content);
                                }
                                else
                                {
                                    stream.WriteAll(File.ReadAllBytes(param.Content));
                                }

                                stream.WriteText("\r\n");
                            }

                            stream.WriteText("--" + boundary + "--");
                        }

                        break;
                    default:
                        throw new ArgumentException("指定されたcontentTypeには対応していません。");
                }
            }

            return req;
        }
    }
}
