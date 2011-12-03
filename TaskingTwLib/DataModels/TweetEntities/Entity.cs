namespace Azyobuzi.TaskingTwLib.DataModels.TweetEntities
{
    /// <summary>
    /// TweetEntityの基底
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// 開始位置と終了位置
        /// </summary>
        public EntityIndices Indices { get; set; }
    }
}
