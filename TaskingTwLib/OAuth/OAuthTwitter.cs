using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.OAuth
{
    /// <summary>
    /// OAuth認証系メソッド
    /// </summary>
    public static class OAuthTwitter
    {
        private static string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();
            byte[] data = Encoding.UTF8.GetBytes(value);
            int len = data.Length;

            for (int i = 0; i < len; i++)
            {
                int c = data[i];
                if (c < 0x80 && "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~".IndexOf((char)c) != -1)
                {
                    result.Append((char)c);
                }
                else
                {
                    result.Append('%').AppendFormat("{0:X2}", (int)data[i]);
                }
            }

            return result.ToString();
        }

        private static Random random = new Random();

        /// <summary>
        /// OAuthのヘッダ（oauth_signature以外）を作成します。
        /// </summary>
        /// <param name="token">OAuthトークン</param>
        /// <param name="verifier">OAuth Verifier</param>
        /// <param name="callback">コールバック</param>
        /// <returns>OAuthのヘッダを<see cref="Dictionary{String, String}"/>で返します。</returns>
        public static Dictionary<string, string> GenerateOAuthParameter(Token token,
                                                                        string verifier,
                                                                        string callback)
        {
            var re = new Dictionary<string, string>()
            {
                { "oauth_version", "1.0" },
                { "oauth_nonce", random.Next(123400, 9999999).ToString() },
                {
                    "oauth_timestamp",
                    Math.Floor(
                        (DateTime.UtcNow - DateTimeUtil.UnixTime)
                        .TotalSeconds
                    )
                    .ToString("0")
                },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_consumer_key", token.ConsumerKey }
            };

            if (!string.IsNullOrEmpty(callback))
                re.Add("oauth_callback", callback);

            if (!string.IsNullOrEmpty(token.OAuthToken))
                re.Add("oauth_token", token.OAuthToken);

            if (!string.IsNullOrEmpty(verifier))
                re.Add("oauth_verifier", verifier);

            return re;
        }

        /// <summary>
        /// oauth_signatureの作成
        /// </summary>
        /// <param name="uri">リクエストURI</param>
        /// <param name="query">OAuth関連ヘッダとパラメータ</param>
        /// <param name="token">OAuthトークン</param>
        /// <param name="verifier">OAuth Verifier</param>
        /// <param name="callback">コールバック</param>
        /// <param name="methodType">HTTPメソッド</param>
        /// <returns>作成されたoauth_signature</returns>
        public static string GenerateSignature(Uri uri,
                                               IEnumerable<KeyValuePair<string, string>> query,
                                               Token token,
                                               string verifier,
                                               string callback,
                                               HttpMethodType methodType)
        {
            //文字列作成
            var param = string.Join("&",
                query.OrderBy(kvp => kvp.Key)//ここまずいかも
                    .Select(kvp => string.Format("{0}={1}", UrlEncode(kvp.Key), UrlEncode(kvp.Value))));

            var signatureBase = string.Format("{0}&{1}&{2}",
                methodType.ToString(),
                UrlEncode(string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath)),
                UrlEncode(param));

            //ハッシュ生成
            var key = string.Format("{0}&{1}", token.ConsumerSecret, token.OAuthTokenSecret);
            using (var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
            {
                return Convert.ToBase64String(hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(signatureBase)));
            }
        }

        /// <summary>
        /// Authorizationヘッダを作成します。
        /// </summary>
        /// <param name="uri">リクエストURI</param>
        /// <param name="realm">保護領域</param>
        /// <param name="query">パラメータ</param>
        /// <param name="token">OAuthトークン</param>
        /// <param name="verifier">OAuth Verifier</param>
        /// <param name="callback">コールバック</param>
        /// <param name="methodType">HTTPメソッド</param>
        /// <returns>作成されたヘッダ</returns>
        public static string CreateAuthorizationHeader(Uri uri,
                                                       string realm,
                                                       IEnumerable<KeyValuePair<string, string>> query,
                                                       Token token,
                                                       string verifier,
                                                       string callback,
                                                       HttpMethodType methodType)
        {
            var oauthHeaders = GenerateOAuthParameter(token, verifier, callback);
            oauthHeaders.Add("oauth_signature",
                GenerateSignature(uri,
                    query != null ? query.Concat(oauthHeaders) : oauthHeaders,
                    token,
                    verifier,
                    callback,
                    methodType));

            if (!string.IsNullOrEmpty(realm))
                oauthHeaders.Add("realm", realm);

            return "OAuth " + string.Join(",",
                oauthHeaders.Select(kvp => string.Format(@"{0}=""{1}""", kvp.Key, UrlEncode(kvp.Value))));
        }
    }
}
