using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;
namespace ARPG
{
    public class BombDamageCollider : DamageCollider, IPoolable
    {
        const string BOMB_DAMAGE_ANIM = "Damage_01";

        [Header("爆炸范围和伤害")]
        public int explosiveRadius = 1;
        public int explosionDamage;
        public int explosionSplashDamage;

        public Rigidbody bombRigidbody;
        public GameObject impactParticles;
        public string impactParticlesName;
        [SerializeField]
        private bool hasCollided = false;

        protected override void Awake()
        {
            damageCollider = GetComponent<Collider>();
            bombRigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            CharacterManager hitCharacter = collision.gameObject.GetComponentInParent<CharacterManager>();
            if (hitCharacter != null && hitCharacter.characterStatsManager.teamIDNumber == teamIDNumber)
            {
                return;
            }
            if (!hasCollided)
            {
                hasCollided = true;
                // Explode 会对范围内所有敌人造成溅射伤害，记录已被溅射命中的角色
                var splashHitCharacters = Explode();
                //将炸弹放回对象池
                this.GetSystem<IPoolSystem>().Recycle(gameObject);
                // 直击伤害：只对直接命中且未被溅射覆盖的目标额外造成伤害
                EnemyManager enemyManager = hitCharacter as EnemyManager;
                if (enemyManager != null && !splashHitCharacters.Contains(enemyManager.characterStatsManager))
                {
                    enemyManager.characterStatsManager.TakeDamage(0, explosionDamage, BOMB_DAMAGE_ANIM, characterManager);
                }
            }
        }

        /// <summary>
        /// 范围爆炸，对范围内所有非友军角色造成溅射伤害。
        /// 返回已被溅射命中的 CharacterStatsManager 集合。
        /// </summary>
        private HashSet<CharacterStatsManager> Explode()
        {
            var hitCharacters = new HashSet<CharacterStatsManager>();
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosiveRadius);
            foreach (Collider objectsInExplosion in colliders)
            {
                CharacterStatsManager character = objectsInExplosion.GetComponent<CharacterStatsManager>();
                if (character != null && character.teamIDNumber != teamIDNumber)
                {
                    character.TakeDamage(0, explosionSplashDamage, BOMB_DAMAGE_ANIM, characterManager);
                    hitCharacters.Add(character);
                }
            }
            //从对象池中取出爆炸特效 发布事件
            this.SendEvent(new BombHitEvent { BombTransform = this.transform });
            return hitCharacters;
        }
        public void OnSpawn()
        {
            hasCollided = false;
        }

        //重置炸弹信息
        public void OnRecycle()
        {
            bombRigidbody.velocity = Vector3.zero;
            bombRigidbody.angularVelocity = Vector3.zero;
        }

    }
}
