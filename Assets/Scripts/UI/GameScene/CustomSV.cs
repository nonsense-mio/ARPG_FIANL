using System.Collections.Generic;
using ARPG;
using UnityEngine;
using UnityEngine.Events; // 引入 UnityAction
using UnityEngine.UI;     // 引入 Button

namespace HT
{
    /// <summary>
    /// 该接口 作为必须被 格子对象继承的类 用于实现初始化格子的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IItemBase<T>
    {
        void InitInfo(T info);
    }

    /// <summary>
    /// 自定义sv类 用于节约性能 通过缓存池创建复用对象
    /// </summary>
    /// <typeparam name="T">代表数据来源类</typeparam>
    /// <typeparam name="K">代表格子类</typeparam>
    public class CustomSV<T, K> where K : IItemBase<T>
    {
        //需要通过它得到可视范围的位置 还要把动态创建的格子设为它的子对象
        private RectTransform content;
        //可视范围高
        private int viewPortH;

        //当前显示着的格子对象
        private Dictionary<int, GameObject> nowShowItem = new Dictionary<int, GameObject>();

        //数据来源
        private List<T> items;

        //记录上一次显示的索引范围
        private int oldMinIndex = -1;
        private int oldMaxIndex = -1;
        //格子的间隔宽高
        private int itemW;
        private int itemH;
        //一行有多少格子
        private int n = 3;
        //预设体资源的路径
        private string itemResName;
        /// <summary>
        /// 初始化格子资源路径
        /// </summary>
        /// <param name="name"></param>
        public void InitItemResName(string name)
        {
            itemResName = name;
        }

        /// <summary>
        /// 初始化content父对象 以及我们可视范围的高
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="h"></param>
        public void InitContentAndSVH(RectTransform trans, int h)
        {
            content = trans;
            viewPortH = h;
        }


        /// <summary>
        /// 初始化数据来源 并且初始化content的高
        /// </summary>
        /// <param name="items"></param>
        public void InitInfos(List<T> items)
        {
            this.items = items;
            //初始化content的长度
            content.sizeDelta = new Vector2(0, Mathf.CeilToInt(items.Count / (float)n) * itemH);
        }
        /// <summary>
        /// 初始化格子间隔大小 以及一行有几个格子
        /// </summary>
        /// <param name="w">格子宽的间隔</param>
        /// <param name="h">格子高的间隔</param>
        /// <param name="n">一行的格子个数</param>
        public void InitItemSizeAndNum(int w, int h, int n)
        {
            this.itemW = w;
            this.itemH = h;
            this.n = n;
        }

        // 1. 定义一个回调，参数是 T (具体的数据，比如 Item)
        private UnityAction<T> onSelect;

        /// <summary>
        /// 提供给外部调用的方法，用于注册点击事件
        /// </summary>
        /// <param name="action"></param>
        public void AddListener(UnityAction<T> action)
        {
            this.onSelect = action;
        }

        /// <summary>
        /// 更新格子显示的方法
        /// </summary>
        public void CheckShowOrHide()
        {
            //检测哪些格子应该显示出来
            int minIndex = (int)(content.anchoredPosition.y / itemH) * n;
            int maxIndex = (int)(content.anchoredPosition.y + viewPortH) / itemH * n + (n - 1);
            if (minIndex < 0)
                minIndex = 0;
            //如果超出最大道具数量
            if (maxIndex >= items.Count)
                maxIndex = items.Count - 1;

            if (minIndex != oldMinIndex || maxIndex != oldMaxIndex)
            {
                //根据上一次的索引和这次新算的索引 用来判断 哪些该移除
                for (int i = oldMinIndex; i < minIndex; i++)
                {
                    if (nowShowItem.ContainsKey(i))
                    {
                        if (nowShowItem[i] != null)
                            GameArchitecture.Interface.GetSystem<PoolSystem>().Recycle(nowShowItem[i]);
                        nowShowItem.Remove(i);
                    }
                }
                for (int i = maxIndex + 1; i <= oldMaxIndex; i++)
                {
                    if (nowShowItem.ContainsKey(i))
                    {
                        if (nowShowItem[i] != null)
                            GameArchitecture.Interface.GetSystem<PoolSystem>().Recycle(nowShowItem[i]);
                        nowShowItem.Remove(i);
                    }
                }

                oldMinIndex = minIndex;
                oldMaxIndex = maxIndex;
            }
            //创建格子
            for (int i = minIndex; i <= maxIndex; i++)
            {
                if (nowShowItem.ContainsKey(i))
                {
                    continue;
                }
                else
                {
                    int index = i;
                    nowShowItem.Add(index, null);
                    //取出对象
                    GameObject obj = GameArchitecture.Interface.GetSystem<PoolSystem>().Spawn(itemResName);
                    //设置它的父对象
                    obj.transform.SetParent(content, false);
                    //重置相对缩放大小
                    obj.transform.localScale = Vector3.one;
                    //重置位置
                    obj.transform.localPosition = new Vector3((index % n) * itemW + 20, -index / n * itemH - 10, 0);

                    // 2. 获取数据并初始化
                    T data = items[i]; // 获取当前数据
                    K itemScript = obj.GetComponent<K>();
                    itemScript.InitInfo(data);

                    // 3. 自动添加按钮监听
                    Button btn = obj.GetComponent<Button>(); 
                    if (btn != null)
                    {
                        // 因为是对象池复用的，必须先移除旧事件
                        btn.onClick.RemoveAllListeners();
                        
                        // 添加新事件
                        if (onSelect != null)
                        {
                            btn.onClick.AddListener(() =>
                            {
                                onSelect(data);
                            });
                        }
                    }

                    if (nowShowItem.ContainsKey(index))
                        nowShowItem[index] = obj;
                    else
                        GameArchitecture.Interface.GetSystem<PoolSystem>().Recycle(obj);
                }

            }
        }

        public void ClearItem()
        {
            foreach (var item in nowShowItem)
            {
                if (item.Value != null)
                    GameArchitecture.Interface.GetSystem<PoolSystem>().Recycle(item.Value);
            }
            nowShowItem.Clear();
            oldMinIndex = -1;
            oldMaxIndex = -1;
        }


    }
    

}
