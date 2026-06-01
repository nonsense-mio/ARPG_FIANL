using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 追击目标状态的共享逻辑基类。
    /// 子类通过 GetFallbackState/GetCombatStanceState 提供退出状态，
    /// 通过 GetEarlyExitState 添加额外前置检查（如 Companion 距离限制）。
    /// </summary>
    public abstract class BasePursueTargetState : State
    {
        protected abstract State GetCombatStanceState();
        protected abstract State GetFallbackState();

        /// <summary>
        /// 子类可重写此方法添加额外的前置退出检查。
        /// 返回非 null 表示需要提前退出到该状态。
        /// </summary>
        protected virtual State GetEarlyExitState(EnemyManager enemy) => null;

        public override State OnUpdate(EnemyManager enemy)
        {
            State earlyExit = GetEarlyExitState(enemy);
            if (earlyExit != null) return earlyExit;

            if (enemy.currentTarget == null)
            {
                State fallback = GetFallbackState();
                return fallback != null ? fallback : (State)this;
            }

            if (enemy.distanceFromTarget >= enemy.detectionRadius)
            {
                enemy.currentTarget = null;
                State fallback = GetFallbackState();
                return fallback != null ? fallback : (State)this;
            }

            if (enemy.combatStyle == E_AICombatStyle.SwordAndShield)
                return ProcessSwordAndShieldCombat(enemy);
            else if (enemy.combatStyle == E_AICombatStyle.Archer)
                return ProcessArcherCombat(enemy);
            else
                return this;
        }

        private State ProcessSwordAndShieldCombat(EnemyManager enemy)
        {
            HandleRotateTowardsTarget(enemy);

            if (enemy.isInteracting)
                return this;

            if (enemy.isPerformingAction)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                enemy.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                enemy.animator.SetFloat("Horizontal", 0);
            }

            if (enemy.distanceFromTarget <= enemy.maximumAggroRadius)
                return GetCombatStanceState();
            else
                return this;
        }

        private State ProcessArcherCombat(EnemyManager enemy)
        {
            HandleRotateTowardsTarget(enemy);

            if (enemy.isInteracting)
                return this;

            if (enemy.isPerformingAction)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                if (!enemy.isStationaryArcher)
                {
                    enemy.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                }
            }

            if (enemy.distanceFromTarget <= enemy.maximumAggroRadius)
                return GetCombatStanceState();
            else
                return this;
        }

        private void HandleRotateTowardsTarget(EnemyManager enemy)
        {
            //手动旋转
            if (enemy.isPerformingAction)
            {
                Vector3 direction = enemy.currentTarget.transform.position - enemy.transform.position;
                direction.y = 0;
                direction.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed * Time.deltaTime);
            }
            //自动寻路的旋转
            else
            {
                Vector3 targetVelocity = enemy.enemyRigidbody.velocity;

                enemy.navmeshAgent.enabled = true;
                enemy.navmeshAgent.SetDestination(enemy.currentTarget.transform.position);
                enemy.enemyRigidbody.velocity = targetVelocity;
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, enemy.navmeshAgent.transform.rotation, enemy.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
