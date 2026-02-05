using ARPG;
using Framework;
using UnityEngine;

namespace HT
{
    public class SpellDamageCollider : DamageCollider, IPoolObject
    {
        public GameObject impactParticles;
        public string fireballFXName;
        bool hasCollided = false;

        // 缓存刚体引用
        private Rigidbody rb;

        protected override void Awake()
        {
            //base.Awake();
            rb = GetComponent<Rigidbody>();
        }


        protected void OnCollisionEnter(Collision other)
        {
            // 先检查同队伍，完全忽略
            CharacterManager hitCharacter = other.gameObject.GetComponentInParent<CharacterManager>();
            if (hitCharacter != null && hitCharacter.characterStatsManager.teamIDNumber == teamIDNumber)
            {
                return;  // 同队伍，完全忽略，火球继续飞
            }

            // 获取碰撞点 - 无论碰到什么都能获取
            Vector3 hitPoint = other.contacts.Length > 0  ? other.contacts[0].point : transform.position;

            // 处理角色伤害
            if (hitCharacter != null)
            {
                EnemyManager ai = hitCharacter as EnemyManager;

                CheckForParry(hitCharacter);
                CheckForBlock(hitCharacter);

                if (hasBeenParried)
                    return;
                if (shieldHasBeenHit)
                    return;

                hitCharacter.characterStatsManager.poiseResetTimer = hitCharacter.characterStatsManager.totalPoiseResetTime;
                hitCharacter.characterStatsManager.totalPoiseDefense -= poiseBreak;

                float directionHitFrom = Vector3.SignedAngle(characterManager.transform.forward, hitCharacter.transform.forward, Vector3.up);
                ChooseWhichDirectionDamageCameFrom(directionHitFrom);

                hitCharacter.characterEffectsManager.InterrupEffect();

                if (hitCharacter.characterStatsManager.totalPoiseDefense > poiseBreak)
                {
                    hitCharacter.characterStatsManager.TakeDamageNoAnimation(0, fireDamage, characterManager);
                }
                else
                {
                    hitCharacter.characterStatsManager.TakeDamage(0, fireDamage, currentDamageAnimation, characterManager);
                }

                if (ai != null)
                    ai.currentTarget = characterManager;
            }

            // 碰撞后生成特效并回收
            if (!hasCollided)
            {
                hasCollided = true;
                GameArchitecture.Interface.SendEvent(new FireBallHitEvent { HitPoint = hitPoint });
                PoolMgr.Instance.PushObj(this.gameObject);
            }
        }

        // 必须实现 IPoolObject 的 ResetInfo 或者利用 OnDisable 来清理
        public void ResetInfo()
        {
            hasCollided = false;

            // 1. 重置刚体物理状态
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }


    }
}
