using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class EnemyStatsManager : CharacterStatsManager
    {
        EnemyManager enemy;
        public UIEnemyHealthBar enemyHealthBar;
        public bool isBoss;

        // 二阶段事件只触发一次（避免重复触发导致动画被反复打断）
        public bool hasPublishedPhaseShift;

        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            enemy = characterMgr as EnemyManager;
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            hasPublishedPhaseShift = false;
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
            EventCenter.Instance.EventTrigger<EnemyManager>(E_EventType.E_BossPhaseShift, enemy);
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
            EventCenter.Instance.EventTrigger<BossHudData?>(E_EventType.E_BossHudChanged, data);
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
                StartCoroutine(WaitForCriticalThenDisable());
                return;
            }

            ExecuteDisableOnDeath();
        }

        private IEnumerator WaitForCriticalThenDisable()
        {
            // 等待背刺/招架状态结束
            // while (enemy.isBeingBackstabbed || enemy.isBeingRiposted)
            // {
            //     yield return null;
            // }

            // 额外等待一小段时间确保动画完成
            yield return new WaitForSeconds(2f);

            ExecuteDisableOnDeath();
        }

        private void ExecuteDisableOnDeath()
        {
            // 禁用 AI 导航
            if (enemy.navmeshAgent != null)
            {
                enemy.navmeshAgent.enabled = false;
            }

            // 禁用刚体物理
            if (enemy.enemyRigidbody != null)
            {
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
            enemy.currentState = null;

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

            // 禁用敌人的 Update 逻辑
            enemy.enabled = false;

            StartCoroutine(DestroyAfterDelay(10f));
        }

        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(enemy.gameObject);
        }

    }
}

