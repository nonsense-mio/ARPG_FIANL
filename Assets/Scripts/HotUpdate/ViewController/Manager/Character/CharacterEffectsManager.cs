using System.Collections;
using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

namespace ARPG
{
    public class CharacterEffectsManager : ARPGController
    {
        protected CharacterManager character;

        [Header("Current Range FX")]
        public GameObject instantiateFX;
        [Header("Poison")]
        public GameObject currentPoisonParticleFX;
        public bool isPoisoned;
        public float poisonBuildUp = 0;
        public float poisonAmount = 100;
        public float defaultPoisonAmount = 100;
        public float poisonTimer = 2f;//这个变量代表每隔多少秒扣一次毒伤
        public int poiseDamage = 1;
        protected int _poisonDamageTimerId = -1;

        public virtual void Init(CharacterManager characterMgr)
        {
            character = characterMgr;
        }

        /// <summary>
        /// 处理所有状态效果
        /// </summary>
        public virtual void HandleAllEffects()
        {
            if (character.isDead)
                return;
            HandlePoisonBuildUp();
            HandleIsPoisonedEffect();
        }

        protected virtual void HandlePoisonBuildUp()
        {
            if (isPoisoned)
                return;
            if (poisonBuildUp > 0 && poisonBuildUp < 100)
            {
                poisonBuildUp -= Time.deltaTime;
            }
            else if (poisonBuildUp >= 100)
            {
                isPoisoned = true;
                poisonBuildUp = 0;
                _poisonDamageTimerId = this.GetSystem<ITimerSystem>().CreateTimer(
                    false, defaultPoisonAmount, OnPoisonEnd,
                    poisonTimer, () =>
                    {
                        character.characterStatsManager.TakePoiseDamage(poiseDamage);
                        this.SendEvent(new PlayerPosionEvent { Character = character });
                    });
            }
        }

        protected virtual void HandleIsPoisonedEffect()
        {
            if (!isPoisoned)
                return;
            poisonAmount -= Time.deltaTime;
        }

        protected virtual void OnPoisonEnd()
        {
            isPoisoned = false;
            poisonAmount = defaultPoisonAmount;
            _poisonDamageTimerId = -1;
        }

        /// <summary>
        /// 主动解毒（供解毒道具/技能调用）
        /// </summary>
        public void CurePoison()
        {
            poisonBuildUp = 0;
            if (!isPoisoned) return;
            if (_poisonDamageTimerId != -1)
                this.GetSystem<ITimerSystem>().RemoveTimer(_poisonDamageTimerId);
            OnPoisonEnd();
        }

        //打断效果
        public virtual void InterrupEffect()
        {
            //将生成物放回对象池
            if (instantiateFX != null)
            {
                this.GetSystem<IPoolSystem>().Recycle(instantiateFX);
            }
            if (character.isHoldingArrow)
            {
                character.animator.SetBool("isHoldingArrow", false);
                Animator rangedWeaponAnimator = character.characterWeaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
                if (rangedWeaponAnimator != null)
                {
                    rangedWeaponAnimator.SetBool("isDrawn", false);
                    rangedWeaponAnimator.Play("Bow_TH_Fire_01");
                }
            }

            if (character.isAiming)
            {
                character.animator.SetBool("isAiming", false);
            }
        }

    }
}
