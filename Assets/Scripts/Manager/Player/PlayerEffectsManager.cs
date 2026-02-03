using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
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
            // 触发治疗事件，VFXMgr和SoundManager都会响应（一对多广播）
            EventCenter.Instance.EventTrigger<PlayerManager>(E_EventType.E_Player_DrinkPotion,player);
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

