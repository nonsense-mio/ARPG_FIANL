using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ARPG
{
    public class ResetAnimatorBool : StateMachineBehaviour
    {
        public string isInvulnerable = "isInvulnerable";
        public bool isInvulnerableStatus = false;

        public string isInteractingBool = "isInteracting";
        public bool isInteractingStatus = false;


        public string isFiringSpellBool = "isFiringSpell";
        public bool isFiringSpellStatus = false;


        public string isRotatingWithRootMotion = "isRotatingWithRootMotion";
        public bool isRotatingWithRootMotionStatus = false;


        public string canRotateBool = "canRotate";
        public bool canRotateStatus = true;

        public string isMirroredBool = "isMirrored";
        public bool isMirroredStatus = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CharacterManager character = animator.GetComponent<CharacterManager>();
            character.isUsingLeftHand = false;
            character.isUsingRightHand = false;
            character.isAttacking = false;
            character.isBeingBackstabbed = false;
            character.isBeingRiposted = false;
            character.isPerformingBackstab = false;
            character.isPerformingRiposte = false;
            character.canBeParried = false;
            character.canBeRiposted = false;
            character.isParrying = false;
            character.canRoll = true;
            character.isUsingComsumable = false;
            animator.SetBool(isInteractingBool, isInteractingStatus);
            animator.SetBool(isFiringSpellBool, isFiringSpellStatus);
            animator.SetBool(isRotatingWithRootMotion, isRotatingWithRootMotionStatus);
            animator.SetBool(canRotateBool, canRotateStatus);
            animator.SetBool(isInvulnerable, isInvulnerableStatus);
            animator.SetBool(isMirroredBool, isMirroredStatus);
        }
    }
}

