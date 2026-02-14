using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace ARPG
{
    public class GamePanel : BasePanel
    {
        private Slider sliderHealth;
        private Slider sliderStamina;
        private Slider sliderFocus;
        private Slider sliderPoisonBuildUp;
        private Slider sliderPoisonAmount;
        private Image imgConsumableSlot;
        private Image imgLeftSlot;
        private Image imgRightSlot;
        private Image imgSpellSlot;
        private Image imgAim;
        private Text txtSoulCount;
        private Text txtHealth;
        private Text txtStamina;
        private Text txtFocus;

        private readonly List<IUnRegister> unRegisters = new List<IUnRegister>();
        private IPlayerModel playerModel;
        private IInventoryModel inventoryModel;

        protected override void Awake()
        {
            base.Awake();
            sliderHealth = GetControl<Slider>("sliderHealth");
            sliderStamina = GetControl<Slider>("sliderStamina");
            sliderFocus = GetControl<Slider>("sliderFocus");
            sliderPoisonBuildUp = GetControl<Slider>("sliderPoisonBuildUp");
            sliderPoisonAmount = GetControl<Slider>("sliderPoisonAmount");
            imgConsumableSlot = GetControl<Image>("imgConsumableSlot");
            imgLeftSlot = GetControl<Image>("imgLeftSlot");
            imgRightSlot = GetControl<Image>("imgRightSlot");
            imgSpellSlot = GetControl<Image>("imgSpellSlot");
            imgAim = GetControl<Image>("imgAim");
            txtSoulCount = GetControl<Text>("txtSoulCount");
            txtHealth = GetControl<Text>("txtHealth");
            txtStamina = GetControl<Text>("txtStamina");
            txtFocus = GetControl<Text>("txtFocus");

            playerModel = this.GetModel<IPlayerModel>();
            inventoryModel = this.GetModel<IInventoryModel>();
        }

        void OnEnable()
        {

            #region 属性绑定
            // Health
            unRegisters.Add(playerModel.MaxHP.RegisterWithInitValue(v =>
            {
                SetMaxValue(sliderHealth, v);
                txtHealth.text = $"{playerModel.CurrentHP} / {v}";
            }));
            unRegisters.Add(playerModel.CurrentHP.RegisterWithInitValue(v =>
            {
                SetCurrentValue(sliderHealth, v);
                txtHealth.text = $"{v} / {playerModel.MaxHP}";
            }));

            // Stamina
            unRegisters.Add(playerModel.MaxStamina.RegisterWithInitValue(v =>
            {
                SetMaxValue(sliderStamina, v);
                txtStamina.text = $"{playerModel.CurrentStamina} / {v}";
            }));
            unRegisters.Add(playerModel.CurrentStamina.RegisterWithInitValue(v =>
            {
                SetCurrentValue(sliderStamina, v);
                txtStamina.text = $"{v} / {playerModel.MaxStamina}";
            }));

            // Focus
            unRegisters.Add(playerModel.MaxFocus.RegisterWithInitValue(v =>
            {
                SetMaxValue(sliderFocus, v);
                txtFocus.text = $"{playerModel.CurrentFocus} / {v}";
            }));
            unRegisters.Add(playerModel.CurrentFocus.RegisterWithInitValue(v =>
            {
                SetCurrentValue(sliderFocus, v);
                txtFocus.text = $"{v} / {playerModel.MaxFocus}";
            }));

            // Poison
            unRegisters.Add(playerModel.PoisonBuildUp.RegisterWithInitValue(v =>
            {
                SetMaxValue(sliderPoisonBuildUp, 100);
                SetCurrentValue(sliderPoisonBuildUp, v);
                SetActive(sliderPoisonBuildUp, v > 0);
            }));
            unRegisters.Add(playerModel.PoisonAmount.RegisterWithInitValue(v =>
            {
                SetMaxValue(sliderPoisonAmount, 100);
                SetCurrentValue(sliderPoisonAmount, v);
                SetActive(sliderPoisonAmount, v < 100);
            }));

            // Soul Count
            unRegisters.Add(playerModel.CurrentSoulCount.RegisterWithInitValue(v =>
            {
                txtSoulCount.text = v.ToString();
            }));
            #endregion

            #region 快速槽位绑定
            unRegisters.Add(inventoryModel.CurrentRightWeaponIndex.RegisterWithInitValue(idx =>
            {
                UpdateSlotIcon(imgRightSlot, inventoryModel.RightHandSlotIDs, idx);
            }));
            unRegisters.Add(inventoryModel.CurrentLeftWeaponIndex.RegisterWithInitValue(idx =>
            {
                UpdateSlotIcon(imgLeftSlot, inventoryModel.LeftHandSlotIDs, idx);
            }));
            unRegisters.Add(inventoryModel.CurrentConsumableIndex.RegisterWithInitValue(idx =>
            {
                UpdateSlotIcon(imgConsumableSlot, inventoryModel.ConsumableSlotIDs, idx);
            }));
            unRegisters.Add(inventoryModel.CurrentSpellIndex.RegisterWithInitValue(idx =>
            {
                UpdateSlotIcon(imgSpellSlot, inventoryModel.SpellSlotIDs, idx);
            }));
            #endregion

            // UI 行为事件
            unRegisters.Add(this.RegisterEvent<AimActionEvent>(e => AimActionUI(e.IsAiming)));
        }

        void OnDisable()
        {
            foreach (var ur in unRegisters) ur.UnRegister();
            unRegisters.Clear();
        }

        #region 快速槽位图标

        /// <summary>
        /// 通过 Model ID 查找 Item_SO 并更新图标
        /// </summary>
        private void UpdateSlotIcon(Image slot, BindableList<int> slotIDs, int index)
        {
            if (index < 0 || index >= slotIDs.Count)
            {
                slot.enabled = false;
                return;
            }

            int id = slotIDs[index];
            var item = ItemDataBase.Instance.GetItemByID(id);
            if (item != null && item.itemIcon != null)
            {
                slot.sprite = item.itemIcon;
                slot.enabled = true;
            }
            else
            {
                slot.sprite = null;
                slot.enabled = false;
            }
        }

        /// <summary>
        /// 兼容方法: 供 BagPanel 装备更换后手动刷新调用
        /// 后续 EquipPanel 重构后可移除
        /// </summary>
        public void UpdateAllQuickSlots()
        {
            var inv = this.GetModel<IInventoryModel>();
            UpdateSlotIcon(imgRightSlot, inv.RightHandSlotIDs, inv.CurrentRightWeaponIndex.Value);
            UpdateSlotIcon(imgLeftSlot, inv.LeftHandSlotIDs, inv.CurrentLeftWeaponIndex.Value);
            UpdateSlotIcon(imgConsumableSlot, inv.ConsumableSlotIDs, inv.CurrentConsumableIndex.Value);
            UpdateSlotIcon(imgSpellSlot, inv.SpellSlotIDs, inv.CurrentSpellIndex.Value);
        }

        #endregion

        private void AimActionUI(bool isAiming)
        {
            SetActive(imgAim, isAiming);
        }

        public override void HideMe()
        {
            this.SendEvent(new EnableInputEvent { Enabled = false });
        }

        public override void ShowMe()
        {
            this.SendEvent(new EnableInputEvent { Enabled = true });
        }
    }
}
