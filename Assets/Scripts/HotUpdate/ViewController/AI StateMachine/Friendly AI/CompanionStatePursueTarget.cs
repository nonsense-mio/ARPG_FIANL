using UnityEngine;

namespace ARPG
{
    public class CompanionStatePursueTarget : BasePursueTargetState
    {
        CompanionStateCombatStance combatStanceState;
        CompanionStateFollowHost followHostState;

        private void Awake()
        {
            combatStanceState = GetComponent<CompanionStateCombatStance>();
            followHostState = GetComponent<CompanionStateFollowHost>();
        }

        protected override State GetCombatStanceState() => combatStanceState;
        protected override State GetFallbackState() => followHostState;

        // Companion 特有：距离宿主过远时返回跟随状态
        protected override State GetEarlyExitState(EnemyManager enemy)
        {
            return enemy.distanceFromCompanion > enemy.maxDistanceFromCompanion
                ? (State)followHostState
                : null;
        }
    }
}
