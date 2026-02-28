using ARPG;
using Framework;
using UnityEngine;

namespace ARPG
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        PlayerManager player;
        private IPlayerModel playerModel;
        public GameObject instantiatedFXModel;
        public int amountToBeHealed;
        public int focusTobeAdded;
        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            player = characterMgr as PlayerManager;
            playerModel = this.GetModel<IPlayerModel>();
        }
        /// <summary>
        /// 喝药瓶时调用的方法
        /// </summary>
        public void HealPlayerFromEffect()
        {
            player.playerStatsManager.HealCharacter(amountToBeHealed);
            player.playerStatsManager.AddFocusPoints(focusTobeAdded);
            // 触发治疗事件，VFXSystem和SoundSystem都会响应（一对多广播）
            this.SendEvent(new PlayerDrinkPotionEvent { Player = player });
        }


        protected override void HandlePoisonBuildUp()
        {
            base.HandlePoisonBuildUp();
            if (playerModel != null)
                playerModel.PoisonBuildUp.Value = Mathf.RoundToInt(poisonBuildUp);
        }

        protected override void HandleIsPoisonedEffect()
        {
            base.HandleIsPoisonedEffect();
            if (playerModel != null)
                playerModel.PoisonAmount.Value = Mathf.RoundToInt(poisonAmount);
        }

        protected override void OnPoisonEnd()
        {
            base.OnPoisonEnd();
            if (playerModel != null)
                playerModel.PoisonAmount.Value = (int)defaultPoisonAmount;
        }
    }

}

