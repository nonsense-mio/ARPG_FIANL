using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace HT
{
    /// <summary>
    /// 层级枚举
    /// </summary>
    public enum E_UILayer
    {
        Bottom,
        Middle,
        Top,
        System,
    }
    /// <summary>
    /// 管理所有UI面板的管理器
    /// 注意：面板预设体名要和面板类名一致
    /// </summary>
    public class UIMgr : BaseManager<UIMgr>
    {
        /// <summary>
        /// 里氏替换原则 用父类容器装载子类对象
        /// </summary>
        private abstract class BasePanelInfo { }
        /// <summary>
        /// 用于存储面板信息 和加载完成的回调函数
        /// </summary>
        /// <typeparam name="T">面板的类型</typeparam>
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

        private Camera uiCamera;
        private Canvas uiCanvas;
        private EventSystem uiEventSystem;
        //层级父对象
        private Transform bottomLayer;
        private Transform middleLayer;
        private Transform topLayer;
        private Transform systemLayer;
        /// <summary>
        /// 用于存储所有面板对象的容器
        /// </summary>
        private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>();

        private UIMgr()
        {
            //动态创建唯一的Canvas和EventSystem(摄像机)
            uiCamera = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/UICamera")).GetComponent<Camera>();
            //ui摄像机过场景不移除
            GameObject.DontDestroyOnLoad(uiCamera.gameObject);

            //动态创建Canvas
            uiCanvas = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
            //设置使用的UI摄像机
            uiCanvas.worldCamera = uiCamera;
            //过场景不移除
            GameObject.DontDestroyOnLoad(uiCanvas.gameObject);

            //找到层级父对象
            bottomLayer = uiCanvas.transform.Find("Bottom");
            middleLayer = uiCanvas.transform.Find("Middle");
            topLayer = uiCanvas.transform.Find("Top");
            systemLayer = uiCanvas.transform.Find("System");


            //动态创建EventSystem
            uiEventSystem = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
            GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
        }

        /// <summary>
        /// 获取对应层级的父对象
        /// </summary>
        /// <param name="layer">层级枚举值</param>
        /// <returns></returns>
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

        /// <summary>
        /// 显示面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <param name="layer">面板所在层级</param>
        /// <param name="callBack">由于可能是异步加载 通过委托回调的形式 将加载完成的面板传递出去进行使用</param>
        /// <param name="isSync">是否采用同步加载 默认为false</param>
        public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle, UnityAction<T> callBack = null, bool isSync = false) where T : BasePanel
        {
            //获取面板名  预设体名必须和面板类名一致
            string panelName = typeof(T).Name;
            //存在面板
            if (panelDic.ContainsKey(panelName))
            {
                //取出字典中已经占好位置的数据
                PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
                //正在异步加载中
                if (panelInfo.panel == null)
                {
                    panelInfo.isHide = false;
                    //应该等待异步加载完毕 只需要记录回调函数  加载完后去调用
                    if (callBack != null)
                        panelInfo.callBack += callBack;
                }
                //已经加载结束
                else
                {
                    //显示面板 如果是失活状态 直接激活面板
                    if (!panelInfo.panel.gameObject.activeSelf)
                        panelInfo.panel.gameObject.SetActive(true);
                    //先回调，便于外部及时 Bind 数据
                    callBack?.Invoke(panelInfo.panel);
                    //再执行面板显示逻辑
                    panelInfo.panel.ShowMe();
                }
                return;
            }
            //不存在面板 先将信息存入字典
            panelDic.Add(panelName, new PanelInfo<T>(callBack));
            //加载面板
            ABResMgr.Instance.LoadResAsync<GameObject>("ui", panelName, (res) =>
            {
                //取出字典中已经占好位置的数据
                PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
                //表示异步加载结束前 就想要隐藏面板
                if (panelInfo.isHide)
                {
                    panelDic.Remove(panelName);
                    return;
                }
                //层级的处理
                Transform father = GetLayerFather(layer);
                if (father == null)
                    father = middleLayer;
                //将面板预设体创建到对应父对象下 并且保持原本的缩放大小
                GameObject panelObj = GameObject.Instantiate(res, father, false);


                //获取对应组件 返回出去
                T panel = panelObj.GetComponent<T>();
                //先回调，便于外部及时 Bind 数据
                panelInfo.callBack?.Invoke(panel);
                //回调执行完将其清空 避免内存泄露
                panelInfo.callBack = null;
                //再执行面板显示逻辑
                panel.ShowMe();
                //存储panel
                panelInfo.panel = panel;

            }, isSync);


        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        public void HidePanel<T>(bool isDestroy = false) where T : BasePanel
        {
            string panelName = typeof(T).Name;
            if (panelDic.ContainsKey(panelName))
            {
                PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
                //字典中存在 但 正在加载中
                if (panelInfo.panel == null)
                {
                    //修改隐藏标识 表示这个面板即将要隐藏
                    panelInfo.isHide = true;
                    //既然要隐藏 将回调函数置空
                    panelInfo.callBack = null;
                }
                else //加载结束
                {
                    panelInfo.panel.HideMe();
                    if (isDestroy)
                    {
                        //销毁面板
                        GameObject.Destroy(panelInfo.panel.gameObject);
                        //从字典中移除
                        panelDic.Remove(panelName);
                    }
                    //如果不销毁 那么就只是失活
                    else
                    {
                        panelInfo.panel.gameObject.SetActive(false);
                    }

                }

            }
        }

        /// <summary>
        /// 获取面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        public void GetPanel<T>(UnityAction<T> callBack) where T : BasePanel
        {
            string panelName = typeof(T).Name;
            if (panelDic.ContainsKey(panelName))
            {
                PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
                //正在加载中
                if (panelInfo.panel == null)
                {
                    //应等待加载结束再通过回调传递给外部
                    panelInfo.callBack += callBack;
                }
                //加载结束 且该面板不会被隐藏时
                else if (!panelInfo.isHide)
                {
                    //通过委托把面板传出去
                    callBack?.Invoke(panelInfo.panel);
                }

            }

        }

        /// <summary>
        /// 为控件添加自定义事件
        /// </summary>
        /// <param name="control">对应控件</param>
        /// <param name="type">事件类型</param>
        /// <param name="callBack">响应函数</param>
        public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
        {
            EventTrigger trigger = control.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = control.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(callBack);

            trigger.triggers.Add(entry);
        }

        //鼠标悬停音效
        public static void AddUISelectSound(UIBehaviour control)
        {
            AddCustomEventListener(control, EventTriggerType.PointerEnter, (data) =>
            {
                MusicMgr.Instance.PlaySound("UI_Select");
            });
        }
        //鼠标点击音效
        public static void AddUIConfirmSound(UIBehaviour control)
        {
            AddCustomEventListener(control, EventTriggerType.PointerClick, (data) =>
            {
                MusicMgr.Instance.PlaySound("UI_Confirm");
            });
        }
    }

}
