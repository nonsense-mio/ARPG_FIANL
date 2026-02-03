using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT
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

        // 防止重复初始化的标记
        private bool hasInitializedUI;
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
        }
        //监听玩家属性变化事件 更新UI显示
        void OnEnable()
        {
            EventCenter.Instance.AddEventListener<bool>(E_EventType.E_AimAction, AimActionUI);
            EventCenter.Instance.AddEventListener(E_EventType.E_ChangeSpell, UpdateSpellSlot);
            EventCenter.Instance.AddEventListener(E_EventType.E_Player_Init_UI, InitializePlayerUI);
            EventCenter.Instance.AddEventListener(E_EventType.E_ChangeLeftWeapon, UpdateWeaponLeftSlot);
            EventCenter.Instance.AddEventListener(E_EventType.E_ChangeConsumable, UpdateConsumableSlot);
            EventCenter.Instance.AddEventListener(E_EventType.E_ChangeRightWeapon, UpdateWeaponRightSlot);
            EventCenter.Instance.AddEventListener(E_EventType.E_Player_Update_SoulCount_UI, UpdateSoulCountUI);
            EventCenter.Instance.AddEventListener<StatChanged>(E_EventType.E_Player_StatChanged, OnPlayerStatChanged);

        }
        //取消监听
        void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.E_ChangeSpell, UpdateSpellSlot);
            EventCenter.Instance.RemoveEventListener<bool>(E_EventType.E_AimAction, AimActionUI);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Player_Init_UI, InitializePlayerUI);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_ChangeLeftWeapon, UpdateWeaponLeftSlot);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_ChangeConsumable, UpdateConsumableSlot);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_ChangeRightWeapon, UpdateWeaponRightSlot);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Player_Update_SoulCount_UI, UpdateSoulCountUI);
            EventCenter.Instance.RemoveEventListener<StatChanged>(E_EventType.E_Player_StatChanged, OnPlayerStatChanged);
        }
        private void EnsureFacade()
        {
            if (playerFacade == null)
                Debug.LogError($"{nameof(EquipPanel)} 未绑定 playerFacade，请在 ShowPanel/GetPanel 回调里调用 panel.Bind(facade)");
        }

        public void UpdateQuickSlotUI(Image imageSlot, Item_SO item)
        {
            if (item.itemIcon != null)
            {
                imageSlot.sprite = item.itemIcon;
                imageSlot.enabled = true;
            }
            else
            {
                imageSlot.sprite = null;
                imageSlot.enabled = false;
            }
        }
        public void SetSoulCountText(int soulCount)
        {
            txtSoulCount.text = soulCount.ToString();
        }
        #region 处理玩家属性变化事件
        private void OnPlayerStatChanged(StatChanged stat)
        {
            switch (stat.stat)
            {
                case E_PlayerStatType.Health:
                    SetMaxValue(sliderHealth, stat.max);
                    SetCurrentValue(sliderHealth, stat.cur);
                    txtHealth.text = stat.cur.ToString() + " / " + stat.max.ToString();
                    break;

                case E_PlayerStatType.Stamina:
                    SetMaxValue(sliderStamina, stat.max);
                    SetCurrentValue(sliderStamina, stat.cur);
                    txtStamina.text = stat.cur.ToString() + " / " + stat.max.ToString();
                    break;

                case E_PlayerStatType.Focus:
                    SetMaxValue(sliderFocus, stat.max);
                    SetCurrentValue(sliderFocus, stat.cur);
                    txtFocus.text = stat.cur.ToString() + " / " + stat.max.ToString();
                    break;
                case E_PlayerStatType.PoisonBuild:
                    SetMaxValue(sliderPoisonBuildUp, stat.max);
                    SetCurrentValue(sliderPoisonBuildUp, stat.cur);
                    break;
                case E_PlayerStatType.PoisonAmount:
                    SetMaxValue(sliderPoisonAmount, stat.max);
                    SetCurrentValue(sliderPoisonAmount, stat.cur);
                    break;
            }
        }

        //更新灵魂数量UI显示
        private void UpdateSoulCountUI()
        {
            EnsureFacade();
            if (playerFacade == null) return;
            SetSoulCountText(playerFacade.CurrentSoulCount);
        }
        //初始化血条、耐力条、专注条等UI显示
        public void InitializePlayerUI()
        {
            EnsureFacade();
            if (playerFacade == null)
            {
                Debug.LogError("无法初始化玩家UI：PlayerUIFacade 为空");
                return;
            }
            hasInitializedUI = true;
            SetMaxValue(sliderHealth, playerFacade.MaxHealth);
            SetCurrentValue(sliderHealth, playerFacade.CurrentHealth);

            SetMaxValue(sliderStamina, (int)playerFacade.MaxStamina);
            SetCurrentValue(sliderStamina, (int)playerFacade.CurrentStamina);
            SetMaxValue(sliderFocus, (int)playerFacade.MaxFocus);
            SetCurrentValue(sliderFocus, (int)playerFacade.CurrentFocus);

            SetMaxValue(sliderPoisonBuildUp, 100);
            SetCurrentValue(sliderPoisonBuildUp, 0);
            SetMaxValue(sliderPoisonAmount, 100);
            SetCurrentValue(sliderPoisonAmount, 100);
            txtHealth.text = playerFacade.CurrentHealth.ToString() + " / " + playerFacade.MaxHealth.ToString();
            txtStamina.text = ((int)playerFacade.CurrentStamina).ToString() + " / " + ((int)playerFacade.MaxStamina).ToString();
            txtFocus.text = ((int)playerFacade.CurrentFocus).ToString() + " / " + ((int)playerFacade.MaxFocus).ToString();
            SetActive(sliderPoisonBuildUp, false);
            SetActive(sliderPoisonAmount, false);

            UpdateSoulCountUI();

            UpdateAllQuickSlots();
        }

        #endregion





        #region 快捷栏显示相关
        //更新左手武器槽UI显示
        private void UpdateWeaponLeftSlot()
        {
            EnsureFacade();
            if (playerFacade == null) return;
            UpdateQuickSlotUI(imgLeftSlot, playerFacade.LeftWeapon);
        }

        //更新右手武器槽UI显示
        private void UpdateWeaponRightSlot()
        {
            EnsureFacade();
            if (playerFacade == null) return;
            UpdateQuickSlotUI(imgRightSlot, playerFacade.RightWeapon);
        }

        //更新消耗品槽UI显示

        private void UpdateConsumableSlot()
        {
            EnsureFacade();
            if (playerFacade == null) return;
            UpdateQuickSlotUI(imgConsumableSlot, playerFacade.CurrentConsumable);
        }
        //更新法术槽UI显示
        private void UpdateSpellSlot()
        {
            EnsureFacade();
            if (playerFacade == null) return;
            UpdateQuickSlotUI(imgSpellSlot, playerFacade.CurrentSpell);
        }
        //更新所有快捷栏UI显示
        public void UpdateAllQuickSlots()
        {
            UpdateWeaponLeftSlot();
            UpdateWeaponRightSlot();
            UpdateConsumableSlot();
            UpdateSpellSlot();
        }

        #endregion

        //处理瞄准UI显示
        private void AimActionUI(bool isAiming)
        {
            SetActive(imgAim, isAiming);
        }


        public override void HideMe()
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_EnableInput, false);
        }

        public override void ShowMe()
        {

            EventCenter.Instance.EventTrigger(E_EventType.E_EnableInput, true);
            // 如果事件先于监听发布，这里主动初始化一次，保证 UI 有数据
            if (!hasInitializedUI)
            {
                InitializePlayerUI();
            }
        }

    }

}
