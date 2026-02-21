using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 旋转朝向目标状态的共享逻辑基类。
    /// 子类只需提供 GetCombatStanceState() 返回对应的战斗姿态状态。
    /// </summary>
    public abstract class BaseRotateTowardsTargetState : State
    {
        protected abstract State GetCombatStanceState();

        public override State Tick(EnemyManager enemy)
        {
            enemy.animator.SetFloat("Vertical", 0);
            enemy.animator.SetFloat("Horizontal", 0);

            if (enemy.isInteracting)
                return this;

            if (enemy.viewableAngle >= 101 && enemy.viewableAngle <= 180)
            {
                enemy.enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Behind", true);
            }
            else if (enemy.viewableAngle <= -101 && enemy.viewableAngle >= -180)
            {
                enemy.enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Behind", true);
            }
            else if (enemy.viewableAngle >= 45 && enemy.viewableAngle <= 100)
            {
                enemy.enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Left", true);
            }
            else if (enemy.viewableAngle <= -45 && enemy.viewableAngle >= -100)
            {
                enemy.enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Right", true);
            }

            return GetCombatStanceState();
        }
    }
}
