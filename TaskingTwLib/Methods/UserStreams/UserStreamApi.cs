using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azyobuzi.TaskingTwLib.DataModels.UserStreams;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.Methods.UserStreams
{
    /// <summary>
    /// User Streams API
    /// </summary>
    public class UserStreamApi : IStreamingApi<RawData>
    {
        string IStreamingApi<RawData>.RequestUri
        {
            get { return "https://userstream.twitter.com/1.1/user.json"; }
        }

        private List<FormData> parameters = new List<FormData>();
        IEnumerable<FormData> IStreamingApi<RawData>.Parameters
        {
            get { return this.parameters; }
        }

        RawData IStreamingApi<RawData>.Parse(string line)
        {
            var json = SerializationHelper.JsonToXml(line);

            if (json.Element("friends") != null)
            {
                return new FriendsData(line);
            }

            if (json.Element("text") != null)
            {
                return new StatusData(line);
            }

            if (json.Element("direct_message") != null)
            {
                return new DirectMessageData(line);
            }

            if (json.Element("delete") != null)
            {
                return new DeleteData(line);
            }

            if (json.Element("event") != null)
            {
                return new EventData(line);
            }

            return new RawData(line);
        }

        /// <summary>
        /// User Streams APIのリクエストを作成します
        /// </summary>
        /// <param name="allReplies">All @replies</param><!--TODO:正しい説明-->
        /// <returns>リクエスト</returns>
        public static UserStreamApi Create(bool allReplies = false)
        {
            var re = new UserStreamApi();

            if (allReplies)
                re.parameters.Add(new FormData("replies", "all"));

            return re;
        }
    }
}
