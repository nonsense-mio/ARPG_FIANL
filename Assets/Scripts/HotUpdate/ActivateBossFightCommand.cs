using Framework;

namespace ARPG
{
    /// <summary>
    /// 激活 Boss 战命令 (HotUpdate) — 可通过热更修改 Boss 战激活逻辑。
    /// 由 ActivateBossFightRequestEvent 桥接事件触发。
    /// </summary>
    public class ActivateBossFightCommand : AbstractCommand
    {
        private readonly BossFightManager bossFight;

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

            // 热更测试：Boss 战 BGM 切换
            this.GetSystem<IMusicSystem>().PlayBGM("Boss1");
        }
    }
}
