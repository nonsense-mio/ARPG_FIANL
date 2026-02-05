using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using ARPG;

namespace HT
{
    /// <summary>
    /// 篝火管理器 - 管理所有篝火和复活点
    /// </summary>
    public class BonfireMgr : BaseManager<BonfireMgr>
    {
        private Dictionary<string, BonFire> bonfireRegistry = new Dictionary<string, BonFire>();

        private BonfireMgr() { }

        /// <summary>
        /// 注册篝火
        /// </summary>
        public void RegisterBonfire(string id, BonFire bonfire)
        {
            if (!bonfireRegistry.ContainsKey(id))
            {
                bonfireRegistry.Add(id, bonfire);
            }
        }

        /// <summary>
        /// 注销篝火
        /// </summary>
        public void UnregisterBonfire(string id)
        {
            if (bonfireRegistry.ContainsKey(id))
            {
                bonfireRegistry.Remove(id);
            }
        }

        /// <summary>
        /// 获取复活点位置
        /// </summary>
        public Vector3 GetRespawnPosition()
        {
            var data = CurrentGameDataMgr.Instance.playerData;

            // 优先从已注册的篝火获取位置
            if (!string.IsNullOrEmpty(data.lastRestedBonfireID)
                && bonfireRegistry.TryGetValue(data.lastRestedBonfireID, out BonFire bonfire))
            {
                return bonfire.GetTeleportPosition();
            }

            // 否则使用存档中的坐标
            return new Vector3(data.respawnX, data.respawnY, data.respawnZ);
        }

        /// <summary>
        /// 传送到指定篝火
        /// </summary>
        public void TeleportToBonfire(string bonfireID, PlayerManager player)
        {
            if (bonfireRegistry.TryGetValue(bonfireID, out BonFire bonfire))
            {
                player.transform.position = bonfire.GetTeleportPosition();
            }
        }

        /// <summary>
        /// 获取所有已激活的篝火ID列表（用于传送菜单）
        /// </summary>
        public List<string> GetActivatedBonfireIDs()
        {
            var activatedList = new List<string>();

            // 从 SceneStateModel 获取已激活的篝火
            var sceneStateModel = GameArchitecture.Interface.GetModel<ISceneStateModel>();
            foreach (var id in bonfireRegistry.Keys)
            {
                if (sceneStateModel.IsBonfireActivated(id))
                {
                    activatedList.Add(id);
                }
            }

            return activatedList;
        }
    }
}
