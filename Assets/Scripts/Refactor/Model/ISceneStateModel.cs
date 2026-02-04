using QFramework;

namespace ARPG
{
    /// <summary>
    /// 场景状态模型接口 - QFramework Model层
    /// 职责: 管理场景元素的持久化状态（宝箱、篝火、Boss等）
    /// </summary>
    public interface ISceneStateModel : IModel
    {
        #region 宝箱状态
        /// <summary>
        /// 设置宝箱开启状态
        /// </summary>
        void SetChestOpened(string chestId, bool opened);

        /// <summary>
        /// 获取宝箱是否已开启
        /// </summary>
        bool IsChestOpened(string chestId);
        #endregion

        #region 篝火状态
        /// <summary>
        /// 设置篝火激活状态
        /// </summary>
        void SetBonfireActivated(string bonfireId, bool activated);

        /// <summary>
        /// 获取篝火是否已激活
        /// </summary>
        bool IsBonfireActivated(string bonfireId);
        #endregion

        #region Boss状态
        /// <summary>
        /// 设置 Boss 击败状态
        /// </summary>
        void SetBossDefeated(string bossId, bool defeated);

        /// <summary>
        /// 获取 Boss 是否已被击败
        /// </summary>
        bool IsBossDefeated(string bossId);
        #endregion

        #region 数据导入导出 (与存档系统集成)
        /// <summary>
        /// 从 SceneStateData 导入数据 (存档加载时调用)
        /// </summary>
        void ImportFromSceneStateData(SceneStateData data);

        /// <summary>
        /// 导出到 SceneStateData (存档保存时调用)
        /// </summary>
        void ExportToSceneStateData(SceneStateData data);
        #endregion
    }
}
