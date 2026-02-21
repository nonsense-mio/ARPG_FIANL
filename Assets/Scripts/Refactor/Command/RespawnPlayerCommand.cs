using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 玩家复活命令：重新加载当前场景并从篝火存档恢复状态。
    /// 场景重载会自动清除所有残留状态（毒伤、箭矢、敌人等），等同于回到篝火休息时刻。
    /// </summary>
    public class RespawnPlayerCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var playerModel = this.GetModel<IPlayerModel>();
            if (string.IsNullOrEmpty(playerModel.LastRestedBonfireID.Value))
            {
                Debug.LogWarning("[RespawnPlayerCommand] 没有激活的篝火");
                return;
            }

            int slot = this.GetSystem<ISaveSystem>().CurrentSlotIndex;
            this.GetSystem<ITimerSystem>().ClearAllTimers();
            this.SendCommand(new ContinueGameCommand(slot));
            
        }
    }
}
