using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace ARPG
{
    /// <summary>
    /// 攻击状态
    /// </summary>
    public class AttackState : State
    {
        public RotateTowardsTargetState rotateTowardsTargetState;
        public CombatStanceState combatStanceState;
        public PursueTargetState pursueTargetState;
        public EnemyAttackAction currentAttack;
        bool willDoComboOnNext = false;
        public bool hasperformedAttack = false;
        //选择一种攻击
        //如果攻击可行 选择一个新攻击
        //如果攻击不可行 停止攻击并攻击目标
        //设置攻击恢复时间
        //返回到战斗姿态的状态
        public override State Tick(EnemyManager enemy)
        {
            float distanceFromTarget = Vector3.Distance(enemy.currentTarget.transform.position, enemy.transform.position);
            RotateTowardsTargetWhileAttacking(enemy);
            //当与目标距离大于仇恨距离时 返回追击状态
            if (distanceFromTarget > enemy.maximumAggroRadius)
            {
                return pursueTargetState;
            }
            if (willDoComboOnNext && enemy.canDoCombo)
            {
                if (currentAttack == null)
                {
                    Debug.Log("当前连击攻击为空");
                }
                AttackTargetWithCombo(enemy);
                return this;
            }
            if (!hasperformedAttack && distanceFromTarget <= currentAttack.maximumDistanceNeededToAttack)
            {
                AttackTarget(enemy);
                RollForComboChance(enemy);
                if (!willDoComboOnNext)
                {
                    currentAttack = null;
                }
            }
            if (willDoComboOnNext && hasperformedAttack)
            {
                return this;
            }
            //如果目标超出当前攻击范围 则重置当前攻击
            if (currentAttack != null && distanceFromTarget > currentAttack.maximumDistanceNeededToAttack)
            {
                currentAttack = null;
            }

            return rotateTowardsTargetState;
        }

        private void AttackTarget(EnemyManager enemy)
        {
            enemy.UpdateWhichHandCharacterIsUsing(currentAttack.isRightHandedAction);
            enemy.enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            enemy.currentRecoveryTime = currentAttack.recoveryTime;
            hasperformedAttack = true;
        }
        private void AttackTargetWithCombo(EnemyManager enemy)
        {
            enemy.UpdateWhichHandCharacterIsUsing(currentAttack.isRightHandedAction);
            willDoComboOnNext = false;
            enemy.enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            enemy.currentRecoveryTime = currentAttack.recoveryTime;
            currentAttack = null;
        }

        private void RotateTowardsTargetWhileAttacking(EnemyManager enemy)
        {
            //手动旋转
            if (enemy.canRotate && enemy.isInteracting)
            {
                Vector3 direction = enemy.currentTarget.transform.position - enemy.transform.position;
                direction.y = 0;
                direction.Normalize();
                if (direction == Vector3.zero)
                {
                    direction = enemy.transform.forward;
                }
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed / Time.deltaTime);
            }

        }

        private void RollForComboChance(EnemyManager enemyManager)
        {
            float comboChance = Random.Range(0, 100);
            if (enemyManager.allowAIToPerformCombos && comboChance <= enemyManager.comboLikelyHood)
            {
                if (currentAttack.comboAction != null)
                {
                    willDoComboOnNext = true;
                    currentAttack = currentAttack.comboAction;
                }
                else
                {
                    willDoComboOnNext = false;
                    currentAttack = null;
                }

            }
        }

    }
}

