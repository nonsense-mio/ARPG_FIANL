using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ARPG
{
    public class CharacterManager : ARPGController
    {
        public Animator animator;
        public CharacterController characterController;
        public CharacterStatsManager characterStatsManager;
        public CharacterCombatManager characterCombatManager;
        public CharacterEffectsManager characterEffectsManager;
        public CharacterAnimatorManager characterAnimatorManager;
        public CharacterInventoryManager characterInventoryManager;
        public CharacterWeaponSlotManager characterWeaponSlotManager;
        public CharacterLocomotionManager characterLocomotionManager;

        public Transform lockOnTransform;
        public Transform criticalAttackRayCastStartPoint;

        // ── Animator 拥有（每帧从 Animator 读取，C# 不应直接写入） ──
        [Header("Animator 拥有（每帧读取）")]
        public bool isInteracting;
        public bool canDoCombo;
        public bool canRotate;
        public bool isInvulnerable;
        public bool isFiringSpell;
        public bool isHoldingArrow;
        public bool isPerformingFullyChargeAttack;

        // ── 代码拥有（每帧写入 Animator，Animator 用于转换条件） ──
        [Header("代码拥有（每帧写入 Animator）")]
        public bool isTwoHandingWeapon;
        public bool isBlocking;
        public bool isDead;
        public bool isGrounded;

        // ── 纯代码标志（不与 Animator 同步，由 ResetAnimatorBool 或逻辑重置） ──
        [Header("战斗标志（纯代码）")]
        public bool isAttacking;
        public bool isParrying;
        public bool canBeParried;
        public bool canBeRiposted;
        public bool isBeingBackstabbed;
        public bool isBeingRiposted;
        public bool isPerformingBackstab;
        public bool isPerformingRiposte;
        public bool isUsingRightHand;
        public bool isUsingLeftHand;
        public bool isAiming;
        public bool canRoll = true;
        public bool isUsingComsumable;

        [Header("运动标志（纯代码）")]
        public bool isRotatingWithRootMotion;
        public bool isSprinting;


        //backstab  riposte
        public int pendingCriticalDamage;

        /// <summary>
        /// 通用动作守卫：未死亡、未处于交互动画中、未使用消耗品、未在释放法术
        /// </summary>
        public bool CanPerformAction()
        {
            return !isDead && !isInteracting && !isUsingComsumable && !isFiringSpell;
        }
        protected virtual void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterInventoryManager = GetComponent<CharacterInventoryManager>();
            characterWeaponSlotManager = GetComponent<CharacterWeaponSlotManager>();
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();

            characterStatsManager.Init(this);
            characterCombatManager.Init(this);
            characterEffectsManager.Init(this);
            characterAnimatorManager.Init(this);
            characterInventoryManager.Init(this);
            characterWeaponSlotManager.Init(this);
            characterLocomotionManager.Init(this);
        }
        protected virtual void FixedUpdate()
        {
            characterAnimatorManager.CheckHandIKWeight(characterWeaponSlotManager.rightHandIKTarget, characterWeaponSlotManager.leftHandIKTarget, isTwoHandingWeapon);
        }

        public virtual void UpdateWhichHandCharacterIsUsing(bool usingRightHand)
        {
            if (usingRightHand)
            {
                isUsingRightHand = true;
                isUsingLeftHand = false;
            }
            else
            {
                isUsingRightHand = false;
                isUsingLeftHand = true;
            }
        }
    }
}

