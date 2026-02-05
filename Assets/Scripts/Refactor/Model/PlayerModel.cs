using Framework;

namespace ARPG
{
    /// <summary>
    /// 玩家数据模型实现 - 管理玩家持久化数据
    /// </summary>
    public class PlayerModel : AbstractModel, IPlayerModel
    {
        #region 基础信息
        public BindableProperty<string> PlayerName { get; } = new BindableProperty<string>("");
        public BindableProperty<int> PlayerLevel { get; } = new BindableProperty<int>(1);
        public BindableProperty<int> CurrentSoulCount { get; } = new BindableProperty<int>(0);
        #endregion

        #region 属性等级
        public BindableProperty<int> HealthLevel { get; } = new BindableProperty<int>(10);
        public BindableProperty<int> StaminaLevel { get; } = new BindableProperty<int>(10);
        public BindableProperty<int> FocusLevel { get; } = new BindableProperty<int>(10);
        public BindableProperty<int> PoiseLevel { get; } = new BindableProperty<int>(10);
        public BindableProperty<int> StrengthLevel { get; } = new BindableProperty<int>(10);
        public BindableProperty<int> DexterityLevel { get; } = new BindableProperty<int>(10);
        public BindableProperty<int> IntelligenceLevel { get; } = new BindableProperty<int>(10);
        public BindableProperty<int> FaithLevel { get; } = new BindableProperty<int>(10);
        #endregion

        #region 位置数据
        public BindableProperty<float> PosX { get; } = new BindableProperty<float>(0f);
        public BindableProperty<float> PosY { get; } = new BindableProperty<float>(0f);
        public BindableProperty<float> PosZ { get; } = new BindableProperty<float>(0f);

        public BindableProperty<float> RespawnX { get; } = new BindableProperty<float>(0f);
        public BindableProperty<float> RespawnY { get; } = new BindableProperty<float>(0f);
        public BindableProperty<float> RespawnZ { get; } = new BindableProperty<float>(0f);
        public BindableProperty<string> LastRestedBonfireID { get; } = new BindableProperty<string>("");
        #endregion

        protected override void OnInit()
        {
            // Model 初始化时不需要特别操作
            // 数据通过 ImportFromPlayerData 加载
        }

        #region 数据导入导出
        public void ImportFromPlayerData(PlayerData data)
        {
            if (data == null) return;

            // 基础信息
            PlayerName.Value = data.playerName ?? "";
            PlayerLevel.Value = data.playerLevel;
            CurrentSoulCount.Value = data.currentSoulCount;

            // 属性等级
            HealthLevel.Value = data.healthLevel;
            StaminaLevel.Value = data.staminaLevel;
            FocusLevel.Value = data.focusLevel;
            PoiseLevel.Value = data.poiseLevel;
            StrengthLevel.Value = data.strengthLevel;
            DexterityLevel.Value = data.dexterityLevel;
            IntelligenceLevel.Value = data.intelligenceLevel;
            FaithLevel.Value = data.faithLevel;

            // 位置
            PosX.Value = data.xPos;
            PosY.Value = data.yPos;
            PosZ.Value = data.zPos;

            // 复活点
            RespawnX.Value = data.respawnX;
            RespawnY.Value = data.respawnY;
            RespawnZ.Value = data.respawnZ;
            LastRestedBonfireID.Value = data.lastRestedBonfireID ?? "";
        }

        public void ExportToPlayerData(PlayerData data)
        {
            if (data == null) return;

            // 基础信息
            data.playerName = PlayerName.Value;
            data.playerLevel = PlayerLevel.Value;
            data.currentSoulCount = CurrentSoulCount.Value;

            // 属性等级
            data.healthLevel = HealthLevel.Value;
            data.staminaLevel = StaminaLevel.Value;
            data.focusLevel = FocusLevel.Value;
            data.poiseLevel = PoiseLevel.Value;
            data.strengthLevel = StrengthLevel.Value;
            data.dexterityLevel = DexterityLevel.Value;
            data.intelligenceLevel = IntelligenceLevel.Value;
            data.faithLevel = FaithLevel.Value;

            // 位置
            data.xPos = PosX.Value;
            data.yPos = PosY.Value;
            data.zPos = PosZ.Value;

            // 复活点
            data.respawnX = RespawnX.Value;
            data.respawnY = RespawnY.Value;
            data.respawnZ = RespawnZ.Value;
            data.lastRestedBonfireID = LastRestedBonfireID.Value;
        }
        #endregion
    }
}
