using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器管理器 主要用于开启 停止 重置等操作来管理计时器
/// </summary>
public class TimerMgr :BaseManager<TimerMgr>
{
    /// <summary>
    /// 用于记录当前将要创建的唯一ID
    /// </summary>
    private int TIMER_KEY = 0;
    /// <summary>
    /// 用于存储管理所有计时器的字典容器
    /// </summary>
    private Dictionary<int,TimerItem> timerDic = new Dictionary<int,TimerItem>();
    /// <summary>
    ///  用于存储管理所有计时器的字典容器(不受Time.timeScale影响的计时器)
    /// </summary>
    private Dictionary<int, TimerItem> realTimerDic = new Dictionary<int, TimerItem>();
    /// <summary>
    /// 待移除列表
    /// </summary>
    private List<TimerItem> delList = new List<TimerItem>();

    private Coroutine timer;
    private Coroutine realTimer;

    /// <summary>
    /// 计时器管理器中的 计时衡量
    /// </summary>
    private const float intervalTimer = 0.1f;

    //声明为成员变量
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTimer);
    private WaitForSeconds waitForSeconds = new WaitForSeconds(intervalTimer);
    private TimerMgr()
    {
        //默认计时器管理器就是开启的
        Start();    
    }

    //开启计时器管理器
    public void Start()
    {
        timer = MonoMgr.Instance.StartCoroutine(StartTiming(false,timerDic));
        realTimer = MonoMgr.Instance.StartCoroutine(StartTiming(true,realTimerDic));
    }
    //关闭计时器管理器
    public void Stop()
    {
        MonoMgr.Instance.StopCoroutine(timer);
        MonoMgr.Instance.StopCoroutine(realTimer);
    }

   
    IEnumerator StartTiming(bool isRealTime,Dictionary<int,TimerItem> timerDic)
    {
        while (true)
        {
            //100毫秒 进行一次计时
            if(isRealTime)
                yield return waitForSecondsRealtime;
            else
                yield return waitForSeconds;
            //遍历所有的计时器进行更新
            foreach(TimerItem item in timerDic.Values)
            {
                if (!item.isRunning)
                    continue;
                //判断计时器是否有间隔时间执行的需求
                if(item.callBack != null)
                {
                    //减去100毫秒
                    item.intervalTime -= (int)(intervalTimer * 1000);
                    if(item.intervalTime <= 0)
                    {
                        item.callBack.Invoke();
                        //重置间隔时间
                        item.intervalTime = item.maxIntervalTime;
                    }

                }
                //总的时间更新
                item.allTime -= (int)(intervalTimer * 1000);
                if( item.allTime <= 0)
                {
                    item.overCallBack.Invoke();
                    //该计时器任务完成 放入待移除列表
                    delList.Add(item);
                }
            }

            //移除待移除列表中的数据
            foreach(TimerItem item in delList)
            {
                //将计时器从字典中移除
                timerDic.Remove(item.keyID);
                //放入缓存池中
                PoolMgr.Instance.PushObj<TimerItem>(item);
            }
            delList.Clear();
        }
    }
    /// <summary>
    /// 创建单个计时器
    /// </summary>
    /// /// <param name="isRealTime">如果是true 不受Time.timeScale影响 </param>
    /// <param name="allTime">总的时间</param>
    /// <param name="overCallBack">总时间结束回调</param>
    /// <param name="intervalTime">间隔时间</param>
    /// <param name="callBack">间隔时间结束回调</param>
    /// <returns>返回唯一ID 用于外部控制对应计时器</returns>
    public int CreateTimer(bool isRealTime,int allTime, UnityAction overCallBack, int intervalTime = 0, UnityAction callBack = null)
    {
        //构建唯一ID
        int keyID = ++TIMER_KEY;
        //从缓存池中取出对应的计时器
        TimerItem timerItem = PoolMgr.Instance.GetObj<TimerItem>();
        //初始化数据
        timerItem.InitInfo(keyID,allTime,overCallBack,intervalTime,callBack);
        //记录到字典中
        if(isRealTime)
            realTimerDic.Add(keyID, timerItem);
        else
            timerDic.Add(keyID, timerItem);
        return keyID;
    }
    /// <summary>
    /// 移除单个计时器
    /// </summary>
    /// <param name="keyID"></param>
    public void RemoveTimer(int keyID)
    {
        if( timerDic.ContainsKey(keyID))
        {
            //移除对应ID计时器 放入缓存池中
            PoolMgr.Instance.PushObj<TimerItem>(timerDic[keyID]);
            timerDic.Remove(keyID);
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            //移除对应ID计时器 放入缓存池中
            PoolMgr.Instance.PushObj<TimerItem>(realTimerDic[keyID]);
            realTimerDic.Remove(keyID);
        }
    }
    /// <summary>
    /// 重置单个计时器
    /// </summary>
    /// <param name="keyID"></param>
    public void ResetTimer(int keyID)
    {
        if( timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].ResetTimer();
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].ResetTimer();
        }
    }
    /// <summary>
    /// 开启单个计时器
    /// </summary>
    /// <param name="keyID"></param>
    public void StartTimer(int keyID)
    {
        if( timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRunning = true;
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].isRunning = true;
        }
    }
    /// <summary>
    /// 停止单个计时器
    /// </summary>
    /// <param name="keyID"></param>
    public void StopTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRunning = false;
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].isRunning = false;
        }
    }
}
