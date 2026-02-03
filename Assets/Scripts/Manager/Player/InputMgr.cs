using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace HT
{
    public class InputMgr : MonoBehaviour
    {
        public bool canProcessInput = true;
        PlayerManager player;
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;
        //Roll按键
        public bool roll_Input;
        public bool walk_Input;
        public bool interact_Input;
        public bool twoHand_Input;
        //轻击按键输入
        public bool tap_Mouse_L_Input;
        public bool hold_MouseL_Input;
        public bool tap_Mouse_R_Input;
        public bool hold_mouseR_Input;
        //重击按键输入
        public bool tap_R_Input;
        public bool hold_R_Input;
        public bool tap_Q_Input;
        public bool hold_Q_Input;

        public bool jump_Input;
        public bool inventory_Input;
        public bool lockOn_Input;
        public bool right_Stick_Right_Input;
        public bool right_Stick_Left_Input;
        public bool x_Input;

        public bool d_Pad_Up;
        public bool d_Pad_Down;
        public bool d_Pad_Left;
        public bool d_Pad_Right;

        public bool rollFlag;
        public bool twoHandFlag;
        public bool comboFlag;
        public bool lockOnFlag;
        public bool fireFlag;
        public bool inventoryFlag;
        public float rollInputTimer;

        public bool input_Has_Been_Qued;
        public float current_Qued_Input_Timer;
        public float default_Qued_Input_Time;
        public bool qued_Mouse_L_Input;
        public bool qued_Mouse_R_Input;


        PlayerControls inputActions;



        Vector2 movementInput;
        Vector2 cameraInput;

        public void Init(PlayerManager playerManager)
        {
            player = playerManager;
        }
        public void EnableOrDisableInput(bool enable)
        {
            //if (enable)
            //{
                // 恢复输入前，清空所有缓存的输入状态
                ClearAllInputs();
            //}
            canProcessInput = enable;
        }

        /// <summary>
        /// 清空所有输入状态，防止恢复输入后执行残留输入
        /// </summary>
        private void ClearAllInputs()
        {
            // 清空移动输入
            movementInput = Vector2.zero;
            cameraInput = Vector2.zero;
            horizontal = 0;
            vertical = 0;
            moveAmount = 0;
            mouseX = 0;
            mouseY = 0;

            // 清空攻击输入
            tap_Mouse_L_Input = false;
            hold_MouseL_Input = false;
            tap_Mouse_R_Input = false;
            hold_mouseR_Input = false;
            tap_R_Input = false;
            hold_R_Input = false;
            tap_Q_Input = false;
            hold_Q_Input = false;

            // 清空其他按键输入
            roll_Input = false;
            walk_Input = false;
            interact_Input = false;
            twoHand_Input = false;
            jump_Input = false;
            inventory_Input = false;
            lockOn_Input = false;
            right_Stick_Right_Input = false;
            right_Stick_Left_Input = false;
            x_Input = false;

            // 清空方向键输入
            d_Pad_Up = false;
            d_Pad_Down = false;
            d_Pad_Left = false;
            d_Pad_Right = false;

            // 清空队列输入
            input_Has_Been_Qued = false;
            qued_Mouse_L_Input = false;
            qued_Mouse_R_Input = false;
            current_Qued_Input_Timer = 0;

            // 清空标志位
            rollFlag = false;
            rollInputTimer = 0;
        }
        private void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                inputActions.PlayerActions.RB.performed += i => tap_Mouse_L_Input = true;
                inputActions.PlayerActions.Hold_MouseL.performed += i => hold_MouseL_Input = true;
                inputActions.PlayerActions.Hold_MouseL.canceled += i => hold_MouseL_Input = false;
                inputActions.PlayerActions.RT.performed += i => tap_R_Input = true;
                inputActions.PlayerActions.Hold_RT.performed += i => hold_R_Input = true;
                inputActions.PlayerActions.Hold_RT.canceled += i => hold_R_Input = false;
                inputActions.PlayerActions.Hold_Q.performed += i => hold_Q_Input = true;
                inputActions.PlayerActions.Hold_Q.canceled += i => hold_Q_Input = false;

                inputActions.PlayerActions.Parry.performed += i => tap_Q_Input = true;

                inputActions.PlayerQuickSlots.DPadRight.performed += i => d_Pad_Right = true;
                inputActions.PlayerQuickSlots.DPadLeft.performed += i => d_Pad_Left = true;
                inputActions.PlayerQuickSlots.DPadUp.performed += i => d_Pad_Up = true;
                inputActions.PlayerQuickSlots.DPadDown.performed += i => d_Pad_Down = true;
                inputActions.PlayerActions.Interact.performed += i => interact_Input = true;
                inputActions.PlayerActions.Mouse_R.performed += i => hold_mouseR_Input = true;
                inputActions.PlayerActions.Mouse_R.canceled += i => hold_mouseR_Input = false;
                inputActions.PlayerActions.Tap_Mouse_R.performed += i => tap_Mouse_R_Input = true;
                inputActions.PlayerActions.Roll.performed += i => roll_Input = true;
                inputActions.PlayerActions.Roll.canceled += i => roll_Input = false;
                inputActions.PlayerActions.Walk.performed += i => walk_Input = true;
                inputActions.PlayerActions.Walk.canceled += i => walk_Input = false;
                inputActions.PlayerActions.Jump.performed += i => jump_Input = true;
                inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;
                inputActions.PlayerActions.LockOn.performed += i => lockOn_Input = true;

                inputActions.PlayerMovement.LockOnTargetRight.performed += i => right_Stick_Right_Input = true;
                inputActions.PlayerMovement.LockOnTargetLeft.performed += i => right_Stick_Left_Input = true;
                inputActions.PlayerActions.Y.performed += i => twoHand_Input = true;
                inputActions.PlayerActions.X.performed += i => x_Input = true;

                inputActions.PlayerActions.Qued_MouseL.performed += i => QueInput(ref qued_Mouse_L_Input);
                inputActions.PlayerActions.Qued_MouseR.performed += i => QueInput(ref qued_Mouse_R_Input);

            }
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput()
        {
            HandleInventoryInput();
            if (player.isDead || !canProcessInput)
            {
                return;
            }
                
            HandleMoveInput();
            HandleRollInput();

            HandleTap_Mouse_L_Input();
            HandleTap_Mouse_R_Input();
            HandleTap_R_Input();
            HandleTap_Q_Input();

            HandleHold_Mouse_R_Input();
            HandleHold_Mouse_L_Input();
            HandleHold_R_Input();
            HandleHold_Q_Input();

            HandleQuickSlotsInput();
            HandleLockOnInput();
            HandleTwoHandInput();
            HandleUseComsumableInput();

            HandleQuedInput();
        }

        private void HandleMoveInput()
        {
            if (player.isHoldingArrow || walk_Input)
            {
                //水平方向位移偏量
                horizontal = movementInput.x;
                //前后位移偏量
                vertical = movementInput.y;
                //位移偏量
                moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
                if (moveAmount > 0.5f)
                {
                    moveAmount = 0.5f;
                }
                mouseX = cameraInput.x;
                mouseY = cameraInput.y;
            }
            else
            {
                //水平方向位移偏量
                horizontal = movementInput.x;
                //前后位移偏量
                vertical = movementInput.y;
                //位移偏量
                moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
                mouseX = cameraInput.x;
                mouseY = cameraInput.y;
            }

        }

        private void HandleRollInput()
        {
            if (roll_Input)
            {
                rollInputTimer += Time.deltaTime;
                if (player.playerStatsManager.currentStamina <= 0)
                {
                    roll_Input = false;
                    player.isSprinting = false;
                }
                if (moveAmount > 0.5f && player.playerStatsManager.currentStamina > 0)
                {
                    player.isSprinting = true;
                }
            }
            else
            {
                player.isSprinting = false;
                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    rollFlag = true;
                }
                rollInputTimer = 0;
            }

        }
        /// <summary>
        /// 攻击输入相关
        /// </summary>
        /// <param name="delta"></param>
        private void HandleTap_Mouse_L_Input()
        {
            if (tap_Mouse_L_Input) //rb
            {
                tap_Mouse_L_Input = false;
                if (player.isTwoHandingWeapon)
                {
                    if (player.playerInventoryManager.rightWeapon.th_tap_Mouse_L_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.th_tap_Mouse_L_Action.PerformAction(player);
                }
                else
                {
                    if (player.playerInventoryManager.rightWeapon.oh_tap_Mouse_L_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.oh_tap_Mouse_L_Action.PerformAction(player);
                }

            }
        }

        private void HandleHold_Mouse_L_Input()
        {
            if (hold_MouseL_Input)
            {
                if (player.isTwoHandingWeapon)
                {
                    if (player.playerInventoryManager.rightWeapon.th_hold_Mouse_L_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.th_hold_Mouse_L_Action.PerformAction(player);
                }
                else
                {
                    if (player.playerInventoryManager.rightWeapon.oh_hold_Mouse_L_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.oh_hold_Mouse_L_Action.PerformAction(player);
                }



            }
        }

        private void HandleTap_Mouse_R_Input()
        {
            if (tap_Mouse_R_Input)//lb
            {
                tap_Mouse_R_Input = false;
                if (player.isTwoHandingWeapon)
                {
                    if (player.playerInventoryManager.rightWeapon.th_tap_Mouse_R_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.th_tap_Mouse_R_Action.PerformAction(player);
                }
                else
                {
                    if (player.playerInventoryManager.rightWeapon.oh_tap_Mouse_R_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.oh_tap_Mouse_R_Action.PerformAction(player);
                }
            }
        }
        private void HandleHold_Mouse_R_Input()
        {
            if (!player.isGrounded || player.isSprinting || player.isFiringSpell)
            {
                hold_mouseR_Input = false;
                return;
            }
            if (hold_mouseR_Input)
            {
                if (player.isTwoHandingWeapon)
                {
                    if (player.playerInventoryManager.rightWeapon.th_hold_Mouse_R_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.th_hold_Mouse_R_Action.PerformAction(player);
                }
                else
                {
                    if (player.playerInventoryManager.rightWeapon.oh_hold_Mouse_R_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.oh_hold_Mouse_R_Action.PerformAction(player);
                }
            }
            else
            {
                if (player.isAiming)
                {
                    player.isAiming = false;
                    //player.uiManager.crossHair.SetActive(false);
                    EventCenter.Instance.EventTrigger<bool>(E_EventType.E_AimAction, false);
                    //重置摄像机的旋转
                    player.cameraMgr.ResetAimCameraRotation();
                }
                if (player.isBlocking)
                {
                    player.isBlocking = false;
                }
            }
        }


        private void HandleTap_R_Input()
        {
            if (tap_R_Input)//rt
            {
                tap_R_Input = false;
                if (player.isTwoHandingWeapon)
                {
                    if (player.playerInventoryManager.rightWeapon.th_tap_R_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.th_tap_R_Action.PerformAction(player);
                }
                else
                {
                    if (player.playerInventoryManager.rightWeapon.oh_tap_R_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.oh_tap_R_Action.PerformAction(player);
                }

            }
        }

        private void HandleHold_R_Input()
        {
            player.animator.SetBool("isChargingAttack", hold_R_Input);
            if (hold_R_Input)//rt
            {

                if (player.isTwoHandingWeapon)
                {
                    if (player.playerInventoryManager.rightWeapon.th_hold_R_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.th_hold_R_Action.PerformAction(player);
                }
                else
                {
                    if (player.playerInventoryManager.rightWeapon.oh_hold_R_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.oh_hold_R_Action.PerformAction(player);
                }

            }
        }

        //Q代表左手的输入
        private void HandleTap_Q_Input()
        {
            if (tap_Q_Input)//lt
            {
                tap_Q_Input = false;
                if (player.isTwoHandingWeapon)
                {
                    if (player.playerInventoryManager.rightWeapon.th_tap_Q_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.th_tap_Q_Action.PerformAction(player);
                }
                else
                {
                    if (player.playerInventoryManager.leftWeapon.oh_tap_Q_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(false);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.leftWeapon;
                    player.playerInventoryManager.leftWeapon.oh_tap_Q_Action.PerformAction(player);
                }
            }

        }
        private void HandleHold_Q_Input()
        {
            if (hold_Q_Input)//lt
            {
                if (player.isTwoHandingWeapon)
                {
                    if (player.playerInventoryManager.rightWeapon.th_hold_Q_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.th_hold_Q_Action.PerformAction(player);
                }
                else
                {
                    if (player.playerInventoryManager.leftWeapon.oh_hold_Q_Action == null)
                        return;
                    player.UpdateWhichHandCharacterIsUsing(false);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.leftWeapon;
                    player.playerInventoryManager.leftWeapon.oh_hold_Q_Action.PerformAction(player);
                }
            }
        }


        /// <summary>
        /// 切换快速插槽的输入相关
        /// </summary>
        private void HandleQuickSlotsInput()
        {
            if (d_Pad_Right)
            {
                d_Pad_Right = false;
                player.playerInventoryManager.ChangeRightWeapon();
                player.playerWeaponSlotManager.LoadBothWeaponsOnSlots();
                EventCenter.Instance.EventTrigger(E_EventType.E_ChangeRightWeapon);
            }
            else if (d_Pad_Left)
            {
                d_Pad_Left = false;
                player.playerInventoryManager.ChangeLeftWeapon();
                player.playerWeaponSlotManager.LoadBothWeaponsOnSlots();
                EventCenter.Instance.EventTrigger(E_EventType.E_ChangeLeftWeapon);
            }
            else if (d_Pad_Up)
            {
                d_Pad_Up = false;
                player.playerInventoryManager.ChangeConsumable();
                EventCenter.Instance.EventTrigger(E_EventType.E_ChangeConsumable);

            }
            else if (d_Pad_Down)
            {
                d_Pad_Down = false;
                player.playerInventoryManager.ChangeSpell();
                print("切换法术");
                EventCenter.Instance.EventTrigger(E_EventType.E_ChangeSpell);
            }

        }
        private void HandleInventoryInput()
        {
            if (inventory_Input)
            {
                inventoryFlag = !inventoryFlag;
                if (inventoryFlag)
                {
                    EventCenter.Instance.EventTrigger<bool>(E_EventType.E_OpenOrCloseSelectWindow, true);
                    UIMgr.Instance.HidePanel<GamePanel>();
                }
                else
                {
                    EventCenter.Instance.EventTrigger<bool>(E_EventType.E_OpenOrCloseSelectWindow, false);
                    UIMgr.Instance.HidePanel<EquipPanel>();
                    UIMgr.Instance.HidePanel<BagPanel>();
                    UIMgr.Instance.HidePanel<LevelUpPanel>();
                    UIMgr.Instance.HidePanel<TaskPanel>();
                    UIMgr.Instance.ShowPanel<GamePanel>();

                }
            }
        }

        private void HandleLockOnInput()
        {
            //当按下锁定视角键 且 当前没有锁定视角时
            if (lockOn_Input && !lockOnFlag)
            {
                lockOn_Input = false;
                player.cameraMgr.HandleLockOn();
                //当最近的目标不为空时
                if (player.cameraMgr.nearestLockOnTarget != null)
                {   //设置其为当前锁定对象
                    player.cameraMgr.currentLockOnTarget = player.cameraMgr.nearestLockOnTarget;
                    //锁定视角标志位设为true
                    lockOnFlag = true;
                }

            }
            //取消视角锁定
            else if (lockOn_Input && lockOnFlag)
            {
                lockOn_Input = false;
                lockOnFlag = false;
                player.cameraMgr.ClearLockOnTargets();
            }

            if (lockOnFlag && right_Stick_Left_Input)
            {
                right_Stick_Left_Input = false;
                player.cameraMgr.HandleLockOn();
                if (player.cameraMgr.leftLockTarget != null)
                {
                    player.cameraMgr.currentLockOnTarget = player.cameraMgr.leftLockTarget;
                }
            }
            if (lockOnFlag && right_Stick_Right_Input)
            {
                right_Stick_Right_Input = false;
                player.cameraMgr.HandleLockOn();
                if (player.cameraMgr.rightLockTarget != null)
                {
                    player.cameraMgr.currentLockOnTarget = player.cameraMgr.rightLockTarget;
                }
            }
            player.cameraMgr.SetCameraHeight();
        }

        private void HandleTwoHandInput()
        {
            if (twoHand_Input)
            {
                twoHand_Input = false;
                twoHandFlag = !twoHandFlag;
                if (twoHandFlag)
                {
                    player.isTwoHandingWeapon = true;
                    //双手模式
                    player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.rightWeapon, false);

                    player.playerWeaponSlotManager.LoadTwoHandIKTargets(true);
                }
                else
                {
                    player.isTwoHandingWeapon = false;
                    player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.rightWeapon, false);
                    player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.leftWeapon, true);
                    player.playerWeaponSlotManager.LoadTwoHandIKTargets(false);

                }
            }
        }


        private void HandleUseComsumableInput()
        {
            if (x_Input)
            {
                x_Input = false;
                //使用消耗品
                player.playerInventoryManager.currentConsumable.AttemptToConsumeItem(player);
            }
        }

        private void QueInput(ref bool quedInput)
        {

            //如果角色正在交互 则将输入放入队列
            if (player.isInteracting)
            {
                quedInput = true;
                current_Qued_Input_Timer = default_Qued_Input_Time;
                input_Has_Been_Qued = true;
            }

        }

        private void HandleQuedInput()
        {
            if (input_Has_Been_Qued)
            {
                if (current_Qued_Input_Timer > 0)
                {
                    current_Qued_Input_Timer -= Time.deltaTime;
                    //执行队列输入
                    ProcessQuedInput();
                }
                else
                {
                    //时间到 清空队列输入
                    input_Has_Been_Qued = false;
                    qued_Mouse_L_Input = false;
                    qued_Mouse_R_Input = false;
                    current_Qued_Input_Timer = 0;
                }
            }
        }

        private void ProcessQuedInput()
        {

            if (qued_Mouse_L_Input)
            {
                tap_Mouse_L_Input = true;
            }
            if (qued_Mouse_R_Input)
            {
                tap_Mouse_R_Input = true;
            }

        }
    }
}

