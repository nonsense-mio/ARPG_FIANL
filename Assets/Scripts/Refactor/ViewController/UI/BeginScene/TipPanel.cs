using ARPG;
using Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ARPG
{
    public class TipPanel : BasePanel
    {
        UnityAction sureCallBack;

        private void Start()
        {
            AddUISelectSound(GetControl<Button>("btnClose"));
            AddUISelectSound(GetControl<Button>("btnSure"));
            AddUIConfirmSound(GetControl<Button>("btnClose"));
            AddUIConfirmSound(GetControl<Button>("btnSure"));
        }

        protected override void ClickBtn(string btnName)
        {
            switch (btnName)
            {
                //确认时执行回调
                case "btnSure":
                    this.GetSystem<IUISystem>().HidePanel<TipPanel>();
                    sureCallBack?.Invoke();
                    break;
                //关闭时不执行回调 仅关闭提示面板
                case "btnClose":
                    this.GetSystem<IUISystem>().HidePanel<TipPanel>();
                    break;
            }
        }

        public void SetTipInfo(string info,UnityAction callBack = null)
        {
            GetControl<Text>("txtInfo").text = info;
            sureCallBack = callBack;
        }


        public override void ShowMe()
        {
        
        }
        public override void HideMe()
        {
            
        }
    }

}
