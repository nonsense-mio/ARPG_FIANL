using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class PatrolStateHumanoid : State
    {
        public PursueTargetStateHumanoid pursueTargetState;

        public bool patorlCompleted;
        public bool repeatPatrol;

        [Header("巡逻重置时间")]
        public float endOfPatrolResetTime;
        public float endOfPatrolTimer;
        [Header("巡逻点相关")]
        public int patrolDestinationIndex;
        public bool hasPatrolDestination;
        public Transform currentPatrolDestination;
        public float distanceFromCurrentPatrolDestination;
        public List<Transform> patrolDestinationList = new List<Transform>();

        private void Awake()
        {
            pursueTargetState = GetComponent<PursueTargetStateHumanoid>();
        }

        public override State OnUpdate(EnemyManager enemy)
        {
            //巡逻时检测目标
            SearchForTargetWhilePatrol(enemy);

            if (enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                enemy.animator.SetFloat("Horizontal", 0);
                return this;
            }
            //发现目标则切换到追击状态

            if (enemy.currentTarget != null)
            {
                return pursueTargetState;
            }
            //巡逻已完成且允许重复巡逻：先原地等一段时间，再重置巡逻索引
            if (patorlCompleted && repeatPatrol)
            {
                if (endOfPatrolResetTime > endOfPatrolTimer)
                {
                    endOfPatrolTimer += Time.deltaTime;
                    enemy.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                    return this;
                }
                else
                {
                    patrolDestinationIndex = -1;
                    hasPatrolDestination = false;
                    currentPatrolDestination = null;
                    patorlCompleted = false;
                    endOfPatrolTimer = 0;
                }
            }
            //巡逻已完成且不重复巡逻：关闭 NavMeshAgent，停住
            else if (patorlCompleted && !repeatPatrol)
            {
                enemy.navmeshAgent.enabled = false;
                enemy.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                return this;
            }
            //如果当前有巡逻目的地，则前往该目的地
            if (hasPatrolDestination)
            {
                if (currentPatrolDestination != null)
                {
                    distanceFromCurrentPatrolDestination = Vector3.Distance(enemy.transform.position, currentPatrolDestination.position);
                    //设置目的地、同步旋转、播放行走动画
                    if (distanceFromCurrentPatrolDestination > 1)
                    {
                        enemy.navmeshAgent.enabled = true;
                        enemy.navmeshAgent.SetDestination(currentPatrolDestination.position);
                        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, enemy.navmeshAgent.transform.rotation, 5f);
                        enemy.animator.SetFloat("Vertical", 0.5f, 0.2f, Time.deltaTime);
                    }
                    //到达目的地后，清空目的地
                    else
                    {
                        currentPatrolDestination = null;
                        hasPatrolDestination = false;
                    }
                }

            }

            //如果没有巡逻目的地，则设置下一个巡逻目的地
            if (!hasPatrolDestination)
            {
                patrolDestinationIndex++;
                //超出巡逻点列表长度则标记巡逻完成
                if (patrolDestinationIndex > patrolDestinationList.Count - 1)
                {
                    patorlCompleted = true;
                    return this;
                }
                currentPatrolDestination = patrolDestinationList[patrolDestinationIndex];
                hasPatrolDestination = true;
            }

            return this;


        }


        private void SearchForTargetWhilePatrol(EnemyManager enemy)
        {
            enemy.TryDetectTarget();
        }

    }
}

