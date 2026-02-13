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
        private string posionFX = "FX/Poison";
        public GameObject currentPoisonParticleFX;
        public Transform buildUpTransform;
        public bool isPoisoned;
        public float poisonBuildUp = 0;
        public float poisonAmount = 100;
        public float defaultPoisonAmount = 100;
        public float poisonTimer = 2f;//这个变量代表每隔多少秒扣一次毒伤
        public int poiseDamage = 1;
        float timer;

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
                if (buildUpTransform != null)
                {
                    //生成毒特效
                    currentPoisonParticleFX = this.GetSystem<IPoolSystem>().Spawn(posionFX);
                    currentPoisonParticleFX.transform.position = buildUpTransform.position;
                }
                else
                {
                    currentPoisonParticleFX = this.GetSystem<IPoolSystem>().Spawn(posionFX);
                    currentPoisonParticleFX.transform.position = character.transform.position;
                }
                currentPoisonParticleFX.transform.localScale = Vector3.one * 0.5f;
            }
        }
        protected virtual void HandleIsPoisonedEffect()
        {
            if (!isPoisoned)
                return;
            //中毒状态 每秒扣血
            if (poisonAmount > 0)
            {
                timer += Time.deltaTime;
                if (timer >= poisonTimer)
                {
                    character.characterStatsManager.TakePoiseDamage(poiseDamage);
                    timer = 0;
                }
                poisonAmount -= Time.deltaTime;
            }
            //中毒结束
            else
            {
                isPoisoned = false;
                poisonAmount = defaultPoisonAmount;
                //移除毒特效
                if (currentPoisonParticleFX != null)
                {
                    this.GetSystem<IPoolSystem>().Recycle(currentPoisonParticleFX);
                    currentPoisonParticleFX = null;
                }
            }
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

