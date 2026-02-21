using System.Collections.Generic;
using Framework;

namespace ARPG
{
    /// <summary>
    /// Boss 被击败命令 - 编排:
    /// 设置状态 → 关闭雾墙 → 隐藏 HUD → 更新 SceneStateModel
    /// </summary>
    public class BossDefeatedCommand : AbstractCommand
    {
        private BossFightManager bossFight;

        public BossDefeatedCommand() { }

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
        }
    }
}
