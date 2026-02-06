using ARPG;
using Framework;
using UnityEngine;


namespace HT
{
    public class EnemyBossManager : MonoBehaviour
    {
        public string bossName;

        EnemyManager enemy;
        CombatStanceStateHumanoid bossCombatStanceState;
        [Header("二阶段特效相关")]
        public GameObject particalFX;

        public void Init(EnemyManager enemyManager)
        {
            enemy = enemyManager;
            //bossHealthBar = FindObjectOfType<UIBossHealthBar>();
            bossCombatStanceState = GetComponentInChildren<CombatStanceStateHumanoid>();
        }

        private void OnEnable()
        {
            GameArchitecture.Interface.RegisterEvent<BossPhaseShiftEvent>(e => OnBossPhaseShift(e.Boss))
                .UnRegisterWhenGameObjectDisabled(gameObject);
        }

        private void OnBossPhaseShift(EnemyManager boss)
        {
            // 只响应属于自己的 Boss
            if (boss == null || enemy == null || boss != enemy)
                return;
            if (enemy.isDead)
                return;
            if (bossCombatStanceState != null && bossCombatStanceState.hasPhaseShifted)
                return;
            // 已经在 Phase Shift 中时不重复触发
            if (enemy.isPhaseShifting)
                return;

            ShiftToSecondPhase();
        }


        /// <summary>
        /// 切换到第二阶段
        /// </summary> <summary>
        public void ShiftToSecondPhase()
        {
            enemy.animator.SetBool("isInvulnerable", true);
            enemy.animator.SetBool("isPhaseShifting", true);
            enemy.enemyAnimatorManager.PlayTargetAnimation("Phase Shift", true);
            if (bossCombatStanceState != null)
                bossCombatStanceState.hasPhaseShifted = true;

            // 强制打断当前攻击/连击，避免在 AttackState 卡住只前进
            if (bossCombatStanceState != null && bossCombatStanceState.attackState != null)
            {
                bossCombatStanceState.attackState.currentAttack = null;
                bossCombatStanceState.attackState.hasperformedAttack = false;
            }
            // 清理恢复时间，便于动画结束后正常挑选新攻击
            enemy.currentRecoveryTime = 0;
            // 切回战斗姿态状态，由状态机重新选择二阶段攻击
            enemy.currentState = bossCombatStanceState;
        }
    }

}
