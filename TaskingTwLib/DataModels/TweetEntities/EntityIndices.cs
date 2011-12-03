using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Azyobuzi.TaskingTwLib.DataModels.TweetEntities
{
    /// <summary>
    /// TweetEntityの開始位置と終了位置
    /// </summary>
    public struct EntityIndices
    {
        /// <summary>
        /// 開始位置
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// 終了位置
        /// </summary>
        public int EndIndex { get; set; }

        /// <summary>
        /// indices要素から<see cref="T:EntityIndices"/>を作成します。
        /// </summary>
        /// <param name="json">indices要素</param>
        /// <returns>作成された<see cref="T:EntityIndices"/></returns>
        public static EntityIndices Create(XElement json)
        {
            var re = new EntityIndices();
            re.StartIndex = (int)json.Elements().First();
            re.EndIndex = (int)json.Elements().Last();

            return re;
        }
    }
}
