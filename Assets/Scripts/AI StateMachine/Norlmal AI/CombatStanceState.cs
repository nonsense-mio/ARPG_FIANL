using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    /// <summary>
    /// 战斗姿态状态
    /// </summary>
    public class CombatStanceState : State
    {
        public AttackState attackState;
        public PursueTargetState pursueTargetState;
        public EnemyAttackAction[] enemyAttacks;
        protected bool randomDestinationSet = false;
        protected float verticalMovementValue = 0;
        protected float horizontalMovementValue = 0;
        public override State Tick(EnemyManager enemy)
        {
            enemy.animator.SetFloat("Vertical", verticalMovementValue, 0.2f, Time.deltaTime);
            enemy.animator.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime);
            // 仅在挑选到新攻击时再重置 hasperformedAttack
            if (enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                enemy.animator.SetFloat("Horizontal", 0);
                return this;
            }
            //当目标距离大于攻击距离 返回追击状态
             if(enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                return pursueTargetState;
            }
            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(enemy);
            }
            //检查攻击距离
            //进入战斗姿态
            HandleRotateTowardsTarget(enemy);
            //当攻击冷却时间结束 且 当前攻击不为null时 进入攻击状态
            if (enemy.currentRecoveryTime <= 0 && attackState.currentAttack != null)
            {   
                randomDestinationSet = false;
                return attackState;
            }
            //返回当前状态 战斗姿态
            else
            {
                GetNewAttack(enemy);
            }
            return this;  
        }
        protected void HandleRotateTowardsTarget(EnemyManager enemy)
        {
            //手动旋转
            if (enemy.isPerformingAction)
            {
                Vector3 direction = enemy.currentTarget.transform.position - enemy.transform.position;
                direction.y = 0;
                direction.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed / Time.deltaTime);
            }
            //自动寻路的旋转
            else
            {
                Vector3 relativeDirection = enemy.transform.InverseTransformDirection(enemy.navmeshAgent.desiredVelocity);
                Vector3 targetVelocity = enemy.enemyRigidbody.velocity;

                enemy.navmeshAgent.enabled = true;
                //设置寻路目标
                enemy.navmeshAgent.SetDestination(enemy.currentTarget.transform.position);
                enemy.enemyRigidbody.velocity = targetVelocity;
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, enemy.navmeshAgent.transform.rotation, enemy.rotationSpeed / Time.deltaTime);
            }

        }

        protected void DecideCirclingAction(EnemyManager enemy)
        {
       
            WalkAroundTarget(enemy);
        }

       protected void WalkAroundTarget(EnemyManager enemy)
        {
            verticalMovementValue = 0.5f;
            horizontalMovementValue = Random.Range(-1, 1);
            if (horizontalMovementValue <= 1 && horizontalMovementValue > 0)
            {
                horizontalMovementValue = 0.5f;
            }
            else if (horizontalMovementValue >= -1 && horizontalMovementValue < 0)
            {
                horizontalMovementValue = -0.5f;
            }
        }
    
        /// <summary>
        /// 得到攻击的方法
        /// </summary>
       protected virtual void GetNewAttack(EnemyManager enemy)
        {
            int maxScore = 0;
            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                //到达攻击范围内
                if (enemy.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                   enemy.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (enemy.viewableAngle <= enemyAttackAction.maximumAttackAngle &&
                       enemy.viewableAngle >= enemyAttackAction.minimumAttackAngle)
                    {
                        maxScore += enemyAttackAction.attckScore;
                    }
                }
            }

            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                //到达攻击范围内 得到攻击
                if (enemy.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                   enemy.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (enemy.viewableAngle <= enemyAttackAction.maximumAttackAngle &&
                       enemy.viewableAngle >= enemyAttackAction.minimumAttackAngle)
                    {
                        if (attackState.currentAttack != null)
                            return;
                        temporaryScore += enemyAttackAction.attckScore;

                        if (temporaryScore > randomValue)
                        {
                            attackState.currentAttack = enemyAttackAction;
                            attackState.hasperformedAttack = false;
                        }
                    }
                }
            }
        }

    }
}

