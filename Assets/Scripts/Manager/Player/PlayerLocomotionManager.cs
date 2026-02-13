using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {

        PlayerManager player;
        public GameObject normalCamera;


        [Header("Movement Stats")]
        [SerializeField]
        private float movementSpeed = 5;
        [SerializeField]
        private float walkingSpeed = 3;
        [SerializeField]
        private float sprintSpeed = 7;
        [SerializeField]
        private float rotationSpeed = 10;

        [Header("Stamina Costs")]
        [SerializeField]
        int rollStaminaCost = 15;
        [SerializeField]
        int backStepStaminaCost = 10;
        [SerializeField]
        int sprintStaminaCost = 1;

        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            player = characterMgr as PlayerManager;
        }

        #region Movement
        /// <summary>
        /// 处理人物的旋转方法
        /// </summary>
        /// <param name="delta"></param>
        public void HandleRotation()
        {
            if (player.canRotate)
            {
                if (player.isAiming)
                {
                    Quaternion targetRotation = Quaternion.Euler(0, player.cameraMgr.cameraTransform.eulerAngles.y, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
                else
                {

                    if (player.inputMgr.lockOnFlag)
                    {
                        if (player.isSprinting || player.inputMgr.rollFlag)
                        {
                            Vector3 targetDirection = Vector3.zero;
                            targetDirection = player.cameraMgr.cameraTransform.forward * player.inputMgr.vertical;
                            targetDirection += player.cameraMgr.cameraTransform.right * player.inputMgr.horizontal;

                            targetDirection.Normalize();
                            targetDirection.y = 0;
                            if (targetDirection == Vector3.zero)
                                targetDirection = transform.forward;
                            Quaternion tr = Quaternion.LookRotation(targetDirection);
                            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                            transform.rotation = targetRotation;
                        }
                        else
                        {
                            Vector3 rotationDirection = moveDirection;
                            rotationDirection = player.cameraMgr.currentLockOnTarget.transform.position - transform.position;
                            rotationDirection.y = 0;
                            rotationDirection.Normalize();
                            Quaternion tr = Quaternion.LookRotation(rotationDirection);
                            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                            transform.rotation = targetRotation;
                        }
                    }
                    else
                    {
                        Vector3 targetDir = Vector3.zero;
                        float moveOverride = player.inputMgr.moveAmount;
                        //根据输入确定 目标方向
                        targetDir =  player.cameraMgr.cameraTransform.forward * player.inputMgr.vertical;
                        targetDir += player.cameraMgr.cameraTransform.right * player.inputMgr.horizontal;

                        targetDir.Normalize();
                        //将垂直方向置0
                        targetDir.y = 0;
                        //目标方向为0代表没有位移上的输入 目标方向即为角色本身
                        if (targetDir == Vector3.zero)
                            targetDir = player.transform.forward;

                        //float rs = rotationSpeed;
                        //将角色 旋转与目标方向一致 即角色会朝向移动的方向旋转
                        //记录旋转量
                        Quaternion tr = Quaternion.LookRotation(targetDir);
                        //插值运算 平滑人物的旋转
                        Quaternion targetRotation = Quaternion.Slerp(player.transform.rotation, tr, rotationSpeed * Time.deltaTime);
                        player.transform.rotation = targetRotation;
                    }
                }

            }


        }
        /// <summary>
        /// 人物移动方法
        /// </summary>
        /// <param name="delta"></param>
        public void HandleGroundedMovement()
        {
            if (player.inputMgr.rollFlag)
                return;
            if (player.isInteracting)
                return;

            if(!player.isGrounded)
                return;

            moveDirection = player.cameraMgr.transform.forward * player.inputMgr.vertical;
            moveDirection += player.cameraMgr.transform.right * player.inputMgr.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            //移动速度的应用
            if (player.isSprinting && player.inputMgr.moveAmount > 0.5f)
            {
                player.characterController.Move(moveDirection * sprintSpeed * Time.deltaTime);
                player.playerStatsManager.DeductSprintingStamina(sprintStaminaCost);
            }
            else 
            {
                if(player.inputMgr.moveAmount > 0.5f)
                {
                    player.characterController.Move(moveDirection * movementSpeed * Time.deltaTime);
                }
                else if(player.inputMgr.moveAmount <= 0.5f)
                {
                    player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
            }

            //当视角锁定且没在冲刺时
            if (player.inputMgr.lockOnFlag && !player.isSprinting)
            {
                player.playerAnimatorManager.UpdateAnimatorValues(player.inputMgr.vertical, player.inputMgr.horizontal, player.isSprinting);
            }
            else
            {
                //动画的应用
                player.playerAnimatorManager.UpdateAnimatorValues(player.inputMgr.moveAmount, 0, player.isSprinting);
            }

        }

        public void HandleRollingAndSprinting()
        {
            //检查是否还有体力 没有就返回
            if (player.playerStatsManager.currentStamina <= 0)
                return;
            if (player.inputMgr.rollFlag)
            {
                player.inputMgr.rollFlag = false;
                if(!player.canRoll)
                    return;
                moveDirection =  player.cameraMgr.cameraTransform.forward * player.inputMgr.vertical;
                moveDirection += player.cameraMgr.cameraTransform.right * player.inputMgr.horizontal;

                if (player.inputMgr.moveAmount > 0)
                {
                    player.playerAnimatorManager.PlayTargetAnimation("Rolling", true);
                    player.playerAnimatorManager.EraseHandIKForWeapon();
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    player.transform.rotation = rollRotation;
                    player.playerStatsManager.DeductStamina(rollStaminaCost);
                }
                else
                {
                    player.playerAnimatorManager.PlayTargetAnimation("Backstep", true);
                    player.playerAnimatorManager.EraseHandIKForWeapon();
                    player.playerStatsManager.DeductStamina(backStepStaminaCost);
                }
            }
        }


        public void HandleJumping()
        {
            if (player.isInteracting)
                return;
            if (player.playerStatsManager.currentStamina <= 0)
                return;
            //如果跳跃键触发
            if (player.inputMgr.jump_Input)
            {
                player.inputMgr.jump_Input = false;
                //如果存在移动输入
                if (player.inputMgr.moveAmount > 0)
                {
                    //根据移动输入确定移动方向
                    moveDirection = player.cameraMgr.cameraTransform.forward * player.inputMgr.vertical;
                    moveDirection += player.cameraMgr.cameraTransform.right * player.inputMgr.horizontal;
                    player.playerAnimatorManager.PlayTargetAnimation("Jump", true);
                    player.playerAnimatorManager.EraseHandIKForWeapon();
                    moveDirection.y = 0;
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    player.transform.rotation = jumpRotation;
                }

            }
        }
        #endregion

    }

}
