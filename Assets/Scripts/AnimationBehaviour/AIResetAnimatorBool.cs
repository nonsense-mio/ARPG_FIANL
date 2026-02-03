using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HT
{
    public class AIResetAnimatorBool : ResetAnimatorBool
    {
        public string isPhaseShiftingBool = "isPhaseShifting";
        public bool isPhaseShiftingStatus = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool(isPhaseShiftingBool, isPhaseShiftingStatus);
        }
    }
}

