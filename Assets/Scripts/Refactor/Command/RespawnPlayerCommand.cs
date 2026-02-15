using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 玩家复活命令 - 编排:
    /// 读取复活点 → 传送 → 重置死亡状态 → 满血回复 → 播放起身动画
    /// </summary>
    public class RespawnPlayerCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var playerModel = this.GetModel<IPlayerModel>();
            var player = PlayerManager.localPlayer;
            if (player == null) return;

            if (string.IsNullOrEmpty(playerModel.LastRestedBonfireID.Value))
            {
                Debug.LogWarning("[RespawnPlayerCommand] 没有激活的篝火");
                return;
            }

            // 读取复活点坐标
            Vector3 respawnPos = this.SendQuery(new GetRespawnPositionQuery());
            player.SetPlayerPosition(respawnPos);

            // 重置状态并回复
            player.isDead = false;
            player.playerStatsManager.FullPlayerStats();
            player.playerAnimatorManager.PlayTargetAnimation("Get up", true);
        }
    }
}
