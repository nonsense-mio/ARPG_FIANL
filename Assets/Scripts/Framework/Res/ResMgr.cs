using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源信息基类 主要用于里氏替换原则 父类容器装子类对象
/// </summary>
public abstract class ResInfoBase
{
    //引用计数
    public int refCount;
}

/// <summary>
/// 资源信息对象 主要用于存储资源信息 异步加载委托信息 异步加载协程信息
/// </summary>
/// <typeparam name="T">资源类型</typeparam>
public class ResInfo<T> : ResInfoBase
{
    //资源
    public T asset;
    //主要用于异步加载结束后 传递资源到外部的委托
    public UnityAction<T> callBack;
    //用于存储异步加载时 开启的协程
    public Coroutine coroutine;
    //决定引用计数为0时  是否真正需要移除
    public bool isDel;
   

    public void AddRefCount()
    {
        refCount++;
    }
    public void SubRefCound()
    {
        refCount--;
        if (refCount < 0)
            Debug.LogError("引用计数小于0了，请检查使用和卸载是否配对执行");
    }
}

/// <summary>
/// Resources资源加载模块管理器
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    //用于存储加载过 或加载中 的资源的 容器
    private Dictionary<string,ResInfoBase> resDic = new Dictionary<string,ResInfoBase>();
    private ResMgr() { }

    /// <summary>
    /// 同步加载Resources下资源的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> info;
        //字典中不存在资源时
        if (!resDic.ContainsKey(resName))
        {
            //同步加载并记录信息到字典中
            T res = Resources.Load<T>(path);
            info = new ResInfo<T>();
            info.asset = res;
            //引用计数增加
            info.AddRefCount();
            resDic.Add(resName, info);
            return res;
        }
        else
        {
            //取出字典中的记录
            info = resDic[resName] as ResInfo<T>;
            //引用计数增加
            info.AddRefCount();
            //存在异步加载 还在加载中
            if (info.asset == null)
            {
                //停止异步加载 
                MonoMgr.Instance.StopCoroutine(info.coroutine);
                //直接采用同步的方式加载 
                T res = Resources.Load<T>(path);
                //记录
                info.asset = res;
                //把那些等待着异步加载结束的委托执行了
                info.callBack?.Invoke(res);
                //清空引用
                info.callBack = null;
                info.coroutine = null;
                //使用
                return res;
            }
            //如果加载结束直接用
            else
            {
                return info.asset;
            }
        }
        
    }


    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径(Resources下的)</param>
    /// <param name="callBack">加载结束后的回调函数 当异步加载资源结束后才会调用</param>
    public void LoadAsync<T>(string path,UnityAction<T> callBack) where T :UnityEngine.Object
    {
        //资源的唯一ID 路径名_资源类型
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> info;
        //如果字典中不存在该资源
        if (!resDic.ContainsKey(resName))
        {
            //声明一个资源信息对象
            info = new ResInfo<T>();
            //引用计数增加
            info.AddRefCount();
            //将资源记录添加到字典中
            resDic.Add(resName, info);
            //记录传入的委托函数 加载完成了再使用
            info.callBack += callBack;
            //开启协程 异步加载资源 并且 记录协同程序(用于之后可能的停止)
            info.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
        //字典中存在资源
        else
        {
            //从字典中取出资源信息
            info = resDic[resName] as ResInfo<T>;
            //引用计数增加
            info.AddRefCount();
            //如果资源还没加载完
            //意味着还在进行异步加载
            if (info.asset == null)
            {
                info.callBack += callBack;
            }
            else
            {
                callBack?.Invoke(info.asset);
            }
        }
        //要通过协程 异步加载资源
        //MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path, callBack));
    }
    private IEnumerator ReallyLoadAsync<T>(string path)where T : UnityEngine.Object
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync<T>(path);
        //等待资源结束后 才会继续执行yield return 后面的代码
        yield return rq;
        //资源的唯一ID 路径名_资源类型
        string resName = path + "_" + typeof(T).Name;
        //资源加载结束 将资源传到外部的委托函数进行使用
        if (resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            //取出资源信息 并且记录加载完成的资源
            resInfo.asset = rq.asset as T;
            //引用计数为0时 再卸载
            if (resInfo.refCount == 0)
            {
                UnloadAsset<T>(path,resInfo.isDel,null,false);
            }
            else
            {
                //将加载完成的资源传递出去
                resInfo.callBack?.Invoke(resInfo.asset);
                //加载完毕后 这些引用就可以清空 避免引用的占用 可能带来的潜在的内存泄露问题
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
               
        }

    }

    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <param name="path">资源路径(Resources下的)</param>
    /// <param name="callBack">加载结束后的回调函数 当异步加载资源结束后才会调用</param>
    [Obsolete("建议使用泛型加载方式 不要两种方法混用")]
    public void LoadAsync(string path,Type type, UnityAction<UnityEngine.Object> callBack)
    {
        //资源的唯一ID 路径名_资源类型
        string resName = path + "_" + type.Name;
        ResInfo<UnityEngine.Object> info;
        //如果字典中不存在该资源
        if (!resDic.ContainsKey(resName))
        {
            //声明一个资源信息对象
            info = new ResInfo<UnityEngine.Object>();
            //引用计数增加
            info.AddRefCount();
            //将资源记录添加到字典中
            resDic.Add(resName, info);
            //记录传入的委托函数 加载完成了再使用
            info.callBack += callBack;
            //开启协程 异步加载资源 并且 记录协同程序(用于之后可能的停止)
            info.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path,type));
        }
        //字典中存在资源
        else
        {
            //从字典中取出资源信息
            info = resDic[resName] as ResInfo<UnityEngine.Object>;
            //引用计数增加
            info.AddRefCount();
            //如果资源还没加载完
            //意味着还在进行异步加载
            if (info.asset == null)
            {
                info.callBack += callBack;
            }
            else
            {
                callBack?.Invoke(info.asset);
            }
        }
    }
    private IEnumerator ReallyLoadAsync(string path,Type type)
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync(path,type);
        //等待资源结束后 才会继续执行yield return 后面的代码
        yield return rq;
        //资源的唯一ID 路径名_资源类型
        string resName = path + "_" + type.Name;
        //资源加载结束 将资源传到外部的委托函数进行使用
        if (resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //取出资源信息 并且记录加载完成的资源
            resInfo.asset = rq.asset;
            if (resInfo.refCount == 0)
            {
                UnloadAsset(path,type,resInfo.isDel,null,false);
            }
            else
            {
                //将加载完成的资源传递出去
                resInfo.callBack?.Invoke(resInfo.asset);
                //加载完毕后 这些引用就可以清空 避免引用的占用 可能带来的潜在的内存泄露问题
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
             
        }
    }

    /// <summary>
    /// 指定卸载一个资源
    /// </summary>
    /// <param name="assetToUnload"></param>
    public void UnloadAsset<T>(string path,bool isDel = false, UnityAction<T> callBack = null,bool isSub = true)
    {
        string resName = path + "_" + typeof(T).Name;
        //判断是否存在对应资源
        if (resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            //引用计数-1
            if(isSub)
                resInfo.SubRefCound();
            //记录 引用计数为0时 是否马上移除标签
            resInfo.isDel = isDel;
            //资源加载结束
            if(resInfo.asset != null && resInfo.refCount==0 && resInfo.isDel)
            {
                //从字典中移除
                resDic.Remove(resName);
                //卸载资源
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
            }
            //资源异步加载中
            else if(resInfo.asset == null)
            {
                //改变标识 待删除
                //resInfo.isDel = true;
                if(callBack != null)
                    resInfo.callBack -= callBack;
            }
            
        }
    }
    public void UnloadAsset(string path,Type type, bool isDel = false, UnityAction<UnityEngine.Object> callBack = null,bool isSub = true)
    {
        string resName = path + "_" + type.Name;
        //判断是否存在对应资源
        if (resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //引用计数-1
            if(isSub)
                resInfo.SubRefCound();
            //记录 引用计数为0时 是否马上移除标签
            resInfo.isDel = isDel;
            //资源加载结束
            if (resInfo.asset != null && resInfo.refCount==0 && resInfo.isDel)
            {
                //从字典中移除
                resDic.Remove(resName);
                //卸载资源
                Resources.UnloadAsset(resInfo.asset);
            }
            //资源异步加载中
            else if(resInfo.asset == null)
            {
                //改变标识 待删除
                //resInfo.isDel = true;
                if (callBack != null)
                    resInfo.callBack -= callBack;
            }

        }
    }
    /// <summary>
    /// 异步卸载没使用的Resources相关资源
    /// </summary>
    /// <param name="callBack">回调函数</param>

    public void UnloadUnusedAssets(UnityAction callBack)
    {
        //在移除不使用的资源前 先把字典里记录的引用计数为0的 且没有被移除记录的资源移除掉
        List<string> list = new List<string>();
        foreach(string path in resDic.Keys)
        {
            if(resDic[path].refCount == 0)
                list.Add(path);
        }
        foreach(string path in list)
        {
            resDic.Remove(path);
        }
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAssets(callBack));
    }
   

    private IEnumerator ReallyUnloadUnusedAssets(UnityAction callBack)
    {
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        //卸载完毕后通知外部
        callBack();
    }

    /// <summary>
    /// 获取当前某个资源的引用计数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public int GetRefCount<T>(string path)
    {
        string resName = path + "_" + typeof(T).Name;

        if (resDic.ContainsKey(resName))
        {
            return (resDic[resName] as ResInfo<T>).refCount;
        }
        return 0;
    }

    /// <summary>
    /// 清空字典
    /// </summary>
    /// <param name="callBack"></param>
    public void ClearDic(UnityAction callBack)
    {
        resDic.Clear();
        MonoMgr.Instance.StartCoroutine(ReallyClearDic(callBack));
    }
    private IEnumerator ReallyClearDic(UnityAction callBack)
    {
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        //卸载完毕后通知外部
        callBack();
    }

}
