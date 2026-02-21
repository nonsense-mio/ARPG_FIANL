using UnityEngine;

namespace ARPG
{
    public class RotateTowardsTargetStateHumanoid : BaseRotateTowardsTargetState
    {
        public CombatStanceStateHumanoid combatStanceState;

        private void Awake()
        {
            combatStanceState = GetComponent<CombatStanceStateHumanoid>();
        }

        protected override State GetCombatStanceState() => combatStanceState;
    }
}
