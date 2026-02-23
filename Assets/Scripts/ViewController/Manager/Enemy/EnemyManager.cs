using System.Collections;
using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace ARPG
{
    public class EnemyManager : CharacterManager, IPoolable
    {

        public EnemyBossManager enemyBossManager;
        public EnemyStatsManager enemyStatsManager;
        public EnemyEffectsManager enemyEffectsManager;
        public EnemyAnimatorManager enemyAnimatorManager;
        public EnemyInventoryManager enemyInventoryManager;
        public EnemyWeaponSlotManager enemyWeaponSlotManager;
        public EnemyLocomotionManager enemyLocomotionManager;

        [Header("状态机")]
        [FormerlySerializedAs("currentState")]
        [SerializeField] State initialState;
        public StateMachineController stateMachine = new StateMachineController();

        /// <summary>
        /// 兼容属性：外部读取当前状态
        /// </summary>
        public State currentState => stateMachine.CurrentState;

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

        [Header("AI 检测图层")]
        public LayerMask detectionLayer;
        public LayerMask layersThatBlockLineOfSight;

        [Header("AI 目标相关")]
        public float distanceFromTarget;// 与目标的距离
        public Vector3 targetDirection;// 目标方向向量
        public float viewableAngle;// 目标与正前方的夹角

        // Animator 参数 Hash 缓存
        private int _hashIsRotatingWithRootMotion;
        private int _hashIsInteracting;
        public int _hashIsPhaseShifting;
        public int _hashIsInvulnerable;
        private int _hashIsHoldingArrow;
        private int _hashCanDoCombo;
        private int _hashCanRotate;
        private int _hashIsDead;
        private int _hashIsBlocking;
        private int _hashIsTwoHandingWeapon;
        private int _hashIsGrounded;

        // isInteracting 安全阀：防止因无效动画导致永久卡死
        private const float MAX_INTERACTING_DURATION = 5f;
        private float _isInteractingTimer;

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

            _hashIsRotatingWithRootMotion = Animator.StringToHash("isRotatingWithRootMotion");
            _hashIsInteracting            = Animator.StringToHash("isInteracting");
            _hashIsPhaseShifting          = Animator.StringToHash("isPhaseShifting");
            _hashIsInvulnerable           = Animator.StringToHash("isInvulnerable");
            _hashIsHoldingArrow           = Animator.StringToHash("isHoldingArrow");
            _hashCanDoCombo               = Animator.StringToHash("canDoCombo");
            _hashCanRotate                = Animator.StringToHash("canRotate");
            _hashIsDead                   = Animator.StringToHash("isDead");
            _hashIsBlocking               = Animator.StringToHash("isBlocking");
            _hashIsTwoHandingWeapon       = Animator.StringToHash("isTwoHandingWeapon");
            _hashIsGrounded               = Animator.StringToHash("isGrounded");
        }
        private void IsNPCChanged(bool val)
        {
            isNPC = !val;
            companion = val ? PlayerManager.localPlayer : null;
        }
        void OnEnable()
        {
            this.RegisterEvent<NPCFollowPlayerEvent>(e => IsNPCChanged(e.IsFollowing))
                .UnRegisterWhenGameObjectDisabled(gameObject);
        }
        private void Start()
        {
            enemyRigidbody.isKinematic = false;
            stateMachine.Initialize(initialState, this);
        }

        private void Update()
        {
            HandleRecoveryTimer();
            HandleStateMachine();
            HandleIsInteractingSafetyValve();

            isRotatingWithRootMotion = animator.GetBool(_hashIsRotatingWithRootMotion);
            isInteracting            = animator.GetBool(_hashIsInteracting);
            isPhaseShifting          = animator.GetBool(_hashIsPhaseShifting);
            isInvulnerable           = animator.GetBool(_hashIsInvulnerable);
            isHoldingArrow           = animator.GetBool(_hashIsHoldingArrow);
            canDoCombo               = animator.GetBool(_hashCanDoCombo);
            canRotate                = animator.GetBool(_hashCanRotate);
            animator.SetBool(_hashIsDead, isDead);
            animator.SetBool(_hashIsBlocking, isBlocking);
            animator.SetBool(_hashIsTwoHandingWeapon, isTwoHandingWeapon);
            animator.SetBool(_hashIsGrounded, isGrounded);
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

        private void HandleStateMachine()
        {
            if (isDead)
                return;
            stateMachine.Update(this);
        }

        /// <summary>
        /// isInteracting 安全阀：防止因无效动画名导致 isInteracting 永久卡 true
        /// </summary>
        private void HandleIsInteractingSafetyValve()
        {
            if (isInteracting)
            {
                _isInteractingTimer += Time.deltaTime;
                if (_isInteractingTimer > MAX_INTERACTING_DURATION)
                {
                    animator.SetBool(_hashIsInteracting, false);
                    _isInteractingTimer = 0;
                }
            }
            else
            {
                _isInteractingTimer = 0;
            }
        }

        /// <summary>
        /// 球形检测范围内寻找有效目标（排除队友、死亡目标、视线遮挡）
        /// 找到目标后设置 currentTarget 并返回 true。
        /// </summary>
        public bool TryDetectTarget()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

                if (targetCharacter == null) continue;
                if (targetCharacter.characterStatsManager.teamIDNumber == enemyStatsManager.teamIDNumber) continue;
                if (targetCharacter.isDead) continue;

                Vector3 targetDir = targetCharacter.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);

                if (angle > minimumDetectionAngle && angle < maximumDetectionAngle && targetDir != Vector3.zero)
                {
                    if (!Physics.Linecast(lockOnTransform.position, targetCharacter.lockOnTransform.position, layersThatBlockLineOfSight))
                    {
                        currentTarget = targetCharacter;
                        return true;
                    }
                }
            }
            return false;
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

        #region IPoolable

        /// <summary>
        /// 回收入池前：销毁武器模型，防止脏武器随池对象跨场景/跨复用残留
        /// </summary>
        public void OnRecycle()
        {
            enemyWeaponSlotManager.UnloadAllWeaponModels();
        }

        /// <summary>
        /// 从池中取出后：完整重初始化，等效于首次 Awake+Start 的效果
        /// </summary>
        public void OnSpawn()
        {
            // 重置代码拥有的战斗/AI 标志位
            isDead = false;
            isAttacking = false;
            isParrying = false;
            isBeingBackstabbed = false;
            isBeingRiposted = false;
            isPerformingAction = false;
            currentRecoveryTime = 0f;
            currentTarget = null;

            // Animator 驱动的参数：重置 isInteracting 防止安全阀延迟触发
            animator.SetBool(_hashIsInteracting, false);

            // 重新启用 EnemyManager 脚本（ExecuteDisableOnDeath 中被 disabled）
            enabled = true;

            // 重置 Rigidbody 物理（Start 中设为 false，ExecuteDisableOnDeath 改为 kinematic）
            if (enemyRigidbody != null)
                enemyRigidbody.isKinematic = false;

            // 重新启用碰撞器（ExecuteDisableOnDeath 中全部 disabled）
            foreach (var col in GetComponentsInChildren<Collider>())
                col.enabled = true;

            // 重新启用 UI（血条、锁定点）
            if (enemyStatsManager.enemyHealthBar != null)
            {
                enemyStatsManager.enemyHealthBar.gameObject.SetActive(true);
                enemyStatsManager.enemyHealthBar.SetMaxHealth(enemyStatsManager.maxHealth);
            }
                
            if (lockOnTransform != null)
                lockOnTransform.gameObject.SetActive(true);

            // 重置血量与消融效果（Init 内部已含 dissolve reset）
            enemyStatsManager.Init(this);

            // 重新加载武器模型（OnRecycle 中已销毁，此处重建）
            enemyWeaponSlotManager.LoadBothWeaponsOnSlots();

            // 重新初始化状态机（会调用 initialState.OnEnter，重置 NavMesh 等 AI 状态）
            stateMachine.Initialize(initialState, this);
        }

        #endregion


    }
}
