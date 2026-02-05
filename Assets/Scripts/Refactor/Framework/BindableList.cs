using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Framework
{
    /// <summary>
    /// 可绑定列表：支持 Add/Remove/Clear 事件通知
    /// 基于 Collection&lt;T&gt; + EasyEvent 实现
    /// </summary>
    [Serializable]
    public class BindableList<T> : Collection<T>
    {
        #region 事件
        private readonly EasyEvent<T> onItemAdded = new EasyEvent<T>();
        private readonly EasyEvent<T> onItemRemoved = new EasyEvent<T>();
        private readonly EasyEvent onCleared = new EasyEvent();
        private readonly EasyEvent<int> onCountChanged = new EasyEvent<int>();
        #endregion

        #region 构造函数
        public BindableList() { }

        public BindableList(IEnumerable<T> collection)
        {
            if (collection == null) return;
            foreach (var item in collection)
                Items.Add(item);
        }
        #endregion

        #region 事件注册
        public IUnRegister RegisterOnItemAdded(Action<T> handler) => onItemAdded.Register(handler);
        public IUnRegister RegisterOnItemRemoved(Action<T> handler) => onItemRemoved.Register(handler);
        public IUnRegister RegisterOnCleared(Action handler) => onCleared.Register(handler);
        public IUnRegister RegisterOnCountChanged(Action<int> handler) => onCountChanged.Register(handler);
        #endregion

        #region Collection<T> 重写
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            onItemAdded.Trigger(item);
            onCountChanged.Trigger(Count);
        }

        protected override void RemoveItem(int index)
        {
            T item = this[index];
            base.RemoveItem(index);
            onItemRemoved.Trigger(item);
            onCountChanged.Trigger(Count);
        }

        protected override void ClearItems()
        {
            var beforeCount = Count;
            base.ClearItems();
            onCleared.Trigger();
            if (beforeCount > 0)
            {
                onCountChanged.Trigger(beforeCount);
            }
            
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = this[index];
            base.SetItem(index, item);
            onItemRemoved.Trigger(oldItem);
            onItemAdded.Trigger(item);
        }
        #endregion

        #region 批量操作
        /// <summary>
        /// 批量替换所有元素（用于存档导入等场景）
        /// 只触发 OnCleared + OnCountChanged，不触发逐项事件
        /// </summary>
        public void Reset(IEnumerable<T> newItems)
        {
            Items.Clear();
            if (newItems != null)
            {
                foreach (var item in newItems)
                    Items.Add(item);
            }
            onCleared.Trigger();
            onCountChanged.Trigger(Count);
        }
        #endregion
    }
}
