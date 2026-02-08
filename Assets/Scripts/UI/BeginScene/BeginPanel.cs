using System.Collections;
using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

namespace HT
{
    public class BeginPanel : BasePanel
    {

        private void Start()
        {
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnStart"));
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnContinue"));
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnSetting"));
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnQuit"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnStart"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnContinue"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnSetting"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnQuit"));
        }

        protected override void ClickBtn(string btnName)
        {
            switch (btnName)
            {
                case "btnStart":
                    // 开始游戏：打开存档面板
                    UIMgr.Instance.ShowPanel<SavePanel>();
                    UIMgr.Instance.HidePanel<BeginPanel>();
                    break;
                case "btnContinue":
                    // 继续游戏：直接进入最近使用的存档
                    if (this.GetSystem<ISaveSystem>().HasAnySave())
                    {
                        int lastSlot = this.GetSystem<ISaveSystem>().SlotInfo.lastUsedSlot;
                        if (lastSlot >= 0)
                        {
                            UIMgr.Instance.HidePanel<BeginPanel>();
                            GameManager.Instance.ClearInfo();
                            GameManager.Instance.ContinueGame(lastSlot);
                        }

                    }
                    //没有存档则提示
                    else
                    {
                        UIMgr.Instance.ShowPanel<TipPanel>(E_UILayer.Top, (panel) =>
                        {
                            panel.SetTipInfo("没有可继续的存档");
                        });
                    }
                    break;
                case "btnSetting":
                    UIMgr.Instance.ShowPanel<SettingPanel>();
                    break;
                case "btnQuit":
                    Application.Quit();
                    break;
            }
        }

        public override void ShowMe()
        {

        }

        public override void HideMe()
        {

        }


    }

}
