using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class AmbushState : State
    {
        public bool isSleeping;
        public float detectionRadius = 2;
        public string sleepAnimation;
        public string wakeAnimation;
        public LayerMask detectionLayer;
        public PursueTargetState pursueTargetState;

        public override State Tick(EnemyManager enemy)
        {
            if(isSleeping && enemy.isInteracting == false)
            {
                enemy.enemyAnimatorManager.PlayTargetAnimation(sleepAnimation, true);
            }

            #region 目标检测
            Collider[] colliders = Physics.OverlapSphere(enemy.transform.position, detectionRadius, detectionLayer);

            for(int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].transform.GetComponent<CharacterManager>();

                if(character != null)
                {
                    Vector3 targetDirection = character.transform.position - enemy.transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, enemy.transform.forward);

                    if(viewableAngle > enemy.minimumDetectionAngle &&
                       viewableAngle < enemy.maximumDetectionAngle && targetDirection != Vector3.zero)
                    {
                        enemy.currentTarget = character;
                        isSleeping = false;
                        enemy.enemyAnimatorManager.PlayTargetAnimation(wakeAnimation, true);
                    }
                }
            }
            #endregion

            #region 状态切换
            //当前目标不为null 返回追击状态
            if(enemy.currentTarget != null)
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
