using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

namespace ARPG
{
    public class DamageCollider : MonoBehaviour
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

                    ChooseWhichDirectionDamageCameFrom(directionHitFrom);
                    //播放受击特效
                    //enemyManager.characterEffectsManager.PlayBloodSplatterFX(contactPoint);
                    GameArchitecture.Interface.SendEvent(new CharacterDamageEvent { HitPoint = contactPoint });
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
            CharacterStatsManager enemyShield = enemyManager.characterStatsManager;
            Vector3 directionFromEnemyToAttacker = (characterManager.transform.position - enemyManager.transform.position).normalized;
            //点乘 
            float facing = Vector3.Dot(directionFromEnemyToAttacker, enemyManager.transform.forward);
            // 0.3 ≈ cos(72.54°)，意味着攻击者在敌人正前方约 ±72° 的扇形内
            if (enemyManager.isBlocking && facing > 0.3f)
            {
                shieldHasBeenHit = true;
                float physicalDamageAfterBlock = physicalDamage - (physicalDamage * enemyShield.blockingPhysicalDamageAbsorption) / 100;
                float fireDamageAfterBlock = fireDamage - (fireDamage * enemyShield.blockingFireDamageAbsorption) / 100;
                enemyManager.characterCombatManager.AttemptBlock(this, physicalDamage, fireDamage, "Block Guard");
                enemyShield.TakeDamageAfterBlock(Mathf.RoundToInt(physicalDamageAfterBlock), Mathf.RoundToInt(fireDamageAfterBlock), characterManager);
            }


        }

        //根据攻击类型造成伤害
        protected virtual void DamageBasedOnAttackType(CharacterManager enemyManager)
        {
            float finalPhysicalDamage = physicalDamage;
            if (characterManager.isUsingRightHand)
            {
                if (characterManager.characterCombatManager.currentAttackType == E_AttackType.LightAttack)
                {
                    //轻攻击伤害计算
                    finalPhysicalDamage *= characterManager.characterInventoryManager.rightWeapon.lightAttackDamageModifier;
                }
                else if (characterManager.characterCombatManager.currentAttackType == E_AttackType.HeavyAttack)
                {
                    //重攻击伤害计算
                    finalPhysicalDamage *= characterManager.characterInventoryManager.rightWeapon.heavyAttackDamageModifier;
                }
            }
            else if (characterManager.isUsingLeftHand)
            {
                if (characterManager.characterCombatManager.currentAttackType == E_AttackType.LightAttack)
                {
                    //轻攻击伤害计算
                    finalPhysicalDamage *= characterManager.characterInventoryManager.leftWeapon.lightAttackDamageModifier;
                }
                else if (characterManager.characterCombatManager.currentAttackType == E_AttackType.HeavyAttack)
                {
                    //重攻击伤害计算
                    finalPhysicalDamage *= characterManager.characterInventoryManager.leftWeapon.heavyAttackDamageModifier;
                }
            }
            //根据敌人破防情况决定受击动画播放与否
            if (enemyManager.characterStatsManager.totalPoiseDefense > poiseBreak)
            {
                enemyManager.characterStatsManager.TakeDamageNoAnimation(Mathf.RoundToInt(finalPhysicalDamage), 0, characterManager);
            }
            //破防后能够播放受击动画
            else
            {
                enemyManager.characterStatsManager.TakeDamage(Mathf.RoundToInt(finalPhysicalDamage), 0, currentDamageAnimation, characterManager);
            }
        }

        /// <summary>
        /// 选择受击方向
        /// </summary>
        /// <param name="direction"></param> <summary>
        protected virtual void ChooseWhichDirectionDamageCameFrom(float direction)
        {
            if (direction >= 135 && direction <= 180 || direction <= -135 && direction >= -180)
            {
                //从后面受到伤害
                currentDamageAnimation = "Hit Forward";
            }
            else if (direction >= -45 && direction <= 45)
            {
                //从前面受到伤害
                currentDamageAnimation = "Hit Backward";
            }
            else if (direction > 45 && direction < 135)
            {
                //从右侧受到伤害
                currentDamageAnimation = "Hit Right";
            }
            else if (direction < -45 && direction > -135)
            {
                //从左侧受到伤害
                currentDamageAnimation = "Hit Left";
            }
        }

    }
}

