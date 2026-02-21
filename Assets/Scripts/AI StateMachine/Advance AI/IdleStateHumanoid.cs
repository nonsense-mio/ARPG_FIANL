using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class IdleStateHumanoid : State
    {
        public PursueTargetStateHumanoid pursueTargetState;

        private void Awake()
        {
            pursueTargetState = GetComponent<PursueTargetStateHumanoid>();
        }

        public override State OnUpdate(EnemyManager enemy)
        {
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
