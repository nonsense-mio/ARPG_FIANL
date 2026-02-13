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

        [Header("是否开启动画根运动")]
        public bool isInteracting;
        [Header("玩家状态")]
        public bool isDead;

        [Header("战斗相关bool")]
        public bool canBeRiposted;
        public bool canBeParried;
        public bool canDoCombo;
        public bool canRoll = true;
        public bool isParrying;
        public bool isBlocking;
        public bool isInvulnerable;
        public bool isUsingRightHand;
        public bool isUsingLeftHand;
        public bool isHoldingArrow;
        public bool isAiming;
        public bool isTwoHandingWeapon;
        public bool isPerformingFullyChargeAttack;
        public bool isAttacking;
        public bool isBeingBackstabbed;
        public bool isBeingRiposted;
        public bool isPerformingBackstab;
        public bool isPerformingRiposte;


        [Header("运动相关bool")]
        public bool isRotatingWithRootMotion;
        public bool canRotate;
        public bool isSprinting;
        public bool isGrounded;

        [Header("Spells")]
        public bool isFiringSpell;
        public bool isUsingComsumable;


        //
        //backstab  riposte 
        public int pendingCriticalDamage;
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

