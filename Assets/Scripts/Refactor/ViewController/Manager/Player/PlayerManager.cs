using Framework;
using UnityEngine;


namespace ARPG
{
    public class PlayerManager : CharacterManager
    {
        public static PlayerManager localPlayer;

        public InputMgr inputMgr;
        public CameraMgr cameraMgr;
        public Collider playerCollider;
        public Collider characterCollider;
        public Transform targetTransformWhileAiming;
        public PlayerEvents playerEvents;
        public PlayerSaveManager playerSaveManager;
        public PlayerStatsManager playerStatsManager;
        public PlayerCombatManager playerCombatManager;
        public PlayerEffectsManager playerEffectsManager;
        public PlayerAnimatorManager playerAnimatorManager;
        public PlayerEquipmentManager playerEquipmentManager;
        public PlayerInventoryManager playerInventoryManager;
        public PlayerWeaponSlotManager playerWeaponSlotManager;

        public PlayerLocomotionManager playerLocomotionManager;

        //检查是否发生过交互
        private bool isShowingInteractInfo;

        // isInteracting 安全阀：防止因无效动画名导致 isInteracting 永久卡 true
        private static readonly int _hashIsInteracting = Animator.StringToHash("isInteracting");
        private const float MAX_INTERACTING_DURATION = 5f;
        private float _isInteractingTimer;

        protected override void Awake()
        {
            base.Awake();
            if (localPlayer == null)
            {
                localPlayer = this;
            }
            else
            {
                Debug.LogWarning("There is more than one PlayerManager in the scene. There should only be one PlayerManager.");
            }
            inputMgr = GetComponent<InputMgr>();
            cameraMgr = FindObjectOfType<CameraMgr>();
            playerEvents = GetComponent<PlayerEvents>();
            playerSaveManager = GetComponent<PlayerSaveManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();

            inputMgr.Init(this);
            playerEvents.Init(this);
            playerSaveManager.Init(this);
            playerStatsManager.Init(this);
            playerCombatManager.Init(this);
            playerEffectsManager.Init(this);
            playerAnimatorManager.Init(this);
            playerEquipmentManager.Init(this);
            playerInventoryManager.Init(this);
            playerWeaponSlotManager.Init(this);
            playerLocomotionManager.Init(this);

            playerCollider = GetComponent<Collider>();

        }
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        // Update is called once per frame
        void Update()
        {   //获取isIneracting的状态
            isInteracting = animator.GetBool("isInteracting");
            canDoCombo = animator.GetBool("canDoCombo");
            canRotate = animator.GetBool("canRotate");
            isInvulnerable = animator.GetBool("isInvulnerable");
            isFiringSpell = animator.GetBool("isFiringSpell");
            isHoldingArrow = animator.GetBool("isHoldingArrow");
            isPerformingFullyChargeAttack = animator.GetBool("isPerformingFullyChargeAttack");
            animator.SetBool("isTwoHandingWeapon", isTwoHandingWeapon);
            animator.SetBool("isBlocking", isBlocking);
            animator.SetBool("isDead", isDead);
            animator.SetBool("isGrounded", isGrounded);

            inputMgr.TickInput();


            playerLocomotionManager.HandleRollingAndSprinting();
            playerLocomotionManager.HandleJumping();
            playerStatsManager.RegenerateStamina();
            playerEffectsManager.HandleAllEffects();
            playerLocomotionManager.HandleGroundedMovement();
            playerLocomotionManager.HandleRotation();


            HandleIsInteractingSafetyValve();

            //检查可交互的物品
            CheckForInteractableObject();
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
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

        }
        private void LateUpdate()
        {
            inputMgr.interact_Input = false;
            inputMgr.inventory_Input = false;

            if (cameraMgr != null)
            {
                cameraMgr.FollowTarget();
                cameraMgr.HandleCameraRotation();
            }

        }


        #region 玩家交互检测

        /// <summary>
        /// 检查可交互的物体
        /// </summary>
        public void CheckForInteractableObject()
        {
            RaycastHit hit;
            //射线检测是否有可交互的物品
            if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, 1f, cameraMgr.ignoreLayers))
            {
                if (hit.collider.tag == "Interactable")
                {
                    Interactable interactObj = hit.collider.GetComponent<Interactable>();
                    if (interactObj != null)
                    {
                        //当交互键按下时 执行物品的Interact逻辑
                        //发布UI交互事件 UI面板订阅该事件 显示交互提示
                        this.SendEvent(new InteractPromptEvent { Target = interactObj });
                        isShowingInteractInfo = true;
                        if (inputMgr.interact_Input)
                        {
                            hit.collider.GetComponent<Interactable>().Interact(this);
                        }
                    }
                }
            }
            //当没有检测到交互物品时
            else
            {
                //隐藏交互UI信息
                if (isShowingInteractInfo)
                {
                    isShowingInteractInfo = false;
                    //发布UI交互事件 UI面板订阅该事件 隐藏交互提示
                    this.SendEvent(new InteractPromptEvent { Target = null });
                }

            }
        }

        public void OpenChestInteraction(Transform playerStandHereWhenOpeningChest)
        {
            transform.position = playerStandHereWhenOpeningChest.position;
            playerAnimatorManager.PlayTargetAnimation("Open Chest", true);
        }
        /// <summary>
        /// 通过迷雾墙
        /// </summary>
        /// <param name="fogWallEntrance"></param>
        public void PassThroughFogWallInteraction(Transform fogWallEntrance)
        {
            Vector3 rotationDirection = fogWallEntrance.transform.forward;
            transform.rotation = Quaternion.LookRotation(rotationDirection);
            playerAnimatorManager.PlayTargetAnimation("Pass Through Fog", true);
        }

        #endregion

        //设置玩家位置
        public void SetPlayerPosition(Vector3 targetPos)
        {
            // 检查位置是否有效
            if (targetPos == Vector3.zero)
            {
                return;
            }

            CharacterController cc = GetComponent<CharacterController>();

            if (cc != null)
            {
                // 禁用 CharacterController 才能正确设置位置
                cc.enabled = false;
                transform.position = targetPos;
                cc.enabled = true;
            }
            else
            {
                transform.position = targetPos;
            }

        }

    }
}

