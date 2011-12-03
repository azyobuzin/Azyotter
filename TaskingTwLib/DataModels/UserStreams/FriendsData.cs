using System.Linq;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.DataModels.UserStreams
{
    /// <summary>
    /// UserStreamで流れてきたフォローしているアカウントのID
    /// </summary>
    public class FriendsData : RawData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="json">一行のJSON形式の文字列</param>
        public FriendsData(string json)
            : base(json)
        {
            this.Kind = DataKind.Friends;
            this.Friends = SerializationHelper.JsonToXml(json)
                .Element("friends")
                .Elements()
                .Select(item => (long)item)
                .ToArray();
        }

        /// <summary>
        /// フォローしているアカウントのID
        /// </summary>
        public long[] Friends { get; private set; }
    }
}
