using Framework;
using UnityEngine;

namespace ARPG
{
    public class BeginPanel : BasePanel
    {
        public override E_UICanvas CanvasType => E_UICanvas.Static;


        private void Start()
        {
            AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnStart"));
            AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnContinue"));
            AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnSetting"));
            AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnQuit"));
            AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnStart"));
            AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnContinue"));
            AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnSetting"));
            AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnQuit"));
        }

        protected override void ClickBtn(string btnName)
        {
            switch (btnName)
            {
                case "btnStart":
                    // 开始游戏：打开存档面板
                    this.GetSystem<IUISystem>().ShowPanel<SavePanel>();
                    this.GetSystem<IUISystem>().HidePanel<BeginPanel>();
                    break;
                case "btnContinue":
                    // 继续游戏：直接进入最近使用的存档
                    if (this.GetSystem<ISaveSystem>().HasAnySave())
                    {
                        int lastSlot = this.GetSystem<ISaveSystem>().SlotInfo.lastUsedSlot;
                        if (lastSlot >= 0)
                        {
                            this.GetSystem<IUISystem>().HidePanel<BeginPanel>();
                            this.SendCommand(new ClearGameInfoCommand());
                            this.SendCommand(new ContinueGameCommand(lastSlot));
                        }

                    }
                    //没有存档则提示
                    else
                    {
                        this.GetSystem<IUISystem>().ShowPanel<TipPanel>((panel) =>
                        {
                            panel.SetTipInfo("没有可继续的存档");
                        });
                    }
                    break;
                case "btnSetting":
                    this.GetSystem<IUISystem>().ShowPanel<SettingPanel>();
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
