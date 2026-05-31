using Framework;

namespace ARPG
{
    /// <summary>
    /// Boss 击败命令 (HotUpdate) — 可通过热更修改击败后行为。
    /// 由 BossDefeatedRequestEvent 桥接事件触发。
    /// </summary>
    public class BossDefeatedCommand : AbstractCommand
    {
        private readonly BossFightManager bossFight;

        public BossDefeatedCommand(BossFightManager bossFight)
        {
            this.bossFight = bossFight;
        }

        protected override void OnExecute()
        {
            if (!bossFight.bossFightIsActive) return;

            bossFight.bossFightIsActive = false;
            bossFight.bossHasBeenDefeated = true;

            // 关雾墙
            foreach (var fogWall in bossFight.fogWalls)
            {
                if (fogWall != null) fogWall.ActivateFogWall(false);
            }

            // 隐藏 HUD
            this.SendEvent(new BossHudChangedEvent { Data = null });

            // 更新 SceneStateModel Boss 击败状态
            var sceneStateModel = this.GetModel<ISceneStateModel>();
            sceneStateModel.SetBossDefeated(bossFight.BossID, true);

            // 热更测试：Boss 击败后恢复场景 BGM
            this.GetSystem<IMusicSystem>().PlayBGM("End");
        }
    }
}
