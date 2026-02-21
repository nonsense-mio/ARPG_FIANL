using System.Collections.Generic;
using Framework;

namespace ARPG
{
    /// <summary>
    /// 激活 Boss 战命令 - 编排:
    /// 前置检查 → 设置状态 → 开启雾墙 → 显示 Boss HUD
    /// </summary>
    public class ActivateBossFightCommand : AbstractCommand
    {
        private BossFightManager bossFight;

        public ActivateBossFightCommand() { }

        public ActivateBossFightCommand(BossFightManager bossFight)
        {
            this.bossFight = bossFight;
        }

        protected override void OnExecute()
        {
            if (bossFight.bossHasBeenDefeated) return;
            if (bossFight.bossFightIsActive) return;
            if (!bossFight.boss.isActiveAndEnabled) return;

            bossFight.bossFightIsActive = true;
            bossFight.bossHasBeenAwakened = true;

            // 开雾墙
            foreach (var fogWall in bossFight.fogWalls)
            {
                if (fogWall != null) fogWall.ActivateFogWall(true);
            }

            // 显示 Boss HUD
            bossFight.PublishBossHud();
        }
    }
}
