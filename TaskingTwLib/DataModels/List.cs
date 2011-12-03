using System.Xml.Linq;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.DataModels
{
    /// <summary>
    /// リスト
    /// </summary>
    public class List
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// @ユーザーの表示名/リスト名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// フォロワー数
        /// </summary>
        public int SubscriberCount { get; set; }

        /// <summary>
        /// メンバー数
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// http://twitter.com/#!/ からリストページへの相対パス
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// フォローしているかどうか
        /// </summary>
        public bool Following { get; set; }

        /// <summary>
        /// プライバシーモード
        /// <para><c>"public"</c>か<c>"private"</c></para>
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 作成者
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// リストの情報を格納したJSON文字列から<see cref="T:Azyobuzi.TaskingTwLib.DataModels.List"/>を作成します。
        /// </summary>
        /// <param name="json">JSON形式の文字列</param>
        /// <returns>作成された<see cref="T:Azyobuzi.TaskingTwLib.DataModels.List"/></returns>
        public static List Create(string json)
        {
            return Create(SerializationHelper.JsonToXml(json));
        }

        /// <summary>
        /// list要素から<see cref="T:Azyobuzi.TaskingTwLib.DataModels.List"/>を作成します。
        /// </summary>
        /// <param name="json">list要素</param>
        /// <returns>作成された<see cref="T:Azyobuzi.TaskingTwLib.DataModels.List"/></returns>
        public static List Create(XElement json)
        {
            var re = new List();
            re.Id = (long)json.Element("id");
            re.Name = (string)json.Element("name");
            re.FullName = (string)json.Element("full_name");
            re.Slug = (string)json.Element("slug");
            re.Description = (string)json.Element("description");
            re.SubscriberCount = (int)json.Element("subscriber_count");
            re.MemberCount = (int)json.Element("member_count");
            re.Uri = (string)json.Element("uri");
            re.Following = (bool)json.Element("following");
            re.Mode = (string)json.Element("mode");
            re.User = User.Create(json.Element("user"));

            return re;
        }
    }
}
