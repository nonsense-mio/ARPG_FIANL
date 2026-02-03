using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HT
{
    public class TipPanel : BasePanel
    {
        UnityAction sureCallBack;

        private void Start()
        {
            UIMgr.AddUISelectSound(GetControl<Button>("btnClose"));
            UIMgr.AddUISelectSound(GetControl<Button>("btnSure"));
            UIMgr.AddUIConfirmSound(GetControl<Button>("btnClose"));
            UIMgr.AddUIConfirmSound(GetControl<Button>("btnSure"));
        }

        protected override void ClickBtn(string btnName)
        {
            switch (btnName)
            {
                //确认时执行回调
                case "btnSure":
                    UIMgr.Instance.HidePanel<TipPanel>();
                    sureCallBack?.Invoke();
                    break;
                //关闭时不执行回调 仅关闭提示面板
                case "btnClose":
                    UIMgr.Instance.HidePanel<TipPanel>();
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
