using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class CompanionStatePursueTarget : State
    {

        CompanionStateCombatStance combatStanceState;
        CompanionStateFollowHost followHostState;
        private void Awake()
        {
            combatStanceState = GetComponent<CompanionStateCombatStance>();
            followHostState = GetComponent<CompanionStateFollowHost>();
        }
        public override State Tick(EnemyManager enemy)
        {
            //如果与友军距离过远 则返回跟随状态
            if (enemy.distanceFromCompanion > enemy.maxDistanceFromCompanion)
            {
                return followHostState;
            }

            //剑盾战斗风格
            if (enemy.combatStyle == E_AICombatStyle.SwordAndShield)
            {
                return ProcessSwordAndShieldCombat(enemy);
            }
            //弓箭手战斗风格
            else if (enemy.combatStyle == E_AICombatStyle.Archer)
            {
                return ProcessArcherCombat(enemy);
            }
            else
            {
                return this;
            }


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
            //当与目标之间的距离大于攻击距离距离
            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                enemy.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                enemy.animator.SetFloat("Horizontal", 0);
            }

            //当与目标的距离小于攻击距离 进入战斗姿态
            if (enemy.distanceFromTarget <= enemy.maximumAggroRadius)
            {
                return combatStanceState;
            }
            else
            {
                return this;
            }
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
            //当与目标之间的距离大于攻击距离距离
            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                if (!enemy.isStationaryArcher)
                {
                    enemy.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                }
            }

            //当与目标的距离小于攻击距离 进入战斗姿态
            if (enemy.distanceFromTarget <= enemy.maximumAggroRadius)
            {
                return combatStanceState;
            }
            else
            {
                return this;
            }

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

    }
}
