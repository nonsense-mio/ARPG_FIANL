using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class CompanionStateIdle : State
    {
        CompanionNPCState companionNPCState;
        CompanionStateFollowHost followHostState;
        CompanionStatePursueTarget pursueTargetState;
        void Awake()
        {
            companionNPCState = GetComponent<CompanionNPCState>();
            followHostState = GetComponent<CompanionStateFollowHost>();
            pursueTargetState = GetComponent<CompanionStatePursueTarget>();
        }

        public override State Tick(EnemyManager enemy)
        {
            enemy.enemyEffectsManager.InterrupEffect();
            enemy.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            enemy.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
            //如果是NPC 则进入NPC状态
            if (enemy.isNPC)
            {
                return companionNPCState;
            }
            if (enemy.distanceFromCompanion > enemy.maxDistanceFromCompanion)
            {
                return followHostState;
            }
            enemy.TryDetectTarget();

            #region 状态的切换
            //追击目标不为空时 返回追击状态
            if (enemy.currentTarget != null)
            {
                return pursueTargetState;
            }
            //否则返回当前状态
            else
            {
                return this;
            }
            #endregion
        }
    }
}

