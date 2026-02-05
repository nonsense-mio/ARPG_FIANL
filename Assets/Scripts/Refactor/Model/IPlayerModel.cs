using Framework;

namespace ARPG
{
    /// <summary>
    /// 玩家数据模型接口 - QFramework Model层
    /// 职责: 管理玩家持久化数据，提供响应式数据绑定
    /// </summary>
    public interface IPlayerModel : IModel
    {
        #region 基础信息
        BindableProperty<string> PlayerName { get; }
        BindableProperty<int> PlayerLevel { get; }
        BindableProperty<int> CurrentSoulCount { get; }
        #endregion

        #region 属性等级 (影响最大值计算)
        BindableProperty<int> HealthLevel { get; }
        BindableProperty<int> StaminaLevel { get; }
        BindableProperty<int> FocusLevel { get; }
        BindableProperty<int> PoiseLevel { get; }
        BindableProperty<int> StrengthLevel { get; }
        BindableProperty<int> DexterityLevel { get; }
        BindableProperty<int> IntelligenceLevel { get; }
        BindableProperty<int> FaithLevel { get; }
        #endregion

        #region 位置数据
        BindableProperty<float> PosX { get; }
        BindableProperty<float> PosY { get; }
        BindableProperty<float> PosZ { get; }

        // 复活点
        BindableProperty<float> RespawnX { get; }
        BindableProperty<float> RespawnY { get; }
        BindableProperty<float> RespawnZ { get; }
        BindableProperty<string> LastRestedBonfireID { get; }
        #endregion

        #region 数据导入导出 (与存档系统集成)
        /// <summary>
        /// 从 PlayerData 导入数据 (存档加载时调用)
        /// </summary>
        void ImportFromPlayerData(PlayerData data);

        /// <summary>
        /// 导出到 PlayerData (存档保存时调用)
        /// </summary>
        void ExportToPlayerData(PlayerData data);
        #endregion
    }
}
