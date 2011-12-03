using System;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.DataModels.UserStreams
{
    /// <summary>
    /// UserStreamで流れてきたイベント
    /// </summary>
    public class EventData : RawData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="json">一行のJSON形式の文字列</param>
        public EventData(string json)
            : base(json)
        {
            this.Kind = DataKind.Event;

            var elm = SerializationHelper.JsonToXml(json);
            this.Event = (string)elm.Element("event");

            if (elm.Element("created_at") != null)
                this.CreatedAt = DateTimeUtil.Parse(elm.Element("created_at").Value);

            if (elm.Element("source") != null)
                this.Source = User.Create(elm.Element("source"));

            if (elm.Element("target") != null)
                this.Target = User.Create(elm.Element("target"));

            if (elm.Element("target_object") != null)
            {
                var targetObject = elm.Element("target_object");

                if (targetObject.Element("mode") != null)
                    this.TargetList = List.Create(targetObject);
                else if (targetObject.Element("text") != null
                    && targetObject.Element("sender") == null)//DMよけ
                    this.TargetStatus = Status.Create(targetObject);
            }
        }

        /// <summary>
        /// イベント名
        /// </summary>
        public string Event { get; private set; }

        /// <summary>
        /// 発生日時
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// イベントを発生させたユーザー
        /// </summary>
        public User Source { get; private set; }

        /// <summary>
        /// ターゲット
        /// </summary>
        public User Target { get; private set; }

        /// <summary>
        /// ターゲットとなるツイート
        /// </summary>
        public Status TargetStatus { get; private set; }

        /// <summary>
        /// ターゲットとなるリスト
        /// </summary>
        public List TargetList { get; private set; }
    }
}
