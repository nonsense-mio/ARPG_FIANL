using UnityEngine;

namespace ARPG
{
    public class CompanionStateRotateTowardTarget : BaseRotateTowardsTargetState
    {
        CompanionStateCombatStance combatStanceState;

        private void Awake()
        {
            combatStanceState = GetComponent<CompanionStateCombatStance>();
        }

        protected override State GetCombatStanceState() => combatStanceState;
    }
}
