using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 攻击状态的共享逻辑基类。
    /// 子类通过抽象方法提供旋转/战斗姿态/追击/退出状态的引用。
    /// </summary>
    public abstract class BaseAttackState : State
    {
        protected abstract State GetRotateTowardsState();
        protected abstract State GetCombatStanceState();
        protected abstract State GetPursueState();
        protected abstract State GetFallbackState();

        public ItemBasedAttackAction currentAttack;
        bool willDoComboOnNext = false;
        public bool hasPerformedAttack = false;

        public override State Tick(EnemyManager enemy)
        {
            if (enemy.currentTarget == null)
            {
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

        #region 剑盾攻击方式
        private State ProcessSwordAndShieldCombat(EnemyManager enemy)
        {
            if (enemy == null || enemy.currentTarget == null || enemy.currentTarget.isDead)
            {
                ResetStateFlags();
                if (enemy != null) enemy.currentTarget = null;
                return GetCombatStanceState();
            }

            RotateTowardsTargetWhileAttacking(enemy);

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                ResetStateFlags();
                return GetPursueState();
            }

            if (currentAttack == null)
            {
                ResetStateFlags();
                return GetCombatStanceState();
            }

            if (willDoComboOnNext && currentAttack.canCombo)
            {
                AttackTargetWithCombo(enemy);
                return this;
            }

            if (!hasPerformedAttack && enemy.distanceFromTarget <= currentAttack.maximumDistanceNeededToAttack)
            {
                AttackTarget(enemy);
                RollForComboChance(enemy);
                if (!willDoComboOnNext)
                {
                    currentAttack = null;
                }
            }

            if (willDoComboOnNext && hasPerformedAttack)
            {
                return this;
            }

            if (currentAttack != null && enemy.distanceFromTarget > currentAttack.maximumDistanceNeededToAttack)
            {
                currentAttack = null;
            }

            ResetStateFlags();
            return GetRotateTowardsState();
        }
        #endregion

        #region 弓箭手攻击方式
        private State ProcessArcherCombat(EnemyManager enemy)
        {
            RotateTowardsTargetWhileAttacking(enemy);

            if (enemy.isInteracting)
                return this;

            if (!enemy.isHoldingArrow)
            {
                ResetStateFlags();
                return GetCombatStanceState();
            }

            if (enemy.currentTarget != null && enemy.currentTarget.isDead)
            {
                ResetStateFlags();
                enemy.currentTarget = null;
                return this;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                ResetStateFlags();
                return GetPursueState();
            }

            if (!hasPerformedAttack && enemy.isHoldingArrow)
            {
                FireAmmo(enemy);
            }

            ResetStateFlags();
            return GetRotateTowardsState();
        }
        #endregion

        private void AttackTarget(EnemyManager enemy)
        {
            if (currentAttack == null) return;
            currentAttack.PerformAttackAction(enemy);
            enemy.currentRecoveryTime = currentAttack.recoveryTime;
            hasPerformedAttack = true;
        }

        private void AttackTargetWithCombo(EnemyManager enemy)
        {
            if (currentAttack == null) return;
            currentAttack.PerformAttackAction(enemy);
            willDoComboOnNext = false;
            enemy.currentRecoveryTime = currentAttack.recoveryTime;
            currentAttack = null;
        }

        private void RotateTowardsTargetWhileAttacking(EnemyManager enemy)
        {
            if (enemy == null) return;

            if (enemy.canRotate && enemy.isInteracting)
            {
                if (enemy.currentTarget == null) return;

                Vector3 direction = enemy.currentTarget.transform.position - enemy.transform.position;
                direction.y = 0;
                direction.Normalize();
                if (direction == Vector3.zero)
                {
                    direction = enemy.transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed * Time.deltaTime);
            }
        }

        private void RollForComboChance(EnemyManager enemyManager)
        {
            float comboChance = Random.Range(0, 100);
            if (enemyManager.allowAIToPerformCombos && comboChance <= enemyManager.comboLikelyHood)
            {
                if (currentAttack.canCombo)
                {
                    willDoComboOnNext = true;
                }
                else
                {
                    willDoComboOnNext = false;
                    currentAttack = null;
                }
            }
        }

        private void ResetStateFlags()
        {
            willDoComboOnNext = false;
            hasPerformedAttack = false;
        }

        private void FireAmmo(EnemyManager enemy)
        {
            if (enemy.isHoldingArrow)
            {
                hasPerformedAttack = true;
                enemy.characterInventoryManager.currentItemBeingUsed = enemy.characterInventoryManager.rightWeapon;
                enemy.characterInventoryManager.rightWeapon.th_tap_Mouse_L_Action.PerformAction(enemy);
            }
        }
    }
}
