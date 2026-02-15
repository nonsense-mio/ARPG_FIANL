using UnityEngine;
using Framework;


namespace ARPG
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
            var sceneStateModel = this.GetModel<ISceneStateModel>();
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
            if (!hasBeenActived)
            {
                // 首次点燃篝火 (视觉效果 + SceneStateModel更新)
                ActivateBonfire(playerManager);
            }

            base.Interact(playerManager);

            // 设置复活点 + 回复状态 + 保存
            this.SendCommand(new RestAtBonfireCommand(bonfireID, GetTeleportPosition()));
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
            var sceneStateModel = this.GetModel<ISceneStateModel>();
            sceneStateModel.SetBonfireActivated(bonfireID, true);
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

