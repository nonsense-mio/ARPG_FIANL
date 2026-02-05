using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT
{
    public class InteractionPanel : BasePanel
    {
        private Image imgSelectWindow;
        private Slider sliderBossHP;
        private Text txtBossName;
        private Text txtTips;
        public Image imgTips;

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
            EventCenter.Instance.AddEventListener<bool>(E_EventType.E_OpenOrCloseSelectWindow, SetActiveSelectWindow);
            EventCenter.Instance.AddEventListener<Interactable>(E_EventType.E_Interact, PopUpTips);
            EventCenter.Instance.AddEventListener<BossHudData?>(E_EventType.E_BossHudChanged, OnBossHudChanged);
        }
        private void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener<bool>(E_EventType.E_OpenOrCloseSelectWindow, SetActiveSelectWindow);
            EventCenter.Instance.RemoveEventListener<Interactable>(E_EventType.E_Interact, PopUpTips);
            EventCenter.Instance.RemoveEventListener<BossHudData?>(E_EventType.E_BossHudChanged, OnBossHudChanged);
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
                        UIMgr.Instance.ShowPanel<BagPanel>(callBack: (bagPanel) =>
                        {
                            bagPanel.RequestInitAllInventory();
                        });
                        SetActiveSelectWindow(false);
                    }
                    break;
                //装备按钮
                case "btnEquip":
                    {
                        UIMgr.Instance.ShowPanel<EquipPanel>(callBack: (equipPanel) =>
                        {
                            equipPanel.Refresh();
                        });
                        SetActiveSelectWindow(false);
                    }
                    break;
                case "btnTask":
                    {
                        UIMgr.Instance.ShowPanel<TaskPanel>();
                        SetActiveSelectWindow(false);
                    }
                    break;
                //退出按钮
                case "btnQuit":
                    {
                        UIMgr.Instance.ShowPanel<TipPanel>(E_UILayer.Top, (tipPanel) =>
                        {
                            tipPanel.SetTipInfo("是否返回标题界面？", () =>
                            {
                                UIMgr.Instance.HidePanel<InteractionPanel>(true);
                                UIMgr.Instance.HidePanel<EquipPanel>(true);
                                UIMgr.Instance.HidePanel<BagPanel>(true);
                                UIMgr.Instance.HidePanel<GamePanel>(true);
                                //保存并清理
                                GameManager.Instance.SaveCurrentGame();
                                GameManager.Instance.ClearInfo();
                                //异步加载场景
                                SceneMgr.Instance.LoadSceneAsync("BeginScene", () =>
                                {
                                    GameManager.Instance.InitBeginScene();
                                });
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
