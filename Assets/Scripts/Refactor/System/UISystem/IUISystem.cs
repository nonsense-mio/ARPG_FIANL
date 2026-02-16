using Framework;
using ARPG;
using UnityEngine;
using UnityEngine.Events;

namespace ARPG
{
    /// <summary>
    /// UI层级枚举 (从 HT.E_UILayer 迁移)
    /// </summary>
    public enum E_UILayer
    {
        Bottom,
        Middle,
        Top,
        System,
    }

    /// <summary>
    /// UI系统接口 - 管理面板的显示/隐藏/获取
    /// 替代原 UIMgr (BaseManager 单例)
    /// </summary>
    public interface IUISystem : ISystem
    {
        /// <summary>
        /// 显示面板
        /// </summary>
        void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle, UnityAction<T> callBack = null) where T : BasePanel;

        /// <summary>
        /// 隐藏面板
        /// </summary>
        void HidePanel<T>(bool isDestroy = false) where T : BasePanel;

        /// <summary>
        /// 获取面板 (异步安全: 如面板正在加载, 会在加载完成后回调)
        /// </summary>
        void GetPanel<T>(UnityAction<T> callBack) where T : BasePanel;

        /// <summary>
        /// 获取对应层级的父对象
        /// </summary>
        Transform GetLayerFather(E_UILayer layer);
    }
}
