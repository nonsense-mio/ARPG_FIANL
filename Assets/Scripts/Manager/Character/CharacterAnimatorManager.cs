
using UnityEngine;
using UnityEngine.Animations.Rigging;
namespace ARPG
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        protected CharacterManager character;

        protected RigBuilder rigBuilder;
        public TwoBoneIKConstraint leftHandIK;
        public TwoBoneIKConstraint rightHandIK;
        bool handIKWeightsRest;

        public virtual void Init(CharacterManager characterMgr)
        {
            character = characterMgr;
            character = GetComponent<CharacterManager>();
            rigBuilder = GetComponent<RigBuilder>();
        }


        /// <summary>
        /// 播放指定动画
        /// </summary>
        /// <param name="targetAnim">目标动画</param>
        /// <param name="isInteracting">播放动画的bool值</param>
        public void PlayTargetAnimation(string targetAnim, bool isInteracting, bool canRotate = false, bool mirrorAnim = false,bool canRoll = false)
        {
            //应用根运动
            character.animator.applyRootMotion = isInteracting;
            character.animator.SetBool("canRotate", canRotate);
            character.animator.SetBool("isInteracting", isInteracting);
            character.animator.SetBool("isMirrored", mirrorAnim);
            character.animator.CrossFade(targetAnim, 0.2f);
            character.canRoll = canRoll;
        }

        public void PlayTargetAnimationWithRootRotation(string targetAnim, bool isInteracting)
        {

            character.animator.applyRootMotion = isInteracting;
            character.animator.SetBool("isRotatingWithRootMotion", true);
            character.animator.SetBool("isInteracting", isInteracting);
            character.animator.CrossFade(targetAnim, 0.2f);
        }

        public virtual void CanRotate()
        {
            character.animator.SetBool("canRotate", true);
        }

        public virtual void StopRotation()
        {
            character.animator.SetBool("canRotate", false);
        }
        /// <summary>
        /// 开启连击
        /// </summary>
        public virtual void EnableCombo()
        {
            character.animator.SetBool("canDoCombo", true);
        }
        /// <summary>
        /// 禁用连击
        /// </summary>
        public virtual void DisableCombo()
        {
            character.animator.SetBool("canDoCombo", false);
        }

        public virtual void EnablecanRoll()
        {
            character.canRoll = true;
        }

        /// <summary>
        /// 开启无敌
        /// </summary>
        public virtual void EnableIsInvulerable()
        {
            character.animator.SetBool("isInvulnerable", true);
        }

        /// <summary>
        /// 关闭无敌
        /// </summary>
        public virtual void DisableIsInvulerable()
        {
            character.animator.SetBool("isInvulnerable", false);
        }



        /// <summary>
        /// 开启格挡
        /// </summary>
        public virtual void EnableIsParrying()
        {
            character.isParrying = true;
        }
        /// <summary>
        /// 关闭格挡
        /// </summary>
        public virtual void DisableIsParrying()
        {
            character.isParrying = false;
        }
        /// <summary>
        /// 可以被反击
        /// </summary>
        public virtual void EnableCanBeRiposted()
        {
            character.canBeRiposted = true;
        }
        public virtual void DisableCanBeRiposted()
        {
            character.canBeRiposted = false;
        }

        /// <summary>
        /// 实现双手持武器时的手部对齐效果
        /// </summary>
        public virtual void SetHandIKForWeapon(RightHandIKTarget rightHandIKTarget, LeftHandIKTarget leftHandIKTarget, bool isTwoHandingWeapon)
        {
            //检查是否是双手持武器
            if (isTwoHandingWeapon)
            {
                if (rightHandIKTarget != null)
                {

                    rightHandIK.data.target = rightHandIKTarget.transform;
                    rightHandIK.data.targetPositionWeight = 1;
                    rightHandIK.data.targetRotationWeight = 1;
                }
                if (leftHandIKTarget != null)
                {
                    leftHandIK.data.target = leftHandIKTarget.transform;
                    leftHandIK.data.targetPositionWeight = 1;
                    leftHandIK.data.targetRotationWeight = 1;
                }
            }
            else
            {
                rightHandIK.data.target = null;
                leftHandIK.data.target = null;
            }
            rigBuilder.Build(); //重新构建Rig以应用更改
        }

        public virtual void CheckHandIKWeight(RightHandIKTarget rightHandIKTarget, LeftHandIKTarget leftHandIKTarget, bool isTwoHandingWeapon)
        {
            if (character.isInteracting)
                return;
            if (handIKWeightsRest)
            {
                handIKWeightsRest = false;
                if (rightHandIK.data.target != null)
                {
                    rightHandIK.data.target = rightHandIKTarget.transform;
                    rightHandIK.data.targetPositionWeight = 1;
                    rightHandIK.data.targetRotationWeight = 1;
                }
                if (leftHandIK.data.target != null)
                {
                    leftHandIK.data.target = leftHandIKTarget.transform;
                    leftHandIK.data.targetPositionWeight = 1;
                    leftHandIK.data.targetRotationWeight = 1;
                }
            }
        }


        public virtual void EraseHandIKForWeapon()
        {
            handIKWeightsRest = true;
            if (rightHandIK.data.target != null)
            {
                rightHandIK.data.targetPositionWeight = 0;
                rightHandIK.data.targetRotationWeight = 0;
            }
            if (leftHandIK.data.target != null)
            {
                leftHandIK.data.targetPositionWeight = 0;
                leftHandIK.data.targetRotationWeight = 0;
            }
        }

        /// <summary>
        /// 这是Unity的一个回调函数，在每帧处理动画移动时调用
        /// 把动画的根运动（位移和旋转）在播放交互动画时应用到角色上
        /// </summary>
        protected virtual void OnAnimatorMove()
        {   //当isInteracting为false时 是不应用根运动的直接返回
            if (character.isInteracting == false)
                return;
            //isInteracting为true时 应用根运动

            Vector3 velocity = character.animator.deltaPosition;
            character.characterController.Move(velocity);
            character.transform.rotation *= character.animator.deltaRotation;
        }

    }
}

