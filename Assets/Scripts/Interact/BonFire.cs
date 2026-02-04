using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using ARPG;

namespace HT
{
    public class BonFire : Interactable
    {
        [Header("篝火传送点")]
        public Transform bonfireTeleportPoint;
        [SerializeField] private string bonfireID;
        public bool hasBeenActived = false;

        public ParticleSystem activationFX;
        public ParticleSystem fireFX;

        protected override void Awake()
        {
            base.Awake();
            soundEffectName = "activeFire";
        }

        private void Start()
        {
            // 从 SceneStateModel 读取篝火激活状态
            var sceneStateModel = GameArchitecture.Interface.GetModel<ISceneStateModel>();
            hasBeenActived = sceneStateModel.IsBonfireActivated(bonfireID);

            // 根据激活状态设置表现
            UpdateBonfireVisual();
        }

        private void UpdateBonfireVisual()
        {
            if (hasBeenActived)
            {
                if (fireFX != null)
                {
                    fireFX.gameObject.SetActive(true);
                    fireFX.Play();
                }
                interactionPrompt = "休息";
                soundEffectName = "saveGame";
            }
            else
            {
                if (fireFX != null)
                {
                    fireFX.gameObject.SetActive(false);
                }
                interactionPrompt = "点燃";
                soundEffectName = "activeFire";
            }
        }

        public override void Interact(PlayerManager playerManager)
        {
            var data = CurrentGameDataMgr.Instance.playerData;

            if (!hasBeenActived)
            {
                // 首次点燃篝火
                ActivateBonfire(playerManager);
            }

            // 设置为当前复活点
            SetAsRespawnPoint(data);

            base.Interact(playerManager);

            // 回复玩家状态
            playerManager.playerStatsManager.FullPlayerStats();

            // 保存游戏
            GameManager.Instance.SaveCurrentGame();
        }

        private void ActivateBonfire(PlayerManager playerManager)
        {
            playerManager.playerAnimatorManager.PlayTargetAnimation(animationName, true);
            hasBeenActived = true;

            // 播放激活特效
            if (activationFX != null)
            {
                activationFX.gameObject.SetActive(true);
                activationFX.Play();
            }

            // 更新视觉表现
            UpdateBonfireVisual();

            // 更新 SceneStateModel 篝火状态
            var sceneStateModel = GameArchitecture.Interface.GetModel<ISceneStateModel>();
            sceneStateModel.SetBonfireActivated(bonfireID, true);
        }

        private void SetAsRespawnPoint(PlayerData data)
        {
            // 设置当前篝火为复活点
            data.lastRestedBonfireID = bonfireID;

            // 保存传送点坐标
            Vector3 respawnPos = bonfireTeleportPoint != null? bonfireTeleportPoint.position : transform.position;

            data.respawnX = respawnPos.x;
            data.respawnY = respawnPos.y;
            data.respawnZ = respawnPos.z;
        }

        /// <summary>
        /// 获取篝火传送点位置
        /// </summary>
        public Vector3 GetTeleportPosition()
        {
            return bonfireTeleportPoint != null  ? bonfireTeleportPoint.position : transform.position;
        }
    }
}

