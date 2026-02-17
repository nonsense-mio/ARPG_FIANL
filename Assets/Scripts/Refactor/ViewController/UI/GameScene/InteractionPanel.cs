using System.Collections.Generic;
using Framework;
using UnityEngine.UI;

namespace ARPG
{
    public class InteractionPanel : BasePanel
    {
        public override E_UICanvas CanvasType => E_UICanvas.Dynamic;

        private Image imgSelectWindow;
        private Slider sliderBossHP;
        private Text txtBossName;
        private Text txtTips;
        public Image imgTips;

        private readonly List<IUnRegister> unRegisters = new List<IUnRegister>();

        protected override void Awake()
        {
            base.Awake();
            imgSelectWindow = GetControl<Image>("imgSelectWindow");
            sliderBossHP = GetControl<Slider>("sliderBossHP");
            txtBossName = GetControl<Text>("txtBossName");
            txtTips = GetControl<Text>("txtTips");
            imgTips = GetControl<Image>("imgTips");
        }
        private void OnEnable()
        {
            unRegisters.Add(this.RegisterEvent<SelectWindowEvent>(e => SetActiveSelectWindow(e.IsOpen)));
            unRegisters.Add(this.RegisterEvent<InteractPromptEvent>(e => PopUpTips(e.Target)));
            unRegisters.Add(this.RegisterEvent<BossHudChangedEvent>(e => OnBossHudChanged(e.Data)));
        }
        private void OnDisable()
        {
            foreach (var ur in unRegisters) ur.UnRegister();
            unRegisters.Clear();
        }

        #region boss相关
        //参数data为空时隐藏boss血条 否则显示并更新 ？表示可空类型
        private void OnBossHudChanged(BossHudData? data)
        {
            if (data == null)
            {
                print("隐藏boss血条");
                // 隐藏
                SetActive(sliderBossHP, false);
                SetActive(txtBossName, false);
                return;
            }

            BossHudData d = data.Value;

            // 显示 & 更新
            txtBossName.text = d.name;

            SetActive(sliderBossHP, true);
            SetActive(txtBossName, true);

            // max 变化时设置 max（这里简单每次 set 一次）
            SetMaxValue(sliderBossHP, d.maxHp);
            SetCurrentValue(sliderBossHP, d.curHp);
        }
        #endregion
        private void SetActiveSelectWindow(bool isActive)
        {

            SetActive(imgSelectWindow, isActive);
        }
        private void PopUpTips(Interactable interact)
        {
            if (interact == null)
            {
                SetActive(imgTips, false);
            }
            else
            {
                txtTips.text = interact.interactionPrompt;
                SetActive(imgTips, true);
            }

        }
        //按钮点击事件
        protected override void ClickBtn(string btnName)
        {
            switch (btnName)
            {
                //背包按钮
                case "btnBag":
                    {
                        this.GetSystem<IUISystem>().ShowPanel<BagPanel>(callBack: (bagPanel) =>
                        {
                            bagPanel.RequestInitAllInventory();
                        });
                        SetActiveSelectWindow(false);
                    }
                    break;
                //装备按钮
                case "btnEquip":
                    {
                        this.GetSystem<IUISystem>().ShowPanel<EquipPanel>(callBack: (equipPanel) =>
                        {
                            equipPanel.Refresh();
                        });
                        SetActiveSelectWindow(false);
                    }
                    break;
                case "btnTask":
                    {
                        this.GetSystem<IUISystem>().ShowPanel<TaskPanel>();
                        SetActiveSelectWindow(false);
                    }
                    break;
                //退出按钮
                case "btnQuit":
                    {
                        this.GetSystem<IUISystem>().ShowPanel<TipPanel>(E_UILayer.Top, (tipPanel) =>
                        {
                            tipPanel.SetTipInfo("是否返回标题界面？", () =>
                            {
                                this.SendCommand(new ReturnToMainMenuCommand());
                            });
                        });
                    }
                    break;
            }
        }


        public override void HideMe()
        {

        }

        public override void ShowMe()
        {

        }
    }

}
