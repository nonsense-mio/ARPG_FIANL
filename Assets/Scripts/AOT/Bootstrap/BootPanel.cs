using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ARPG
{
    /// <summary>
    /// 启动期进度面板 — 纯 MonoBehaviour，不依赖 QFramework。
    /// 仅在 Bootstrap(AOT) 阶段由 Main.cs 直接驱动，因此刻意不继承 BasePanel/IController，
    /// 以免把整个 QFramework 拖进 AOT 程序集（参见架构分析报告 P0）。
    /// 字段可在 Inspector 赋值；未赋值时按子物体名称自动查找（兼容原 GetControl 行为，无需改场景）。
    /// </summary>
    public class BootPanel : MonoBehaviour
    {
        public Slider sliderProgress;
        public Text txtTips;

        void Awake()
        {
            if (sliderProgress == null) sliderProgress = FindByName<Slider>("sliderProgress");
            if (txtTips == null) txtTips = FindByName<Text>("txtTips");
        }

        public void SetProgress(float value, string tip)
        {
            if (sliderProgress != null) sliderProgress.value = value;
            if (txtTips != null) txtTips.text = tip;
        }

        private T FindByName<T>(string objName) where T : Component
            => GetComponentsInChildren<T>(true).FirstOrDefault(c => c.gameObject.name == objName);
    }
}
