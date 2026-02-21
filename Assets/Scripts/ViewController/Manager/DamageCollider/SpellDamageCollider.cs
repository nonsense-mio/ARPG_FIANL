using ARPG;
using Framework;
using UnityEngine;

namespace ARPG
{
    public class SpellDamageCollider : DamageCollider, IPoolable
    {
        public GameObject impactParticles;
        public string fireballFXName;
        bool hasCollided = false;

        // 缓存刚体引用
        private Rigidbody rb;

        protected override void Awake()
        {
            base.Awake();  // 设置 isTrigger = true，与基类设计一致
            rb = GetComponent<Rigidbody>();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            // 友军：穿透，不爆炸
            CharacterManager hitCharacter = other.gameObject.GetComponentInParent<CharacterManager>();
            if (hitCharacter != null && hitCharacter.characterStatsManager.teamIDNumber == teamIDNumber)
                return;

            Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);

            if (hitCharacter != null)
            {
                // 每次检测前重置，防止对象池中的残留状态影响判断
                hasBeenParried = false;
                shieldHasBeenHit = false;

                EnemyManager ai = hitCharacter as EnemyManager;
                CheckForParry(hitCharacter);
                CheckForBlock(hitCharacter);

                // 格挡/招架时跳过伤害（CheckForBlock 内已处理格挡穿透伤害）
                if (!hasBeenParried && !shieldHasBeenHit)
                {
                    hitCharacter.characterStatsManager.poiseResetTimer = hitCharacter.characterStatsManager.totalPoiseResetTime;
                    hitCharacter.characterStatsManager.totalPoiseDefense -= poiseBreak;

                    float directionHitFrom = Vector3.SignedAngle(characterManager.transform.forward, hitCharacter.transform.forward, Vector3.up);
                    currentDamageAnimation = this.SendQuery(new GetHitDirectionQuery(directionHitFrom));
                    hitCharacter.characterEffectsManager.InterrupEffect();

                    if (hitCharacter.characterStatsManager.totalPoiseDefense > poiseBreak)
                        hitCharacter.characterStatsManager.TakeDamageNoAnimation(0, fireDamage, characterManager);
                    else
                        hitCharacter.characterStatsManager.TakeDamage(0, fireDamage, currentDamageAnimation, characterManager);

                    if (ai != null)
                        ai.currentTarget = characterManager;
                }
            }

            // 无论格挡/招架与否，火球接触实体后都爆炸并回收
            if (!hasCollided)
            {
                hasCollided = true;
                this.SendEvent(new FireBallHitEvent { HitPoint = hitPoint });
                this.GetSystem<IPoolSystem>().Recycle(gameObject);
            }
        }

        public void OnSpawn()
        {
            hasCollided = false;
            hasBeenParried = false;
            shieldHasBeenHit = false;
        }

        public void OnRecycle()
        {
            

            // 重置刚体物理状态
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }


    }
}
