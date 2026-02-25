using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ARPG
{
    /// <summary>
    /// UI系统实现 - 管理面板的生命周期和层级
    /// 支持多 Canvas 动静分离 (Dynamic/Common/Static/Overlay)
    /// </summary>
    public class UISystem : AbstractSystem, IUISystem
    {
        #region 内部类型

        private abstract class BasePanelInfo
        {
            public abstract void Clear(IAssetLoader assetLoader, string panelName);
        }

        private class PanelInfo<T> : BasePanelInfo where T : BasePanel
        {
            public T panel;
            public UnityAction<T> callBack;
            public bool isHide;

            public PanelInfo(UnityAction<T> callBack)
            {
                this.callBack += callBack;
            }

            public override void Clear(IAssetLoader assetLoader, string panelName)
            {
                if (panel != null)
                {
                    panel.HideMe();
                    GameObject.Destroy(panel.gameObject);
                }
                else
                {
                    isHide = true;
                    callBack = null;
                }
                assetLoader.Unload($"ui/{panelName}");
            }
        }

        #endregion

        private Camera uiCamera;
        private EventSystem uiEventSystem;
        private GameObject uiRoot;

        // 四 Canvas 动静分离
        private Canvas dynamicCanvas;
        private Canvas commonCanvas;
        private Canvas staticCanvas;
        private Canvas overlayCanvas;

        // 每个 Canvas 的根 Transform: canvasLayers[canvasType]
        private Dictionary<E_UICanvas, Transform> canvasLayers = new Dictionary<E_UICanvas, Transform>();

        private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>();

        private IAssetLoader assetLoader;

        protected override void OnInit()
        {
            assetLoader = this.GetUtility<IAssetLoader>();

            //动态创建UICamera (通过 YooAsset 同步加载)
            uiCamera = GameObject.Instantiate(assetLoader.LoadSync<GameObject>("ui_bootstrap/UICamera")).GetComponent<Camera>();
            GameObject.DontDestroyOnLoad(uiCamera.gameObject);

            //加载 UIRoot (内含 4 个 Canvas 子物体: DynamicCanvas, CommonCanvas, StaticCanvas, OverlayCanvas)
            uiRoot = GameObject.Instantiate(assetLoader.LoadSync<GameObject>("ui_bootstrap/UIRoot"));
            GameObject.DontDestroyOnLoad(uiRoot);

            dynamicCanvas = uiRoot.transform.Find("DynamicCanvas").GetComponent<Canvas>();
            commonCanvas = uiRoot.transform.Find("CommonCanvas").GetComponent<Canvas>();
            staticCanvas = uiRoot.transform.Find("StaticCanvas").GetComponent<Canvas>();
            overlayCanvas = uiRoot.transform.Find("OverlayCanvas").GetComponent<Canvas>();

            //统一绑定 UICamera
            dynamicCanvas.worldCamera = uiCamera;
            commonCanvas.worldCamera = uiCamera;
            staticCanvas.worldCamera = uiCamera;
            overlayCanvas.worldCamera = uiCamera;

            //初始化各 Canvas 的层级引用
            InitCanvasLayers(E_UICanvas.Dynamic, dynamicCanvas);
            InitCanvasLayers(E_UICanvas.Common, commonCanvas);
            InitCanvasLayers(E_UICanvas.Static, staticCanvas);
            InitCanvasLayers(E_UICanvas.Overlay, overlayCanvas);

            //动态创建EventSystem (通过 YooAsset 同步加载)
            uiEventSystem = GameObject.Instantiate(assetLoader.LoadSync<GameObject>("ui_bootstrap/EventSystem")).GetComponent<EventSystem>();
            GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
        }

        private void InitCanvasLayers(E_UICanvas type, Canvas canvas)
        {
            canvasLayers[type] = canvas.transform;
        }

        public void ShowPanel<T>(UnityAction<T> callBack = null) where T : BasePanel
        {
            string panelName = typeof(T).Name;
            if (panelDic.ContainsKey(panelName))
            {
                PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
                //正在异步加载中
                if (panelInfo.panel == null)
                {
                    panelInfo.isHide = false;
                    if (callBack != null)
                        panelInfo.callBack += callBack;
                }
                //已经加载结束
                else
                {
                    if (!panelInfo.panel.gameObject.activeSelf)
                        panelInfo.panel.gameObject.SetActive(true);
                    panelInfo.panel.transform.SetAsLastSibling();
                    callBack?.Invoke(panelInfo.panel);
                    panelInfo.panel.ShowMe();
                }
                return;
            }
            //不存在面板 先将信息存入字典
            panelDic.Add(panelName, new PanelInfo<T>(callBack));
            //加载面板
            assetLoader.LoadAsync<GameObject>($"ui/{panelName}", (res) =>
            {
                if (!panelDic.TryGetValue(panelName, out var info))
                    return;
                PanelInfo<T> panelInfo = info as PanelInfo<T>;
                //异步加载结束前就想要隐藏
                if (panelInfo.isHide)
                {
                    panelDic.Remove(panelName);
                    assetLoader.Unload($"ui/{panelName}");
                    return;
                }
                //实例化面板并根据 CanvasType 路由到对应 Canvas
                GameObject panelObj = GameObject.Instantiate(res);
                T panel = panelObj.GetComponent<T>();

                Transform canvasRoot = canvasLayers[panel.CanvasType];
                panelObj.transform.SetParent(canvasRoot, false);

                panelInfo.callBack?.Invoke(panel);
                panelInfo.callBack = null;
                panel.ShowMe();
                panelInfo.panel = panel;
            });
        }

        public void HidePanel<T>(bool isDestroy = false) where T : BasePanel
        {
            string panelName = typeof(T).Name;
            if (panelDic.ContainsKey(panelName))
            {
                PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
                //正在加载中
                if (panelInfo.panel == null)
                {
                    panelInfo.isHide = true;
                    panelInfo.callBack = null;
                }
                else
                {
                    panelInfo.panel.HideMe();
                    if (isDestroy)
                    {
                        GameObject.Destroy(panelInfo.panel.gameObject);
                        panelDic.Remove(panelName);
                        assetLoader.Unload($"ui/{panelName}");
                    }
                    else
                    {
                        panelInfo.panel.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void ClearAllPanels()
        {
            foreach (var kvp in panelDic)
                kvp.Value.Clear(assetLoader, kvp.Key);
            panelDic.Clear();
        }

        public void GetPanel<T>(UnityAction<T> callBack) where T : BasePanel
        {
            string panelName = typeof(T).Name;
            if (panelDic.ContainsKey(panelName))
            {
                PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
                //正在加载中
                if (panelInfo.panel == null)
                {
                    panelInfo.callBack += callBack;
                }
                //加载结束且不会被隐藏
                else if (!panelInfo.isHide)
                {
                    callBack?.Invoke(panelInfo.panel);
                }
            }
        }
    }
}
