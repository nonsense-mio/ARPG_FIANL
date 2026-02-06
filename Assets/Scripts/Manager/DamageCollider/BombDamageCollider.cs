using ARPG;
using Framework;
using UnityEngine;
namespace HT
{
    public class BombDamageCollider : DamageCollider, IPoolable
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
            CharacterManager hitCharacter = collision.gameObject.GetComponentInParent<CharacterManager>();
            if (hitCharacter != null && hitCharacter.characterStatsManager.teamIDNumber == teamIDNumber)
            {
                return;
            }
            if (!hasCollided)
            {
                hasCollided = true;
                Explode();
                //将炸弹放回对象池
                GameArchitecture.Interface.GetSystem<PoolSystem>().Recycle(gameObject);
                EnemyManager enemyManager = hitCharacter as EnemyManager;

                if (enemyManager != null)
                {
                    enemyManager.characterStatsManager.TakeDamage(0, explosionDamage, currentDamageAnimation, characterManager);
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
                    character.TakeDamage(0, explosionSplashDamage, currentDamageAnimation, characterManager);
                }
            }
            //从对象池中取出爆炸特效 发布事件
            GameArchitecture.Interface.SendEvent(new BombHitEvent { BombTransform = this.transform });
        }
        public void OnSpawn() { }

        //重置炸弹信息
        public void OnRecycle()
        {
            bombRigidbody.velocity = Vector3.zero;
            bombRigidbody.angularVelocity = Vector3.zero;
            hasCollided = false;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        void OnDisable()
        {
            //Explode();
        }
    }
}

