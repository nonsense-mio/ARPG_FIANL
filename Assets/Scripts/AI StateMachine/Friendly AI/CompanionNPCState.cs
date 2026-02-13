using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class CompanionNPCState : State
    {
        CompanionStateFollowHost followHostState;
        CompanionStateIdle idleState;

        [Header("NPC 寻路设置")]
        [Tooltip("到达目标位置的判定距离")]
        public float arrivalThreshold = 0.5f;

        // 是否已经到达目标位置
        private bool hasArrivedAtPost = false;

        private void Awake()
        {
            followHostState = GetComponent<CompanionStateFollowHost>();
            idleState = GetComponent<CompanionStateIdle>();
        }

        public override State Tick(EnemyManager enemy)
        {
            // 如果不是NPC，则设置同伴并切换到跟随状态
            if (!enemy.isNPC)
            {
                enemy.companion = PlayerManager.localPlayer;
                hasArrivedAtPost = false; // 重置状态
                return followHostState;
            }

            // 如果没有设置目标位置，直接返回
            if (enemy.posNPC == null)
            {
                return this;
            }

            // 计算与目标位置的距离
            float distanceToPost = Vector3.Distance(enemy.transform.position, enemy.posNPC.position);

            HandleRotateTowardsTarget(enemy);
            // 已经到达目标位置
            if (hasArrivedAtPost || distanceToPost <= arrivalThreshold)
            {
                // 标记已到达
                hasArrivedAtPost = true;

                // 停止寻路
                if (enemy.navmeshAgent.enabled)
                {
                    enemy.navmeshAgent.ResetPath();
                    enemy.navmeshAgent.enabled = false;
                }

                // 停止移动动画
                enemy.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);

                //让NPC面向特定方向
                enemy.transform.rotation = enemy.posNPC.rotation;
                ResetArrivalState();
                return idleState;
            }
            enemy.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);


            return this;
        }

        protected void HandleRotateTowardsTarget(EnemyManager enemy)
        {

            Vector3 relativeDirection = enemy.transform.InverseTransformDirection(enemy.navmeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemy.enemyRigidbody.velocity;

            enemy.navmeshAgent.enabled = true;
            //设置寻路目标
            enemy.navmeshAgent.SetDestination(enemy.posNPC.position);
            enemy.enemyRigidbody.velocity = targetVelocity;
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, enemy.navmeshAgent.transform.rotation, enemy.rotationSpeed / Time.deltaTime);

        }

        /// <summary>
        /// 重置状态（当NPC需要重新回到岗位时调用）
        /// </summary>
        public void ResetArrivalState()
        {
            hasArrivedAtPost = false;
        }
    }



}
