using Framework;

namespace ARPG
{
    /// <summary>
    /// 确认升级命令 - 双路写入:
    /// 1. IPlayerModel (驱动 GamePanel 响应式更新)
    /// 2. PlayerStatsManager (运行时逻辑)
    /// </summary>
    public class ConfirmLevelUpCommand : AbstractCommand
    {
        private int projectedPlayerLevel, soulsToDeduct;
        private int newHealthLevel, newStaminaLevel, newFocusLevel, newPoiseLevel;
        private int newStrengthLevel, newDexterityLevel, newIntelligenceLevel, newFaithLevel;

        public ConfirmLevelUpCommand() { }

        public ConfirmLevelUpCommand(int projectedPlayerLevel, int soulsToDeduct,
            int health, int stamina, int focus, int poise,
            int strength, int dexterity, int intelligence, int faith)
        {
            this.projectedPlayerLevel = projectedPlayerLevel;
            this.soulsToDeduct = soulsToDeduct;
            this.newHealthLevel = health;
            this.newStaminaLevel = stamina;
            this.newFocusLevel = focus;
            this.newPoiseLevel = poise;
            this.newStrengthLevel = strength;
            this.newDexterityLevel = dexterity;
            this.newIntelligenceLevel = intelligence;
            this.newFaithLevel = faith;
        }

        protected override void OnExecute()
        {
            var playerModel = this.GetModel<IPlayerModel>();
            int newSoulCount = playerModel.CurrentSoulCount - soulsToDeduct;

            // 1) 写入 PlayerModel (驱动 GamePanel 响应式更新)
            playerModel.PlayerLevel.Value = projectedPlayerLevel;
            playerModel.HealthLevel.Value = newHealthLevel;
            playerModel.StaminaLevel.Value = newStaminaLevel;
            playerModel.FocusLevel.Value = newFocusLevel;
            playerModel.PoiseLevel.Value = newPoiseLevel;
            playerModel.StrengthLevel.Value = newStrengthLevel;
            playerModel.DexterityLevel.Value = newDexterityLevel;
            playerModel.IntelligenceLevel.Value = newIntelligenceLevel;
            playerModel.FaithLevel.Value = newFaithLevel;
            playerModel.CurrentSoulCount.Value = newSoulCount;

            // 2) 写入 PlayerStatsManager (运行时逻辑)
            var pm = PlayerManager.localPlayer;
            if (pm == null) return;

            var stats = pm.playerStatsManager;
            stats.playerLevel = projectedPlayerLevel;
            stats.healthLevel = newHealthLevel;
            stats.staminaLevel = newStaminaLevel;
            stats.focusLevel = newFocusLevel;
            stats.poiseLevel = newPoiseLevel;
            stats.strengthLevel = newStrengthLevel;
            stats.dexterityLevel = newDexterityLevel;
            stats.intelligenceLevel = newIntelligenceLevel;
            stats.faithLevel = newFaithLevel;
            stats.currentSoulCount = newSoulCount;

            // 重新计算 max 值
            stats.SetMaxHealthFromHealthLevel();
            stats.SetMaxStaminaFromStaminaLevel();
            stats.SetMaxFocusPointsFromFocusLevel();

            // 同步 max 值到 Model (GamePanel 自动更新)
            playerModel.MaxHP.Value = stats.maxHealth;
            playerModel.MaxStamina.Value = (int)stats.maxStamina;
            playerModel.MaxFocus.Value = (int)stats.maxFocus;
        }
    }
}
