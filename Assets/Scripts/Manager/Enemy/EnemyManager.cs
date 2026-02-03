using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HT
{
    public class EnemyManager : CharacterManager
    {

        public EnemyBossManager enemyBossManager;
        public EnemyStatsManager enemyStatsManager;
        public EnemyEffectsManager enemyEffectsManager;
        public EnemyAnimatorManager enemyAnimatorManager;
        public EnemyInventoryManager enemyInventoryManager;
        public EnemyWeaponSlotManager enemyWeaponSlotManager;
        public EnemyLocomotionManager enemyLocomotionManager;

        public State currentState;
        public CharacterManager currentTarget;
        public NavMeshAgent navmeshAgent;
        public Rigidbody enemyRigidbody;

        public bool isPerformingAction;
        public float rotationSpeed = 15;
        public float maximumAggroRadius = 1.5f;

        [Header("AI 设置")]
        //敌人可以检测到玩家的半径
        public float detectionRadius = 20;
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;
        public float currentRecoveryTime = 0;
        public float stoppingDistance = 1.2f;
        [Header("高级AI 设置")]
        public bool allowToPerformBlock;
        public int blockLikelyHood;
        public bool allowToPerformDodge;
        public int dodgeLikelyHood;
        public bool allowToPerformParry;
        public int parryLikelyHood;

        [Header("AI 战斗相关")]
        public bool allowAIToPerformCombos;
        public bool isPhaseShifting;
        public float comboLikelyHood;
        public E_AICombatStyle combatStyle;
        [Header("AI 弓箭手")]
        public bool isStationaryArcher;

        public float minimumTimeToAimAtTarget = 1.5f;
        public float maximumTimeToAimAtTarget = 3.5f;
        [Header("AI同盟设置")]
        public float maxDistanceFromCompanion;  //和友军的最大距离
        public float minDistanceFromCompanion;  //和友军的最小距离
        public float returnDistanceFromCompanion = 2; //回归到友军时保持的距离
        public float distanceFromCompanion;
        public CharacterManager companion;
        public bool isNPC;
        public Transform posNPC;//NPC对话位置

        [Header("AI 目标相关")]
        public float distanceFromTarget;// 与目标的距离
        public Vector3 targetDirection;// 目标方向向量
        public float viewableAngle;// 目标与正前方的夹角
        protected override void Awake()
        {
            base.Awake();
            navmeshAgent.enabled = false;
            enemyRigidbody = GetComponent<Rigidbody>();

            enemyBossManager = GetComponent<EnemyBossManager>();
            enemyStatsManager = GetComponent<EnemyStatsManager>();
            enemyEffectsManager = GetComponent<EnemyEffectsManager>();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            enemyInventoryManager = GetComponent<EnemyInventoryManager>();
            enemyWeaponSlotManager = GetComponent<EnemyWeaponSlotManager>();
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();

            enemyBossManager?.Init(this);
            enemyStatsManager.Init(this);
            enemyEffectsManager.Init(this);
            enemyAnimatorManager.Init(this);
            enemyInventoryManager.Init(this);
            enemyWeaponSlotManager.Init(this);
            enemyLocomotionManager.Init(this);

        }
        private void IsNPCChanged(bool val)
        {
            isNPC = !val;
            companion = val ? PlayerManager.localPlayer : null;
        }
        void OnEnable()
        {
            EventCenter.Instance.AddEventListener<bool>(E_EventType.E_NPC_FollowPlayer, IsNPCChanged);
        }
        void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener<bool>(E_EventType.E_NPC_FollowPlayer, IsNPCChanged);
        }
        private void Start()
        {
            enemyRigidbody.isKinematic = false;
        }

        private void Update()
        {
            HandleRecoveryTimer();
            HandleStateMachine();
            isRotatingWithRootMotion = animator.GetBool("isRotatingWithRootMotion");
            isInteracting = animator.GetBool("isInteracting");
            isPhaseShifting = animator.GetBool("isPhaseShifting");
            isInvulnerable = animator.GetBool("isInvulnerable");
            isHoldingArrow = animator.GetBool("isHoldingArrow");
            canDoCombo = animator.GetBool("canDoCombo");
            canRotate = animator.GetBool("canRotate");
            animator.SetBool("isDead", isDead);
            animator.SetBool("isBlocking", isBlocking);
            animator.SetBool("isTwoHandingWeapon", isTwoHandingWeapon);
            animator.SetBool("isGrounded", isGrounded);
            if (currentTarget != null && !isDead)
            {
                distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
                targetDirection = currentTarget.transform.position - transform.position;
                viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            }
            if(companion != null)
            {
                distanceFromCompanion = Vector3.Distance(companion.transform.position, transform.position);
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            enemyEffectsManager.HandleAllEffects();
        }

        private void LateUpdate()
        {
            navmeshAgent.transform.localPosition = Vector3.zero;
            navmeshAgent.transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// 处理状态机
        /// </summary>
        private void HandleStateMachine()
        {
            if (isDead)
                return;
            if (currentState != null)
            {
                State nextState = currentState.Tick(this);
                if (nextState != null)
                {
                    SwitchToNextState(nextState);
                }
            }
        }
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="state"></param>
        private void SwitchToNextState(State state)
        {
            currentState = state;
        }

        /// <summary>
        /// 处理恢复时间
        /// </summary>
        private void HandleRecoveryTimer()
        {
            if (currentRecoveryTime > 0)
            {
                currentRecoveryTime -= Time.deltaTime;
            }
            if (isPerformingAction)
            {
                if (currentRecoveryTime <= 0)
                {
                    isPerformingAction = false;
                }
            }
        }


    }
}



