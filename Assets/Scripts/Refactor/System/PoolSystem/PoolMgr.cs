using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 作为PoolObject<T>的父类 父类容器装载子类对象
/// </summary>
public abstract class PoolObjectBase
{

}

/// <summary>
/// 用于存储 (不继承Mono的)数据结构类 和 逻辑类 的容器类
/// </summary>
/// <typeparam name="T"></typeparam>
public class PoolObject<T> : PoolObjectBase where T : class
{
    public Queue<T> poolObjs = new Queue<T>();
}
/// <summary>
/// 想要被复用的 数据结构类、逻辑类 都必须继承该接口
/// </summary>
public interface IPoolObject
{
    /// <summary>
    /// 重置数据的方法
    /// </summary>
    void ResetInfo();
}

/// <summary>
/// 缓存池(对象池)模块 管理器
/// 仅保留非MonoBehaviour的泛型池化功能
/// GameObject池化已迁移至 PoolSystem (QFramework)
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    /// <summary>
    /// 用于存储 数据结构类、逻辑类对象 的 池子的字典容器
    /// </summary>
    private Dictionary<string, PoolObjectBase> poolObjectDic = new Dictionary<string, PoolObjectBase>();

    private PoolMgr()
    {

    }

    /// <summary>
    /// 获取自定义的数据结构类和逻辑类对象 (不继承Mono的)
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <returns></returns>
    public T GetObj<T>(string nameSpace = "") where T : class, IPoolObject, new()
    {
        //池子的名字根据类的类型决定
        string poolName = nameSpace + "_" + typeof(T).Name;
        //有池子
        if (poolObjectDic.ContainsKey(poolName))
        {
            PoolObject<T> pool = poolObjectDic[poolName] as PoolObject<T>;
            //池子中是否有可以复用的内容
            if (pool.poolObjs.Count > 0)
            {
                //从队列中取出对象进行复用
                T obj = pool.poolObjs.Dequeue();
                return obj;
            }
            //池子中没有可以复用的内容
            else
            {
                //必须保证存在无参构造函数
                T obj = new T();
                return obj;
            }
        }
        //没有池子
        else
        {
            T obj = new T();
            return obj;
        }
    }

    /// <summary>
    /// 将自定义数据结构类和逻辑类放入池子中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="nameSpace"></param>
    public void PushObj<T>(T obj, string nameSpace = "") where T : class, IPoolObject
    {
        if (obj == null)
            return;
        //池子的名字根据类的类型决定
        string poolName = nameSpace + "_" + typeof(T).Name;
        PoolObject<T> pool;
        //有池子
        if (poolObjectDic.ContainsKey(poolName))
        {
            //取出池子 压入对象
            pool = poolObjectDic[poolName] as PoolObject<T>;
        }
        //没有池子
        else
        {
            pool = new PoolObject<T>();
            poolObjectDic.Add(poolName, pool);
        }
        //放入池子之前 先重置对象的数据
        obj.ResetInfo();
        pool.poolObjs.Enqueue(obj);
    }

    /// <summary>
    /// 清除泛型池中的数据
    /// </summary>
    public void ClearPool()
    {
        poolObjectDic.Clear();
    }
}
