using UnityEngine;

namespace ARPG
{
    public class PursueTargetStateHumanoid : BasePursueTargetState
    {
        CombatStanceStateHumanoid combatStanceState;
        PatrolStateHumanoid patrolState;

        private void Awake()
        {
            combatStanceState = GetComponent<CombatStanceStateHumanoid>();
            patrolState = GetComponent<PatrolStateHumanoid>();
        }

        protected override State GetCombatStanceState() => combatStanceState;
        protected override State GetFallbackState() => patrolState;
    }
}
