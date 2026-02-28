using UnityEngine;

namespace ARPG
{
    public class AttackStateHumanoid : BaseAttackState
    {
        RotateTowardsTargetStateHumanoid rotateTowardsTargetState;
        CombatStanceStateHumanoid combatStanceState;
        PursueTargetStateHumanoid pursueTargetState;
        PatrolStateHumanoid patrolState;

        private void Awake()
        {
            rotateTowardsTargetState = GetComponent<RotateTowardsTargetStateHumanoid>();
            combatStanceState = GetComponent<CombatStanceStateHumanoid>();
            pursueTargetState = GetComponent<PursueTargetStateHumanoid>();
            patrolState = GetComponent<PatrolStateHumanoid>();
        }

        protected override State GetRotateTowardsState() => rotateTowardsTargetState;
        protected override State GetCombatStanceState() => combatStanceState;
        protected override State GetPursueState() => pursueTargetState;
        protected override State GetFallbackState() => patrolState;
    }
}
