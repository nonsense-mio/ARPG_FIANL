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

        private abstract class BasePanelInfo { }

        private class PanelInfo<T> : BasePanelInfo where T : BasePanel
        {
            public T panel;
            public UnityAction<T> callBack;
            public bool isHide;

            public PanelInfo(UnityAction<T> callBack)
            {
                this.callBack += callBack;
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

        // 每个 Canvas 的层级引用: canvasLayers[canvasType][(int)layer]
        private Dictionary<E_UICanvas, Transform[]> canvasLayers = new Dictionary<E_UICanvas, Transform[]>();

        private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>();

        private IAssetLoader assetLoader;

        protected override void OnInit()
        {
            var resourceLoader = this.GetUtility<IResourceLoader>();
            assetLoader = this.GetUtility<IAssetLoader>();

            //动态创建UICamera
            uiCamera = GameObject.Instantiate(resourceLoader.Load<GameObject>("UI/UICamera")).GetComponent<Camera>();
            GameObject.DontDestroyOnLoad(uiCamera.gameObject);

            //加载 UIRoot (内含 4 个 Canvas 子物体，每个 Canvas 下有 Bottom/Middle/Top/System)
            uiRoot = GameObject.Instantiate(resourceLoader.Load<GameObject>("UI/UIRoot"));
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

            //动态创建EventSystem
            uiEventSystem = GameObject.Instantiate(resourceLoader.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
            GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
        }

        private void InitCanvasLayers(E_UICanvas type, Canvas canvas)
        {
            canvasLayers[type] = new Transform[]
            {
                canvas.transform.Find("Bottom"),
                canvas.transform.Find("Middle"),
                canvas.transform.Find("Top"),
                canvas.transform.Find("System")
            };
        }

        public Transform GetLayerFather(E_UILayer layer)
        {
            return GetLayerFather(E_UICanvas.Common, layer);
        }

        private Transform GetLayerFather(E_UICanvas canvasType, E_UILayer layer)
        {
            if (canvasLayers.TryGetValue(canvasType, out var layers))
                return layers[(int)layer] ?? layers[(int)E_UILayer.Middle];
            return canvasLayers[E_UICanvas.Common][(int)E_UILayer.Middle];
        }

        public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle, UnityAction<T> callBack = null) where T : BasePanel
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
                PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
                //异步加载结束前就想要隐藏
                if (panelInfo.isHide)
                {
                    panelDic.Remove(panelName);
                    return;
                }
                //先实例化获取面板组件，再根据 CanvasType 路由到对应 Canvas 的层级
                GameObject panelObj = GameObject.Instantiate(res);
                T panel = panelObj.GetComponent<T>();

                Transform father = GetLayerFather(panel.CanvasType, layer);
                panelObj.transform.SetParent(father, false);

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
                    }
                    else
                    {
                        panelInfo.panel.gameObject.SetActive(false);
                    }
                }
            }
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
