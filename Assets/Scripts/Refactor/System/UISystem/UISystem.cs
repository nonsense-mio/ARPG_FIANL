using System.Collections.Generic;
using Framework;
using HT;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ARPG
{
    /// <summary>
    /// UI系统实现 - 管理面板的生命周期和层级
    /// 替代原 UIMgr (BaseManager 单例)
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
        private Canvas uiCanvas;
        private EventSystem uiEventSystem;

        private Transform bottomLayer;
        private Transform middleLayer;
        private Transform topLayer;
        private Transform systemLayer;

        private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>();

        private IAssetSystem assetSystem;

        protected override void OnInit()
        {
            var resourceSystem = this.GetUtility<IResourceSystem>();
            assetSystem = this.GetUtility<IAssetSystem>();

            //动态创建UICamera
            uiCamera = GameObject.Instantiate(resourceSystem.Load<GameObject>("UI/UICamera")).GetComponent<Camera>();
            GameObject.DontDestroyOnLoad(uiCamera.gameObject);

            //动态创建Canvas
            uiCanvas = GameObject.Instantiate(resourceSystem.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
            uiCanvas.worldCamera = uiCamera;
            GameObject.DontDestroyOnLoad(uiCanvas.gameObject);

            //找到层级父对象
            bottomLayer = uiCanvas.transform.Find("Bottom");
            middleLayer = uiCanvas.transform.Find("Middle");
            topLayer = uiCanvas.transform.Find("Top");
            systemLayer = uiCanvas.transform.Find("System");

            //动态创建EventSystem
            uiEventSystem = GameObject.Instantiate(resourceSystem.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
            GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
        }

        public Transform GetLayerFather(E_UILayer layer)
        {
            switch (layer)
            {
                case E_UILayer.Bottom:
                    return bottomLayer;
                case E_UILayer.Middle:
                    return middleLayer;
                case E_UILayer.Top:
                    return topLayer;
                case E_UILayer.System:
                    return systemLayer;
                default:
                    return null;
            }
        }

        public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle, UnityAction<T> callBack = null, bool isSync = false) where T : BasePanel
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
            assetSystem.LoadAssetAsync<GameObject>("ui", panelName, (res) =>
            {
                PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
                //异步加载结束前就想要隐藏
                if (panelInfo.isHide)
                {
                    panelDic.Remove(panelName);
                    return;
                }
                //层级处理
                Transform father = GetLayerFather(layer);
                if (father == null)
                    father = middleLayer;
                //创建面板到对应父对象下
                GameObject panelObj = GameObject.Instantiate(res, father, false);

                T panel = panelObj.GetComponent<T>();
                panelInfo.callBack?.Invoke(panel);
                panelInfo.callBack = null;
                panel.ShowMe();
                panelInfo.panel = panel;

            }, isSync);
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
