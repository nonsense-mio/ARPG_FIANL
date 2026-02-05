using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

namespace HT
{
    public class RangedProjectileDamageCollider : DamageCollider, IPoolObject
    {
        public RangedAmmoItem_SO ammoItem;
        protected bool hasAlreadyPenetratedASurface;
        protected GameObject penetratedProjectile;
        Rigidbody arrowRigidbody;
        CapsuleCollider arrowCapsuleCollider;


        private List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        protected override void Awake()
        {
            arrowRigidbody = GetComponent<Rigidbody>();
            arrowCapsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void FixedUpdate()
        {
            // 让箭矢始终面向飞行方向
            if (arrowRigidbody.velocity != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(arrowRigidbody.velocity);
            }
        }
        public void ResetInfo()
        {
            hasAlreadyPenetratedASurface = false;
            penetratedProjectile = null;
            arrowCapsuleCollider.enabled = true;
            // 1. 暂停物理，清空速度
            //arrowRigidbody.isKinematic = true;
            arrowRigidbody.velocity = Vector3.zero;
            arrowRigidbody.angularVelocity = Vector3.zero;
            arrowRigidbody.isKinematic = true;

            if (charactersDamaged.Count > 0)
            {
                //重置已受伤害列表
                charactersDamaged.Clear();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            shieldHasBeenHit = false;
            hasBeenParried = false;
            Debug.Log("箭矢撞到了: " + collision.gameObject.name + " | Tag: " + collision.gameObject.tag);
            CharacterManager enemyManager = collision.gameObject.GetComponentInParent<CharacterManager>();
            if (enemyManager != null)
            {
                EnemyManager ai = enemyManager as EnemyManager;
                //队伍ID相同 则不受伤害
                if (enemyManager.characterStatsManager.teamIDNumber == teamIDNumber)
                {
                    return;
                }

                //防止同一攻击多次伤害同一目标
                if (charactersDamaged.Contains(enemyManager))
                {
                    return;
                }
                charactersDamaged.Add(enemyManager);



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
                GameArchitecture.Interface.SendEvent(new CharacterDamageEvent { HitPoint = contactPoint });
                enemyManager.characterEffectsManager.InterrupEffect();

                if (enemyManager.characterStatsManager.totalPoiseDefense > poiseBreak)
                {
                    enemyManager.characterStatsManager.TakeDamageNoAnimation(physicalDamage, 0, characterManager);
                }
                //破防后能够播放受击动画
                else
                {
                    enemyManager.characterStatsManager.TakeDamage(physicalDamage, 0, currentDamageAnimation, characterManager);
                }
                //设置敌人锁定目标为攻击者
                if (ai != null)
                    ai.currentTarget = characterManager;
            }


            if (collision.gameObject.tag == "Illusionary Wall")
            {
                IllusionaryWall illusionaryWall = collision.gameObject.GetComponent<IllusionaryWall>();
                if (illusionaryWall != null)
                {
                    illusionaryWall.wallHasBeenHit = true;
                    if (illusionaryWall.audioSource != null)
                    {
                        //TODO 音效待替换
                        illusionaryWall.audioSource.PlayOneShot(illusionaryWall.illusionaryWallSound);
                    }
                }
            }

            if (!hasAlreadyPenetratedASurface && penetratedProjectile == null)
            {
                hasAlreadyPenetratedASurface = true;
                arrowRigidbody.isKinematic = true;
                arrowCapsuleCollider.enabled = false;

                gameObject.transform.position = collision.GetContact(0).point + (transform.forward * 0.1f);
                gameObject.transform.rotation = Quaternion.LookRotation(transform.forward);
                gameObject.transform.parent = collision.collider.transform;


            }

        }
    }
}

