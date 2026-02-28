using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 篝火休息命令 - 编排:
    /// 设置复活点 → 回复玩家状态 → 保存游戏
    /// </summary>
    public class RestAtBonfireCommand : AbstractCommand
    {
        private string bonfireID;
        private Vector3 respawnPosition;

        public RestAtBonfireCommand() { }

        public RestAtBonfireCommand(string bonfireID, Vector3 respawnPosition)
        {
            this.bonfireID = bonfireID;
            this.respawnPosition = respawnPosition;
        }

        protected override void OnExecute()
        {
            var playerModel = this.GetModel<IPlayerModel>();

            // 1) 设置复活点
            playerModel.LastRestedBonfireID.Value = bonfireID;
            playerModel.RespawnX.Value = respawnPosition.x;
            playerModel.RespawnY.Value = respawnPosition.y;
            playerModel.RespawnZ.Value = respawnPosition.z;

            // 2) 回复玩家状态
            var player = PlayerManager.localPlayer;
            if (player != null)
                player.playerStatsManager.FullPlayerStats();

            // 3) 保存游戏
            this.SendCommand(new SaveGameCommand());
        }
    }
}
