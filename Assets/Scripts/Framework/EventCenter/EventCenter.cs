using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using XLua;
namespace HT
{
    /// <summary>
    /// 用于 里氏替换原则 装载子类的父类
    /// </summary>
    public abstract class EventInfoBase
    {

    }
    /// <summary>
    /// 用于包裹 对应观察者 函数委托的类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventInfo<T> : EventInfoBase
    {
        //真正的观察者 对应的函数信息 记录在其中
        public UnityAction<T> actions;
        public EventInfo(UnityAction<T> action)
        {
            actions += action;
        }
    }
    /// <summary>
    /// 用来记录无参无返回值委托
    /// </summary>
    public class EventInfo : EventInfoBase
    {
        public UnityAction actions;
        public EventInfo(UnityAction action)
        {
            actions += action;
        }
    }

    /// <summary>
    /// 事件中心模块
    /// </summary>
    public class EventCenter : BaseManager<EventCenter>
    {
        //用于记录对应事件
        private Dictionary<E_EventType, EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>();
        private EventCenter() { }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventName">事件名字</param>
        public void EventTrigger<T>(E_EventType eventName, T info)
        {
            //存在关联这件事的对象 执行逻辑
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
            }

        }
        /// <summary>
        /// 触发事件 无参数
        /// </summary>
        /// <param name="eventName"></param>
        public void EventTrigger(E_EventType eventName)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo).actions?.Invoke();
            }
        }
        /// <summary>
        /// 添加事件监听者
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="func"></param>
        public void AddEventListener<T>(E_EventType eventName, UnityAction<T> func)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo<T>).actions += func;
            }
            else
            {
                eventDic.Add(eventName, new EventInfo<T>(func));
            }

        }
        public void AddEventListener(E_EventType eventName, UnityAction func)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo).actions += func;
            }
            else
            {
                eventDic.Add(eventName, new EventInfo(func));
            }
        }
        /// <summary>
        /// 移除事件监听者
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="func"></param>
        public void RemoveEventListener<T>(E_EventType eventName, UnityAction<T> func)
        {
            if (eventDic.ContainsKey(eventName))
                (eventDic[eventName] as EventInfo<T>).actions -= func;
        }
        public void RemoveEventListener(E_EventType eventName, UnityAction func)
        {
            if (eventDic.ContainsKey(eventName))
                (eventDic[eventName] as EventInfo).actions -= func;
        }
        /// <summary>
        /// 清空所有事件监听
        /// </summary>
        public void Clear()
        {
            eventDic.Clear();
        }
        /// <summary>
        /// 清除指定事件监听
        /// </summary>
        /// <param name="eventName"></param>
        public void Clear(E_EventType eventName)
        {
            if (eventDic.ContainsKey(eventName))
                eventDic.Remove(eventName);
        }
    }

}
