using UnityEngine;

namespace ARPG
{
    public class CompanionStateAttack : BaseAttackState
    {
        CompanionStateRotateTowardTarget rotateTowardsTargetState;
        CompanionStateCombatStance combatStanceState;
        CompanionStatePursueTarget pursueTargetState;
        CompanionStateFollowHost followHostState;

        private void Awake()
        {
            rotateTowardsTargetState = GetComponent<CompanionStateRotateTowardTarget>();
            combatStanceState = GetComponent<CompanionStateCombatStance>();
            pursueTargetState = GetComponent<CompanionStatePursueTarget>();
            followHostState = GetComponent<CompanionStateFollowHost>();
        }

        protected override State GetRotateTowardsState() => rotateTowardsTargetState;
        protected override State GetCombatStanceState() => combatStanceState;
        protected override State GetPursueState() => pursueTargetState;
        protected override State GetFallbackState() => followHostState;
    }
}
