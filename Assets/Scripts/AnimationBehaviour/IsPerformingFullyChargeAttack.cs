using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class IsPerformingFullyChargeAttack : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("isPerformingFullyChargeAttack", true);
        }
    }

}
