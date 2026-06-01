using System.Collections;
using ARPG;
using Framework;
using UnityEngine;

namespace ARPG
{
    public class EnemyStatsManager : CharacterStatsManager
    {
        EnemyManager enemy;
        public UIEnemyHealthBar enemyHealthBar;
        public bool isBoss;

        [Header("消融设置")]
        [SerializeField] private float dissolveDelay = 1.5f;    // 死亡动画播放完毕前的等待时间
        [SerializeField] private float dissolveDuration = 3f;   // 消融持续时间（秒）

        // Shader.PropertyToID 缓存，避免每帧做字符串哈希
        private static readonly int DissolvePropertyID = Shader.PropertyToID("_Dissolve");
        // MaterialPropertyBlock：按实例设置属性，不创建新材质，不影响同材质的其他对象
        // 不能在字段初始化器里 new（Unity 禁止在构造函数中调用原生 API），在 Init() 里延迟创建
        private MaterialPropertyBlock _mpb;

        // 二阶段事件只触发一次（避免重复触发导致动画被反复打断）
        public bool hasPublishedPhaseShift;

        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            enemy = characterMgr as EnemyManager;
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            hasPublishedPhaseShift = false;
            
            
            _mpb ??= new MaterialPropertyBlock();

            // 对象池复用时重置消融值，确保敌人完整出现
            foreach (var r in GetComponentsInChildren<Renderer>())
            {
                r.GetPropertyBlock(_mpb);
                _mpb.SetFloat(DissolvePropertyID, 0f);
                r.SetPropertyBlock(_mpb);
            }
        }

        /// <summary>
        /// 当 Boss 血量首次低于 1/2 时发布二阶段切换事件。
        /// 返回 true 表示本次成功触发（调用方通常应避免再播放受伤/攻击等覆盖动画）。
        /// </summary>
        private bool TryPublishBossPhaseShift()
        {
            if (!isBoss)
                return false;
            if (hasPublishedPhaseShift)
                return false;
            if (enemy == null || enemy.isDead)
                return false;
            // 死亡时不触发二阶段
            if (currentHealth <= 0)
                return false;
            // 低于或等于 1/2 触发
            if (currentHealth > maxHealth / 2)
                return false;

            hasPublishedPhaseShift = true;
            this.SendEvent(new BossPhaseShiftEvent { Boss = enemy });
            return true;
        }

        void Start()
        {
            if (!isBoss && enemyHealthBar != null)
                enemyHealthBar.SetMaxHealth(maxHealth);
        }
        public void BreakGuard()
        {
            enemy.enemyAnimatorManager.PlayTargetAnimation("Break Guard", true);
        }

        public override void TakeDamage(int physicalDamage, int fireDamage, string damageAnimation, CharacterManager enemyCharacterDamagingMe)
        {
            base.TakeDamage(physicalDamage, fireDamage, damageAnimation, enemyCharacterDamagingMe);
            // 先判断是否要切二阶段，避免后续受伤动画覆盖 Phase Shift
            bool phaseShiftTriggered = TryPublishBossPhaseShift();
            if (isBoss)
            {
                PublishBossHud();
                print("触发boss受伤事件");
            }
            else
            {
                if (enemyHealthBar != null)
                    enemyHealthBar.SetHealth(currentHealth);
            }

            // 若本次触发二阶段，Phase Shift 会由 EnemyBossManager 监听事件来强制播放
            // 这里不要再播放受伤动画，否则会把 Phase Shift 覆盖掉
            if (!phaseShiftTriggered)
                enemy.enemyAnimatorManager.PlayTargetAnimation(damageAnimation, true);

            if (currentHealth <= 0)
            {
                HandleDeath();
            }
        }

        /// <summary>
        /// 受伤时不播放其他动画 主要用于特定动画的使用
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamageNoAnimation(int physicalDamage, int fireDamage, CharacterManager enemyCharacterDamagingMe)
        {
            base.TakeDamageNoAnimation(physicalDamage, fireDamage, enemyCharacterDamagingMe);
            TryPublishBossPhaseShift();
            if (isBoss)
            {
                PublishBossHud();
            }
            else
            {
                if (enemyHealthBar != null)
                    enemyHealthBar.SetHealth(currentHealth);
            }
            if (currentHealth <= 0)
            {
                HandleDeath();
            }
        }

        public override void TakePoiseDamage(int damage)
        {
            if (enemy.isDead)
                return;
            base.TakePoiseDamage(damage);
            TryPublishBossPhaseShift();
            if (isBoss)
            {
                PublishBossHud();
            }
            else
            {
                enemyHealthBar.SetHealth(currentHealth);
            }
            if (currentHealth <= 0)
            {
                HandleDeath();
            }
        }

        // 发布Boss血条事件
        private void PublishBossHud()
        {
            // 只在 Boss 时发布
            if (!isBoss) return;
            // 死亡/0血后不再发布，避免 Boss HUD 在死亡后因为残余伤害又被显示
            if (enemy == null || enemy.isDead || currentHealth <= 0) return;

            string bossName = (enemy != null && enemy.enemyBossManager != null) ? enemy.enemyBossManager.bossName : "Boss";
            var data = new BossHudData(bossName, currentHealth, maxHealth);
            this.SendEvent(new BossHudChangedEvent { Data = data });
        }

        protected override void HandleDeath()
        {
            base.HandleDeath();

            // 禁用敌人身上的功能组件，但保留身体（渲染器和Animator保留用于显示尸体）
            DisableEnemyOnDeath();
        }

        private void DisableEnemyOnDeath()
        {
            // 如果正在被背刺/招架，延迟执行死亡流程
            if (enemy.isBeingBackstabbed || enemy.isBeingRiposted)
            {
                this.GetSystem<ITimerSystem>().CreateTimer(false, 2f, ExecuteDisableOnDeath);
                return;
            }

            ExecuteDisableOnDeath();
        }

        private void ExecuteDisableOnDeath()
        {
            // 禁用 AI 导航
            if (enemy.navmeshAgent != null)
            {
                enemy.navmeshAgent.enabled = false;
            }

            // 禁用刚体物理（先清速度再 kinematic，若已是 kinematic 则跳过清速度）
            if (enemy.enemyRigidbody != null)
            {
                if (!enemy.enemyRigidbody.isKinematic)
                    enemy.enemyRigidbody.velocity = Vector3.zero;
                enemy.enemyRigidbody.isKinematic = true;
            }

            // 禁用碰撞器
            Collider[] colliders = enemy.GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                col.enabled = false;
            }

            // 禁用 AI 状态机
            enemy.stateMachine.Stop(enemy);

            // 禁用血条 UI
            if (enemyHealthBar != null)
            {
                enemyHealthBar.gameObject.SetActive(false);
            }

            // 禁用锁定点
            Transform lockOnTransform = enemy.lockOnTransform;
            if (lockOnTransform != null)
            {
                lockOnTransform.gameObject.SetActive(false);
            }

            // 禁用敌人的 Update 逻辑（EnemyStatsManager 自身仍然活跃，协程可继续运行）
            enemy.enabled = false;

            // 启动消融协程：等待死亡动画 → 逐帧推进 _Dissolve → 消融完毕后回收
            StartCoroutine(DissolveAndRecycle());
        }

        private IEnumerator DissolveAndRecycle()
        {
            // 等待死亡动画播放
            yield return new WaitForSeconds(dissolveDelay);

            Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
            float elapsed = 0f;

            while (elapsed < dissolveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / dissolveDuration);

                foreach (var r in renderers)
                {
                    r.GetPropertyBlock(_mpb);
                    _mpb.SetFloat(DissolvePropertyID, t);
                    r.SetPropertyBlock(_mpb);
                }

                yield return null;
            }

            // 消融完毕，回收进对象池
            this.GetSystem<IPoolSystem>().Recycle(enemy.gameObject);
        }

    }
}

