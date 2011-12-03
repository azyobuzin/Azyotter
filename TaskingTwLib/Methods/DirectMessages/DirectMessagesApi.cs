using System.Collections.Generic;
using Azyobuzi.TaskingTwLib.DataModels;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.Methods.DirectMessages
{
    /// <summary>
    /// direct_messages、direct_messages/sent
    /// </summary>
    public class DirectMessagesApi : ITwitterApi<DirectMessage[]>
    {
        private DirectMessagesApi() { }

        private string requestUri;
        string ITwitterApi<DirectMessage[]>.RequestUri
        {
            get { return this.requestUri; }
        }

        HttpMethodType ITwitterApi<DirectMessage[]>.MethodType
        {
            get { return HttpMethodType.GET; }
        }

        private List<FormData> parameters = new List<FormData>();
        IEnumerable<FormData> ITwitterApi<DirectMessage[]>.Parameters
        {
            get { return this.parameters; }
        }

        string ITwitterApi<DirectMessage[]>.ContentType
        {
            get { return null; }
        }

        string ITwitterApi<DirectMessage[]>.Verifier
        {
            get { return null; }
        }

        string ITwitterApi<DirectMessage[]>.Callback
        {
            get { return null; }
        }

        DirectMessage[] ITwitterApi<DirectMessage[]>.Parse(string response, Token token)
        {
            return ResponseParser.ParseDirectMessages(response);
        }

        /// <summary>
        /// リクエストを作成します。
        /// </summary>
        /// <param name="type">ダイレクトメッセージAPIの種類</param>
        /// <param name="sinceId">指定したID以降のダイレクトメッセージを返します。</param>
        /// <param name="maxId">指定したID以前のダイレクトメッセージを返します。</param>
        /// <param name="count">取得する件数</param>
        /// <param name="page">ページ</param>
        /// <returns>リクエスト</returns>
        public static DirectMessagesApi Create(DirectMessagesApiType type, ulong? sinceId = null, ulong? maxId = null, int count = 20, int page = 1)
        {
            var re = new DirectMessagesApi();

            switch (type)
            {
                case DirectMessagesApiType.Received:
                    re.requestUri = "http://api.twitter.com/1/direct_messages.json";
                    break;
                case DirectMessagesApiType.Sent:
                    re.requestUri = "http://api.twitter.com/1/direct_messages/sent.json";
                    break;
            }

            re.parameters.Add(new FormData("count", count.ToString()));
            re.parameters.Add(new FormData("page", page.ToString()));
            re.parameters.Add(new FormData("include_entities", "true"));

            if (sinceId.HasValue)
                re.parameters.Add(new FormData("since_id", sinceId.ToString()));

            if (maxId.HasValue)
                re.parameters.Add(new FormData("max_id", maxId.ToString()));

            return re;
        }
    }

    /// <summary>
    /// ダイレクトメッセージAPIの種類
    /// </summary>
    public enum DirectMessagesApiType
    {
        /// <summary>
        /// 受信したメッセージ
        /// </summary>
        Received,
        /// <summary>
        /// 送信したメッセージ
        /// </summary>
        Sent
    }
}
