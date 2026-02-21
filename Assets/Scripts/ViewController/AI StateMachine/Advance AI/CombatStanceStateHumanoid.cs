using UnityEngine;

namespace ARPG
{
    public class CombatStanceStateHumanoid : BaseCombatStanceState
    {
        // Boss 阶段切换标志（EnemyBossManager 访问）
        public bool hasPhaseShifted = false;

        // EnemyBossManager 通过 attackState 直接访问 currentAttack/hasperformedAttack
        public AttackStateHumanoid attackState;
        PursueTargetStateHumanoid pursueTargetState;
        PatrolStateHumanoid patrolState;

        private void Awake()
        {
            attackState = GetComponent<AttackStateHumanoid>();
            pursueTargetState = GetComponent<PursueTargetStateHumanoid>();
            patrolState = GetComponent<PatrolStateHumanoid>();
        }

        protected override BaseAttackState GetAttackState() => attackState;
        protected override State GetPursueState() => pursueTargetState;
        protected override State GetFallbackState() => patrolState;
    }
}
