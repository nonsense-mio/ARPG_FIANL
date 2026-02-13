using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class CompanionStateIdle : State
    {
        public LayerMask detectionLayer;
        public LayerMask layersThatBlockLineOfSight;
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
            #region 目标的检测
            //在球形范围内 寻找目标
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemy.detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

                if (targetCharacter != null)
                {
                    //排除队友
                    if (targetCharacter.characterStatsManager.teamIDNumber == enemy.enemyStatsManager.teamIDNumber)
                        continue;
                    Vector3 targetDirection = targetCharacter.transform.position - enemy.transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
                    //当范围检测到的目标在视野范围内便设立追击目标
                    if (viewableAngle > enemy.minimumDetectionAngle && viewableAngle < enemy.maximumDetectionAngle && targetDirection != Vector3.zero)
                    {

                        if (Physics.Linecast(enemy.lockOnTransform.position, targetCharacter.lockOnTransform.position, layersThatBlockLineOfSight))
                        {
                            return this;
                        }
                        else
                        {
                            enemy.currentTarget = targetCharacter;
                        }

                    }
                }
            }
            #endregion

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

