using ARPG;
using Framework;
using UnityEngine;

namespace ARPG
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        PlayerManager player;
        //PoisonBuildUpBar poisonBuildUpBar;
        //PoisonAmountBar poisonAmountBar;
        public GameObject instantiatedFXModel;
        public int amountToBeHealed;
        public int focusTobeAdded;
        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            player = characterMgr as PlayerManager;
            //poisonBuildUpBar = FindObjectOfType<PoisonBuildUpBar>();
            //poisonAmountBar = FindObjectOfType<PoisonAmountBar>();
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
            if (poisonBuildUp <= 0)
            {
                //poisonBuildUpBar.gameObject.SetActive(false);
            }
            else
            {
                //poisonBuildUpBar.gameObject.SetActive(true);
            }
            base.HandlePoisonBuildUp();
            //poisonBuildUpBar.SetPoisonBuildUpAmount(Mathf.RoundToInt(poisonBuildUp));
        }

        
        protected override void HandleIsPoisonedEffect()
        {
            if (!isPoisoned)
            {
                //poisonAmountBar.gameObject.SetActive(false);
            }
            else
            {
                //poisonAmountBar.gameObject.SetActive(true);
            }
            base.HandleIsPoisonedEffect();
            //poisonAmountBar.SetPoisonAmount(Mathf.RoundToInt(poisonAmount));
        }
    }

}

