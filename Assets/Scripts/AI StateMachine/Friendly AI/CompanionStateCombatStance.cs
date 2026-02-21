using UnityEngine;

namespace ARPG
{
    public class CompanionStateCombatStance : BaseCombatStanceState
    {
        CompanionStateAttack attackState;
        CompanionStatePursueTarget pursueTargetState;
        CompanionStateFollowHost followHostState;

        private void Awake()
        {
            attackState = GetComponent<CompanionStateAttack>();
            pursueTargetState = GetComponent<CompanionStatePursueTarget>();
            followHostState = GetComponent<CompanionStateFollowHost>();
        }

        protected override BaseAttackState GetAttackState() => attackState;
        protected override State GetPursueState() => pursueTargetState;
        protected override State GetFallbackState() => followHostState;

        // Companion 特有：距离宿主过远时退出战斗
        protected override bool ShouldExitToFallback(EnemyManager enemy)
        {
            return enemy.distanceFromCompanion > enemy.maxDistanceFromCompanion;
        }

        // Companion 特有：格挡需要体力
        protected override bool CanPerformBlock(EnemyManager enemy)
        {
            return enemy.enemyStatsManager.currentStamina > 0;
        }
    }
}
