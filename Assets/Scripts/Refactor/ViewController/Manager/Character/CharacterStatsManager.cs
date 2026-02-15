using ARPG;
using Framework;
using UnityEngine;

namespace ARPG
{   //人物基类
    public class CharacterStatsManager : ARPGController
    {
        CharacterManager character;
        public string characterName;
        [Header("Team ID")]
        public int teamIDNumber = 0;

        public int maxHealth;
        public int currentHealth;

        public float maxStamina;
        public float currentStamina;

        public float maxFocus;
        public float currentFocus;

        public int currentSoulCount = 0;
        public int soulsAwardedOnDeath = 50;
        [Header("人物等级")]
        public int playerLevel = 1;
        [Header("状态等级")]
        public int healthLevel = 10;
        public int staminaLevel = 10;
        public int focusLevel = 10;
        public int poiseLevel = 10;
        public int strengthLevel = 10;
        public int dexterityLevel = 10;
        public int intelligenceLevel = 10;
        public int faithLevel = 10;

        [Header("Poise")]
        public float totalPoiseDefense; //总姿态防御
        public float offensivePoiseBonus; //攻击姿态加成
        public float armorPoiseBonus; //护甲姿态加成
        public float totalPoiseResetTime = 15f; //总姿态重置时间
        public float poiseResetTimer;


        [Header("Armor Absorptions")]
        public float physicalDamageAbsorptionHead;
        public float physicalDamageAbsorptionBody;
        public float physicalDamageAbsorptionLegs;
        public float physicalDamageAbsorptionHands;

        public float fireDamageAbsorptionHead;
        public float fireDamageAbsorptionBody;
        public float fireDamageAbsorptionLegs;
        public float fireDamageAbsorptionHands;

        [Header("Blcock Absorptions")]
        public float blockingPhysicalDamageAbsorption;
        public float blockingFireDamageAbsorption;
        public float blockingStabilityRating;

        [Header("伤害修改")]
        public float physicalDamagePercentageModifier = 100;
        public float fireDamagePercentageModifier = 100;
        [Header("伤害吸收修改")]
        public float physicalAbsorptionPercentageModifier = 0;
        public float fireAbsorptionPercentageModifier = 0;
        
        public virtual void Init(CharacterManager characterMgr)
        {
            character = characterMgr;
        }
        private void Start()
        {
            totalPoiseDefense = armorPoiseBonus;
        }
        protected virtual void Update()
        {
            HandlePoiseResetTimer();
        }
        /// <summary>
        /// 计算并减去最终伤害 在受伤时调用
        /// (伤害公式委托给 CalculateFinalDamageQuery)
        /// </summary>
        private void CalculateDamage(int physicalDamage, int fireDamage, CharacterManager enemyCharacterDamagingMe = null)
        {
            var input = new DamageInput
            {
                RawPhysicalDamage = physicalDamage,
                RawFireDamage = fireDamage,
                AttackerPhysicalModifier = enemyCharacterDamagingMe != null
                    ? enemyCharacterDamagingMe.characterStatsManager.physicalDamagePercentageModifier : 100f,
                AttackerFireModifier = enemyCharacterDamagingMe != null
                    ? enemyCharacterDamagingMe.characterStatsManager.fireDamagePercentageModifier : 100f,
                IsFullyChargedAttack = enemyCharacterDamagingMe != null
                    && enemyCharacterDamagingMe.isPerformingFullyChargeAttack,
                PhysicalAbsorptionHead = physicalDamageAbsorptionHead,
                PhysicalAbsorptionBody = physicalDamageAbsorptionBody,
                PhysicalAbsorptionLegs = physicalDamageAbsorptionLegs,
                PhysicalAbsorptionHands = physicalDamageAbsorptionHands,
                FireAbsorptionHead = fireDamageAbsorptionHead,
                FireAbsorptionBody = fireDamageAbsorptionBody,
                FireAbsorptionLegs = fireDamageAbsorptionLegs,
                FireAbsorptionHands = fireDamageAbsorptionHands,
                PhysicalAbsorptionModifier = physicalAbsorptionPercentageModifier,
                FireAbsorptionModifier = fireAbsorptionPercentageModifier
            };

            int finalDamage = this.SendQuery(new CalculateFinalDamageQuery(input));
            Debug.Log("最终伤害：" + finalDamage);
            currentHealth -= finalDamage;
        }
        public virtual void TakeDamage(int physicalDamage, int fireDamage, string damageAnimation, CharacterManager enemyCharacterDamagingMe)
        {
            if (character.isDead)
                return;


            character.characterAnimatorManager.EraseHandIKForWeapon();
            CalculateDamage(physicalDamage, fireDamage, enemyCharacterDamagingMe);
        }
        //格挡时收到的伤害
        public virtual void TakeDamageAfterBlock(int physicalDamage, int fireDamage, CharacterManager enemyCharacterDamagingMe)
        {
            if (character.isDead)
                return;

            character.characterAnimatorManager.EraseHandIKForWeapon();
            CalculateDamage(physicalDamage, fireDamage, enemyCharacterDamagingMe);
        }


        /// <summary>
        /// 受伤时不播放其他动画 主要用于特定动画的使用
        /// </summary>
        /// <param name="damage"></param>
        public virtual void TakeDamageNoAnimation(int physicalDamage, int fireDamage, CharacterManager enemyCharacterDamagingMe)
        {
            if (character.isDead)
                return;
            CalculateDamage(physicalDamage, fireDamage, enemyCharacterDamagingMe);
        }

        public virtual void TakePoiseDamage(int damage)
        {
            currentHealth -= damage;

        }
        /// <summary>
        /// 处理姿态重置计时器
        /// </summary> <summary>
        public virtual void HandlePoiseResetTimer()
        {
            if (poiseResetTimer > 0)
            {
                poiseResetTimer -= Time.deltaTime;
            }
            else
            {
                totalPoiseDefense = armorPoiseBonus;
            }
        }
        public virtual void HealCharacter(int healAmount)
        {
            currentHealth += healAmount;
            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
            }

        }


        /// <summary>
        /// 该方法是根据健康等级决定最大生命值 
        /// 在本游戏中最大生命值会随着健康等级的变化而变化
        /// </summary>
        /// <returns>返回最大生命值</returns>
        public int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }
        /// <summary>
        /// 设置最大体力
        /// </summary>
        /// <returns></returns>
        public float SetMaxStaminaFromStaminaLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        public float SetMaxFocusPointsFromFocusLevel()
        {
            maxFocus = focusLevel * 10;
            return maxFocus;
        }



        protected virtual void HandleDeath()
        {
            //处理人物死亡
            currentHealth = 0;
            if(!character.isBeingBackstabbed && !character.isBeingRiposted)
                character.characterAnimatorManager.PlayTargetAnimation("Dead_01", true);
            print("人物死亡");
            character.isDead = true;
            this.SendEvent(new CharacterDeathEvent { Character = character });
        }

    }
}

