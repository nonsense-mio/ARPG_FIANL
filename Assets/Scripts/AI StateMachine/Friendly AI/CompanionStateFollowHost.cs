using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class CompanionStateFollowHost : State
    {
        CompanionStateIdle idleState;
        CompanionNPCState companionNPCState;
        void Awake()
        {
            idleState = GetComponent<CompanionStateIdle>();
            companionNPCState = GetComponent<CompanionNPCState>();
        }

        public override State Tick(EnemyManager enemy)
        {
            enemy.enemyEffectsManager.InterrupEffect();
            if (enemy.isNPC)
            {
                return companionNPCState;
            }
            if (enemy.companion == null)
            {
                return idleState;
            }
            if (enemy.isInteracting)
                return this;
            if (enemy.isPerformingAction)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }
            HandleRotateTowardsTarget(enemy);

            if (enemy.distanceFromCompanion > enemy.maxDistanceFromCompanion)
            {
                enemy.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }

            //当与友军的距离小于最小距离 进入待机状态
            if (enemy.distanceFromCompanion <= enemy.returnDistanceFromCompanion)
            {

                return idleState;
            }
            else
            {
                return this;
            }

        }

        protected void HandleRotateTowardsTarget(EnemyManager enemy)
        {

            Vector3 relativeDirection = enemy.transform.InverseTransformDirection(enemy.navmeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemy.enemyRigidbody.velocity;

            enemy.navmeshAgent.enabled = true;
            //设置寻路目标
            enemy.navmeshAgent.SetDestination(enemy.companion.transform.position);
            enemy.enemyRigidbody.velocity = targetVelocity;
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, enemy.navmeshAgent.transform.rotation, enemy.rotationSpeed * Time.deltaTime);

        }
    }
}

