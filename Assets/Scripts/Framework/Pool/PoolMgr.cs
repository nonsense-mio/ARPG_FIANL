using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 抽屉对象(池子中的数据)
/// </summary>
public class PoolData
{
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    private List<GameObject> usedList = new List<GameObject>();
    private int maxNum;
    private GameObject rootObj;
    private string drawerName;

    public int Count => dataStack.Count;
    public int UsedCount => usedList.Count;
    public bool NeedCreate => usedList.Count < maxNum;

    public PoolData(GameObject root, string name, GameObject usedObj)
    {
        drawerName = name;

        if (PoolMgr.isOpenLayout)
        {
            rootObj = new GameObject(name);
            rootObj.transform.SetParent(root.transform);
        }
        PushUsedList(usedObj);
        PoolObj poolObj = usedObj.GetComponent<PoolObj>();
        if (poolObj == null)
        {
            Debug.LogError("请为使用缓存池功能的预设体对象挂载PoolObj脚本 用于设置数量上限");
            return;
        }
        maxNum = poolObj.maxNum;
    }

    public GameObject Pop()
    {
        GameObject obj;
        if (Count > 0)
        {
            obj = dataStack.Pop();
            usedList.Add(obj);
        }
        else
        {
            obj = usedList[0];
            usedList.RemoveAt(0);
            usedList.Add(obj);
        }


        obj.SetActive(true);

        if (PoolMgr.isOpenLayout)
            obj.transform.SetParent(null, false);

        return obj;
    }

    /// <summary>
    /// 将物体放入到抽屉对象中
    /// </summary>
    /// <param name="obj">要放入的对象</param>
    /// <param name="poolRoot">柜子根对象，用于重建抽屉</param>
    public void Push(GameObject obj, GameObject poolRoot)
    {
        //放入池子之前 先重置对象的数据
        var poolObj = obj.GetComponent<IPoolObject>();
        poolObj?.ResetInfo();
        obj.SetActive(false);
        if (PoolMgr.isOpenLayout)
        {
            // 检查抽屉父对象是否被销毁，如果是则重建
            if (rootObj == null)
            {
                rootObj = new GameObject(drawerName);
                rootObj.transform.SetParent(poolRoot.transform);
            }
            obj.transform.SetParent(rootObj.transform);
        }
        dataStack.Push(obj);
        usedList.Remove(obj);
    }

    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}


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
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    //值代表的是一个抽屉对象
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    /// <summary>
    /// 用于存储 数据结构类、逻辑类对象 的 池子的字典容器
    /// </summary>
    private Dictionary<string, PoolObjectBase> poolObjectDic = new Dictionary<string, PoolObjectBase>();

    //池子根对象
    private GameObject poolObj;
    //是否开启布局功能
    public static bool isOpenLayout = true;
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
    /// 拿东西的方法
    /// </summary>
    /// <param name="name">抽屉容器的名字</param>
    /// <returns>从缓存池中取出的对象</returns>
    public GameObject GetObj(string name)
    {
        GameObject obj;
        if (poolObj == null && isOpenLayout)
        {
            poolObj = new GameObject("Pool");
        }


        if (!poolDic.ContainsKey(name) || (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
        {
            //通过资源加载 实例化出一个GameObject
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;

            //创建抽屉
            if (!poolDic.ContainsKey(name))
                poolDic.Add(name, new PoolData(poolObj, name, obj));
            else
                poolDic[name].PushUsedList(obj);
        }
        else
        {
            obj = poolDic[name].Pop();
        }



        return obj;
    }

    /// <summary>
    /// 往缓存池中放入对象
    /// </summary>
    /// <param name="obj">希望放入的对象</param>
    public void PushObj(GameObject obj)
    {
        // 确保 poolObj 存在
        if (poolObj == null && isOpenLayout)
        {
            poolObj = new GameObject("Pool");
        }
        if (!poolDic.ContainsKey(obj.name))
        {
            Debug.LogWarning($"缓存池中不存在名为 {obj.name} 的抽屉，无法放入对象。请确保该对象是通过 PoolMgr 获取的。");
            return;
        }
        poolDic[obj.name].Push(obj, poolObj);
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
    /// 用于清除整个柜子当中的数据(清除缓存池)
    /// 使用场景主要是 切场景时
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        // 销毁根对象，确保下次能重新创建
        if (poolObj != null)
        {
            GameObject.Destroy(poolObj);
            poolObj = null;
        }
        poolObjectDic.Clear();

    }
}
