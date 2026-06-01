using Framework;
using UnityEngine.Events;

namespace ARPG
{
    /// <summary>
    /// UI Canvas 分组枚举 - 按更新频率分离 Canvas 以优化重建性能
    /// </summary>
    public enum E_UICanvas
    {
        Dynamic,     // 高频变动 (HUD)
        Common,      // 中频交互 (菜单)
        Static,      // 低频静态 (主菜单)
        Overlay      // 全局最高层 (弹窗/对话)
    }

    /// <summary>
    /// UI系统接口 - 管理面板的显示/隐藏/获取
    /// </summary>
    public interface IUISystem : ISystem
    {
        /// <summary>
        /// 显示面板
        /// </summary>
        void ShowPanel<T>(UnityAction<T> callBack = null) where T : BasePanel;

        /// <summary>
        /// 隐藏面板
        /// </summary>
        void HidePanel<T>(bool isDestroy = false) where T : BasePanel;

        /// <summary>
        /// 获取面板 
        /// </summary>
        void GetPanel<T>(UnityAction<T> callBack) where T : BasePanel;

        /// <summary>
        /// 销毁并卸载所有已打开的面板
        /// </summary>
        void ClearAllPanels();
    }
}
