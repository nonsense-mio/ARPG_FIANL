using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

namespace ARPG
{
    public class DamageCollider : ARPGController
    {
        public CharacterManager characterManager;
        protected Collider damageCollider;
        public bool enabledOnStart = false;

        [Header("Team ID")]
        public int teamIDNumber = 0;
        [Header("Poise")]
        public float poiseBreak;
        public float offensivePoiseBonus;
        [Header("伤害")]
        public int physicalDamage;
        public int fireDamage;
        public int magicDamage;
        public int lightningDamage;
        public int darkDamage;
        [Header("破防")]
        public float guardBreakModifier = 1;
        private List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        protected bool shieldHasBeenHit;
        protected bool hasBeenParried;
        protected string currentDamageAnimation;

        protected virtual void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = enabledOnStart;
        }
        /// <summary>
        /// 激活碰撞体组件
        /// </summary>
        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }
        /// <summary>
        /// 激活碰撞体组件
        /// </summary>
        public void DisableDamageCollider()
        {
            if (charactersDamaged.Count > 0)
            {
                //重置已受伤害列表
                charactersDamaged.Clear();
            }

            damageCollider.enabled = false;
        }

        protected virtual void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Damageable Character"))
            {
                shieldHasBeenHit = false;
                hasBeenParried = false;

                CharacterManager enemyManager = collision.GetComponentInParent<CharacterManager>();
                if (enemyManager != null)
                {
                    EnemyManager ai = enemyManager as EnemyManager;
                    //防止同一攻击多次伤害同一目标
                    if (charactersDamaged.Contains(enemyManager))
                    {
                        return;
                    }
                    charactersDamaged.Add(enemyManager);


                    //队伍ID相同 则不受伤害
                    if (enemyManager.characterStatsManager.teamIDNumber == teamIDNumber)
                        return;

                    CheckForParry(enemyManager);
                    CheckForBlock(enemyManager);

                    if (hasBeenParried)
                        return;
                    if (shieldHasBeenHit)
                        return;
                    enemyManager.characterStatsManager.poiseResetTimer = enemyManager.characterStatsManager.totalPoiseResetTime;
                    enemyManager.characterStatsManager.totalPoiseDefense -= poiseBreak;

                    //检测武器和敌人碰撞点位置
                    Vector3 contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    float directionHitFrom = Vector3.SignedAngle(characterManager.transform.forward, enemyManager.transform.forward, Vector3.up);

                    currentDamageAnimation = this.SendQuery(new GetHitDirectionQuery(directionHitFrom));

                    this.SendEvent(new CharacterDamageEvent { HitPoint = contactPoint });
                    enemyManager.characterEffectsManager.InterrupEffect();

                    //根据攻击类型造成伤害
                    DamageBasedOnAttackType(enemyManager);
                    //设置敌人锁定目标为攻击者
                    if (ai != null)
                        ai.currentTarget = characterManager;
                }


            }

            if (collision.tag == "Illusionary Wall")
            {
                IllusionaryWall illusionaryWall = collision.GetComponent<IllusionaryWall>();
                if (illusionaryWall != null)
                {
                    illusionaryWall.wallHasBeenHit = true;
                    if (illusionaryWall.audioSource != null)
                    {
                        illusionaryWall.audioSource.PlayOneShot(illusionaryWall.illusionaryWallSound);
                    }
                }
            }
        }

        protected virtual void CheckForParry(CharacterManager enemyManager)
        {
            if (enemyManager.isParrying)
            {
                //发动攻击的角色就被招架
                this.characterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Parried", true);
                hasBeenParried = true;
            }
        }

        //格挡
        protected virtual void CheckForBlock(CharacterManager enemyManager)
        {
            var enemyStats = enemyManager.characterStatsManager;
            var blockResult = this.SendQuery(new CalculateBlockResultQuery(new BlockInput
            {
                AttackerPosition = characterManager.transform.position,
                DefenderPosition = enemyManager.transform.position,
                DefenderForward = enemyManager.transform.forward,
                IsDefenderBlocking = enemyManager.isBlocking,
                PhysicalDamage = physicalDamage,
                FireDamage = fireDamage,
                GuardBreakModifier = guardBreakModifier,
                BlockingPhysicalAbsorption = enemyStats.blockingPhysicalDamageAbsorption,
                BlockingFireAbsorption = enemyStats.blockingFireDamageAbsorption,
                BlockingStabilityRating = enemyStats.blockingStabilityRating,
                DefenderCurrentStamina = enemyStats.currentStamina
            }));

            if (!blockResult.IsBlocked) return;

            shieldHasBeenHit = true;
            // 扣除格挡体力
            enemyStats.currentStamina -= blockResult.StaminaDamage;
            // 玩家角色需要同步体力到 Model (驱动 UI 更新)
            if (enemyManager is PlayerManager pm)
                pm.playerStatsManager.DeductStamina(0);
            // 破防或播放格挡动画
            if (blockResult.IsGuardBroken)
            {
                enemyManager.isBlocking = false;
                enemyManager.characterAnimatorManager.PlayTargetAnimation("Guard Break", true);
            }
            else
            {
                enemyManager.characterAnimatorManager.PlayTargetAnimation("Block Guard", true);
            }
            // 穿透伤害
            enemyStats.TakeDamageAfterBlock(blockResult.PhysicalDamageAfterBlock, blockResult.FireDamageAfterBlock, characterManager);
        }

        //根据攻击类型造成伤害
        protected virtual void DamageBasedOnAttackType(CharacterManager enemyManager)
        {
            var inv = characterManager.characterInventoryManager;
            int finalPhysicalDamage = this.SendQuery(new CalculateAttackTypeDamageQuery(
                physicalDamage,
                characterManager.characterCombatManager.currentAttackType,
                characterManager.isUsingRightHand,
                inv.rightWeapon.lightAttackDamageModifier,
                inv.rightWeapon.heavyAttackDamageModifier,
                inv.leftWeapon.lightAttackDamageModifier,
                inv.leftWeapon.heavyAttackDamageModifier));

            //根据敌人破防情况决定受击动画播放与否
            if (enemyManager.characterStatsManager.totalPoiseDefense > poiseBreak)
            {
                enemyManager.characterStatsManager.TakeDamageNoAnimation(finalPhysicalDamage, 0, characterManager);
            }
            else
            {
                enemyManager.characterStatsManager.TakeDamage(finalPhysicalDamage, 0, currentDamageAnimation, characterManager);
            }
        }



    }
}

