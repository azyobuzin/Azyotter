using System;
using System.Xml.Linq;
using Azyobuzi.TaskingTwLib.DataModels.TweetEntities;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.DataModels
{
    /// <summary>
    /// ダイレクトメッセージ
    /// </summary>
    public class DirectMessage
    {
        /// <summary>
        /// ID
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// 送信者のID
        /// </summary>
        public long SenderId { get; set; }

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 受信者のID
        /// </summary>
        public long RecipientId { get; set; }

        /// <summary>
        /// 投稿日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 送信者の表示名
        /// </summary>
        public string SenderScreenName { get; set; }

        /// <summary>
        /// 受信者の表示名
        /// </summary>
        public string RecipientScreenName { get; set; }

        /// <summary>
        /// 送信者
        /// </summary>
        public User Sender { get; set; }

        /// <summary>
        /// 受信者
        /// </summary>
        public User Recipient { get; set; }

        /// <summary>
        /// TweetEntities
        /// </summary>
        public Entities Entities { get; set; }

        /// <summary>
        /// ダイレクトメッセージの内容を格納したJSON文字列から<see cref="T:DirectMessage"/>を作成します。
        /// </summary>
        /// <param name="json">JSON形式の文字列</param>
        /// <returns>作成された<see cref="T:DirectMessage"/></returns>
        public static DirectMessage Create(string json)
        {
            return Create(SerializationHelper.JsonToXml(json));
        }

        /// <summary>
        /// direct_message要素から<see cref="T:DirectMessage"/>を作成します。
        /// </summary>
        /// <param name="json">direct_message要素</param>
        /// <returns>作成された<see cref="T:DirectMessage"/></returns>
        public static DirectMessage Create(XElement json)
        {
            var re = new DirectMessage();
            re.Id = (ulong)json.Element("id");
            re.SenderId = (long)json.Element("sender_id");
            re.Text = json.Element("text").Value;
            re.RecipientId = (long)json.Element("recipient_id");
            re.CreatedAt = DateTimeUtil.Parse(json.Element("created_at").Value);
            re.SenderScreenName = json.Element("sender_screen_name").Value;
            re.RecipientScreenName = json.Element("recipient_screen_name").Value;
            re.Sender = User.Create(json.Element("sender"));
            re.Recipient = User.Create(json.Element("recipient"));
            re.Entities = json.Element("entities") != null
                ? Entities.Create(json.Element("entities"))
                : null;

            return re;
        }
    }
}
