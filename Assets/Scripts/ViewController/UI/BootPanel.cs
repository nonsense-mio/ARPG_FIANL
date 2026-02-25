using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ARPG
{
    public class BootPanel : BasePanel
    {
        public Slider sliderProgress;
        public Text txtTips;
        protected override void Awake()
        {
            base.Awake();
            sliderProgress = GetControl<Slider>("sliderProgress");
            txtTips = GetControl<Text>("txtTips");
        }
        
        public void SetProgress(float value, string tip)
        {
            sliderProgress.value = value;
            txtTips.text = tip;
        }

        public override void ShowMe() { }
        public override void HideMe() { }
    }
}

