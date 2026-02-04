using System.Collections.Generic;
using UnityEngine;

public class PoolSystem //: AbstractSystem
{
    // 每种预制体对应一个对象池容器
    private Dictionary<string, PoolContainer> poolContainers;

    // 预制体缓存
    private Dictionary<string, GameObject> prefabs;
    //对象池根节点
    private GameObject poolRoot;
    //是否启用父节点管理功能
    private bool enableHierarchy = true;

    protected void OnInit()
    {
        poolContainers = new Dictionary<string, PoolContainer>();
        prefabs = new Dictionary<string, GameObject>();

        if (enableHierarchy)
        {
            poolRoot = new GameObject("PoolSystem");
            GameObject.DontDestroyOnLoad(poolRoot);
        }

        // 监听场景切换事件，清理对象池
        //this.RegisterEvent<SceneChangedEvent>(OnSceneChanged);
    }

    // 预热对象池，提前创建一定数量的对象
    public void WarmupPool(string prefabPath, int count)
    {
        // 加载预制体
        GameObject prefab = GetOrLoadPrefab(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"无法加载预制体: {prefabPath}");
            return;
        }
        // 获取或创建对象池容器
        PoolContainer container = GetOrCreateContainer(prefabPath);

        // 预热指定数量的对象
        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            //游戏对象名即为对象池字典键名
            obj.name = prefabPath;
            container.PrewarmObject(obj);
        }
    }

    // 从对象池获取对象
    public GameObject Spawn(string prefabPath)
    {
        // 如果对象池容器不存在
        if (!poolContainers.ContainsKey(prefabPath))
        {
            // 获取对象池 对象的缓存 
            GameObject prefab = GetOrLoadPrefab(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"无法加载预制体: {prefabPath}");
                return null;
            }

            //创建对象池容器 
            GetOrCreateContainer(prefabPath);
        }

        PoolContainer container = poolContainers[prefabPath];
        GameObject obj = container.Spawn();

        // 如果池中没有可用对象,创建新的
        if (obj == null)
        {
            GameObject prefab = GetOrLoadPrefab(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"无法加载预制体: {prefabPath}");
                return null;
            }
            obj = GameObject.Instantiate(prefab);
            obj.name = prefabPath;//游戏对象名即为对象池字典键名
            //添加到容器的活动对象中
            container.RegisterNewObject(obj);
        }
        //激活对象并移除父节点
        obj.SetActive(true);
        if (enableHierarchy)
        {
            obj.transform.SetParent(null, false);
        }
        // 通知对象被生成，对象可以监听这个来初始化自己
        var poolable = obj.GetComponent<IPoolable>();
        poolable?.OnSpawn();

        // 发送对象生成事件
        //this.SendEvent(new ObjectSpawnedEvent { Obj = obj, PrefabPath = prefabPath });

        return obj;
    }

    // 将对象回收到池中
    public void Recycle(GameObject obj)
    {
        if (obj == null) return;
        string prefabPath = obj.name;
        if (!poolContainers.ContainsKey(prefabPath))
        {
            Debug.LogWarning($"对象 {prefabPath} 不属于任何对象池,将直接销毁");
            GameObject.Destroy(obj);
            return;
        }

        // 通知对象被回收，对象可以在这里清理状态
        var poolable = obj.GetComponent<IPoolable>();
        poolable?.OnRecycle();

        // 禁用对象
        obj.SetActive(false);
        // 放入父节点
        if (enableHierarchy)
        {
            PoolContainer container = poolContainers[prefabPath];
            obj.transform.SetParent(container.GetContainerRoot(), false);
        }
        // 回收到池中
        poolContainers[prefabPath].Recycle(obj);
        // 发送对象回收事件
        //this.SendEvent(new ObjectRecycledEvent { Obj = obj, PrefabPath = poolName });
    }

    // 延迟回收对象
    public void RecycleAfterDelay(GameObject obj, float delay)
    {
        // 这里需要一个协程，但 System 不是 MonoBehaviour
        // 可以通过其他方式实现，比如使用定时器系统
        //this.GetSystem<TimerSystem>().Schedule(delay, () => Recycle(obj));
    }

    // 清空指定的对象池
    public void ClearPool(string prefabPath)
    {
        if (!poolContainers.ContainsKey(prefabPath)) return;

        poolContainers[prefabPath].Clear();
        poolContainers.Remove(prefabPath);
        prefabs.Remove(prefabPath);
    }

    // 清空所有对象池
    public void ClearAllPools()
    {
        foreach (var container in poolContainers.Values)
        {
            container.Clear();
        }

        poolContainers.Clear();
        prefabs.Clear();

        if (poolRoot != null)
        {
            GameObject.Destroy(poolRoot);
            poolRoot = null;
        }
    }

    // 场景切换时的处理
    // private void OnSceneChanged(SceneChangedEvent e)
    // {
    //     // 根据配置决定是否清空对象池
    //     // 某些全局的对象池可能需要保留
    //     if (e.ShouldClearPools)
    //     {
    //         ClearAllPools();
    //     }
    // }

    // 加载或获取缓存的预制体
    private GameObject GetOrLoadPrefab(string prefabPath)
    {
        if (prefabs.ContainsKey(prefabPath))
        {
            return prefabs[prefabPath];
        }

        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab != null)
        {
            prefabs[prefabPath] = prefab;
        }

        return prefab;
    }

    // 获取或创建对象池容器
    private PoolContainer GetOrCreateContainer(string prefabPath)
    {
        //如果字典中不存在该对象池容器 则 new 一个
        if (!poolContainers.ContainsKey(prefabPath))
        {
            Transform containerParent = enableHierarchy ? poolRoot.transform : null;
            poolContainers[prefabPath] = new PoolContainer(prefabPath, containerParent);
        }

        return poolContainers[prefabPath];
    }

    /// <summary>
    /// 获取对象池统计信息
    /// </summary>
    public PoolStats GetPoolStats(string prefabPath)
    {
        if (!poolContainers.ContainsKey(prefabPath))
        {
            return new PoolStats();
        }

        return poolContainers[prefabPath].GetStats();
    }
}



/// <summary>
/// 对象池统计信息
/// </summary>
public struct PoolStats
{
    public int AvailableCount;  // 可用对象数量
    public int ActiveCount;     // 活动对象数量
    public int TotalCount;      // 总对象数量
    public int MaxCapacity;     // 最大容量
}

// 可池化对象的接口
public interface IPoolable
{
    void OnSpawn();   // 从池中取出时调用
    void OnRecycle(); // 回收到池中时调用
}