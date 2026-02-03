using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HT
{
    public class BombDamageCollider : DamageCollider, IPoolObject
    {
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
            Debug.Log("炸弹撞到了: " + collision.gameObject.name + " | Tag: " + collision.gameObject.tag);
            if (!hasCollided)
            {
                hasCollided = true;
                //将炸弹放回对象池
                PoolMgr.Instance.PushObj(transform.parent.parent.gameObject);
                CharacterStatsManager character = collision.gameObject.GetComponent<CharacterStatsManager>();
                if (character != null && character.teamIDNumber != teamIDNumber)
                {
                    character.TakeDamage(0, explosionDamage, currentDamageAnimation,characterManager);
                    Debug.Log(explosionDamage);
                }

            }
        }
        private void Explode()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosiveRadius);
            foreach (Collider objectsInExplosion in colliders)
            {
                CharacterStatsManager character = objectsInExplosion.GetComponent<CharacterStatsManager>();
                if (character != null && character.teamIDNumber != teamIDNumber)
                {
                    character.TakeDamage(0, explosionSplashDamage, currentDamageAnimation,characterManager);
                }
            }
        }
        //重置炸弹信息
        public void ResetInfo()
        {

            bombRigidbody.velocity = Vector3.zero;
            bombRigidbody.angularVelocity = Vector3.zero;
            hasCollided = false;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

        }
        void OnDisable()
        {
            Explode();
            //从对象池中取出爆炸特效 发布事件
            EventCenter.Instance.EventTrigger<Transform>(E_EventType.E_BombHit, this.transform);
        }
    }
}

