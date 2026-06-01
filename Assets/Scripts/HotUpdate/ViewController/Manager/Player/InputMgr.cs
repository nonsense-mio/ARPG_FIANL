using Framework;
using UnityEngine;

namespace ARPG
{
    public class InputMgr : ARPGController
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
            // 恢复输入前，清空所有缓存的输入状态
            ClearAllInputs();
            canProcessInput = enable;
        }

        /// <summary>
        /// 清空所有输入状态，防止恢复输入后执行残留输入
        /// </summary>
        public void ClearAllInputs()
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

        #region 武器动作分发

        /// <summary>
        /// 统一的武器动作分发 — 消除重复的单手/双手判断样板代码
        /// </summary>
        private void DispatchWeaponAction(
            System.Func<WeaponItem_SO, ItemAction_SO> ohSelector,
            System.Func<WeaponItem_SO, ItemAction_SO> thSelector,
            bool isRightHand = true)
        {
            var weapon = isRightHand
                ? player.playerInventoryManager.rightWeapon
                : player.playerInventoryManager.leftWeapon;

            var action = player.isTwoHandingWeapon
                ? thSelector(weapon)
                : ohSelector(weapon);

            if (action == null) return;

            player.UpdateWhichHandCharacterIsUsing(isRightHand);
            player.playerInventoryManager.currentItemBeingUsed = weapon;
            action.PerformAction(player);
        }

        #endregion

        #region 移动输入

        private void HandleMoveInput()
        {
            if (player.isHoldingArrow || walk_Input)
            {
                horizontal = movementInput.x;
                vertical = movementInput.y;
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
                horizontal = movementInput.x;
                vertical = movementInput.y;
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

        #endregion

        #region 战斗输入

        private void HandleTap_Mouse_L_Input()
        {
            if (!tap_Mouse_L_Input) return;
            tap_Mouse_L_Input = false;
            DispatchWeaponAction(w => w.oh_tap_Mouse_L_Action, w => w.th_tap_Mouse_L_Action);
        }

        private void HandleHold_Mouse_L_Input()
        {
            if (!hold_MouseL_Input) return;
            DispatchWeaponAction(w => w.oh_hold_Mouse_L_Action, w => w.th_hold_Mouse_L_Action);
        }

        private void HandleTap_Mouse_R_Input()
        {
            if (!tap_Mouse_R_Input) return;
            tap_Mouse_R_Input = false;
            DispatchWeaponAction(w => w.oh_tap_Mouse_R_Action, w => w.th_tap_Mouse_R_Action);
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
                DispatchWeaponAction(w => w.oh_hold_Mouse_R_Action, w => w.th_hold_Mouse_R_Action);
            }
            else
            {
                if (player.isAiming)
                {
                    player.isAiming = false;
                    this.SendEvent(new AimActionEvent { IsAiming = false });
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
            if (!tap_R_Input) return;
            tap_R_Input = false;
            DispatchWeaponAction(w => w.oh_tap_R_Action, w => w.th_tap_R_Action);
        }

        private void HandleHold_R_Input()
        {
            player.animator.SetBool("isChargingAttack", hold_R_Input);
            if (!hold_R_Input) return;
            DispatchWeaponAction(w => w.oh_hold_R_Action, w => w.th_hold_R_Action);
        }

        //Q代表左手的输入
        private void HandleTap_Q_Input()
        {
            if (!tap_Q_Input) return;
            tap_Q_Input = false;
            if (player.isTwoHandingWeapon)
                DispatchWeaponAction(w => w.oh_tap_Q_Action, w => w.th_tap_Q_Action, true);
            else
                DispatchWeaponAction(w => w.oh_tap_Q_Action, w => w.th_tap_Q_Action, false);
        }

        private void HandleHold_Q_Input()
        {
            if (!hold_Q_Input) return;
            if (player.isTwoHandingWeapon)
                DispatchWeaponAction(w => w.oh_hold_Q_Action, w => w.th_hold_Q_Action, true);
            else
                DispatchWeaponAction(w => w.oh_hold_Q_Action, w => w.th_hold_Q_Action, false);
        }

        #endregion

        #region 快捷输入 (Command 分发)

        private void HandleQuickSlotsInput()
        {
            if (d_Pad_Right)
            {
                d_Pad_Right = false;
                this.SendCommand(new ChangeQuickSlotCommand(E_QuickSlotType.RightWeapon));
            }
            else if (d_Pad_Left)
            {
                d_Pad_Left = false;
                this.SendCommand(new ChangeQuickSlotCommand(E_QuickSlotType.LeftWeapon));
            }
            else if (d_Pad_Up)
            {
                d_Pad_Up = false;
                this.SendCommand(new ChangeQuickSlotCommand(E_QuickSlotType.Consumable));
            }
            else if (d_Pad_Down)
            {
                d_Pad_Down = false;
                this.SendCommand(new ChangeQuickSlotCommand(E_QuickSlotType.Spell));
            }
        }

        private void HandleInventoryInput()
        {
            if (inventory_Input)
            {
                inventoryFlag = !inventoryFlag;
                this.SendCommand(new ToggleInventoryCommand(inventoryFlag));
            }
        }

        private void HandleTwoHandInput()
        {
            if (twoHand_Input)
            {
                twoHand_Input = false;
                twoHandFlag = !twoHandFlag;
                this.SendCommand(new ToggleTwoHandCommand(twoHandFlag));
            }
        }

        private void HandleUseComsumableInput()
        {
            if (x_Input)
            {
                x_Input = false;
                this.SendCommand(new UseConsumableCommand());
            }
        }

        #endregion

        #region 视角锁定

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

        #endregion

        #region 输入队列

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
                    ProcessQuedInput();
                }
                else
                {
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
                tap_Mouse_L_Input = true;
            if (qued_Mouse_R_Input)
                tap_Mouse_R_Input = true;
        }

        #endregion
    }
}
