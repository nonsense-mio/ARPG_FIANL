using System.Collections.Generic;
using Framework;
using UnityEngine;
namespace ARPG
{
    /// <summary>
    /// 对象池系统实现 - 管理 GameObject 预制体池 + 非 MonoBehaviour 泛型池
    /// </summary>
    public class PoolSystem : AbstractSystem, IPoolSystem
    {
        #region GameObject 池 字段

        // 每种预制体对应一个对象池容器
        private Dictionary<string, PoolContainer> poolContainers;

        // 预制体缓存
        private Dictionary<string, GameObject> prefabs;
        //对象池根节点
        private GameObject poolRoot;
        //是否启用父节点管理功能
        private bool enableHierarchy = true;

        #endregion

        #region 泛型池 字段

        // 泛型对象池字典
        private Dictionary<string, GenericPoolBase> genericPools;

        #endregion

        protected override void OnInit()
        {
            poolContainers = new Dictionary<string, PoolContainer>();
            prefabs = new Dictionary<string, GameObject>();
            genericPools = new Dictionary<string, GenericPoolBase>();

            if (enableHierarchy)
            {
                poolRoot = new GameObject("PoolSystem");
                GameObject.DontDestroyOnLoad(poolRoot);
            }
        }

        #region GameObject 池 实现

        // 预热对象池，提前创建一定数量的对象并设置最大容量
        public void WarmupPool(string prefabPath, int count, int maxCapacity)
        {
            GameObject prefab = GetOrLoadPrefab(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"无法加载预制体: {prefabPath}");
                return;
            }

            PoolContainer container = GetOrCreateContainer(prefabPath);
            container.SetMaxCapacity(maxCapacity);

            for (int i = 0; i < count; i++)
            {
                GameObject obj = GameObject.Instantiate(prefab);
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
            else
            {
                // 从池中取出的对象：自动重置 Transform
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
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
        }

        // 清空指定的对象池
        public void ClearPool(string prefabPath)
        {
            if (!poolContainers.ContainsKey(prefabPath)) return;

            poolContainers[prefabPath].Clear();
            poolContainers.Remove(prefabPath);
            prefabs.Remove(prefabPath);
        }

        // 清空所有池 (GameObject 池 + 泛型池)
        public void ClearAllPools()
        {
            // 清理 GameObject 池
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

            // 重建 poolRoot 以支持后续 Spawn
            if (enableHierarchy)
            {
                poolRoot = new GameObject("PoolSystem");
                GameObject.DontDestroyOnLoad(poolRoot);
            }

            // 清理泛型池
            genericPools.Clear();
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

        #endregion

        #region 泛型池 实现

        /// <summary>
        /// 从泛型池获取对象，池中无可用对象时自动创建新实例
        /// </summary>
        public T Spawn<T>(string nameSpace = "") where T : class, IPoolObject, new()
        {
            string poolName = nameSpace + "_" + typeof(T).Name;

            if (genericPools.ContainsKey(poolName))
            {
                GenericPool<T> pool = genericPools[poolName] as GenericPool<T>;
                if (pool.poolObjs.Count > 0)
                {
                    return pool.poolObjs.Dequeue();
                }
            }

            return new T();
        }

        /// <summary>
        /// 将对象回收到泛型池中，回收前自动调用 ResetInfo()
        /// </summary>
        public void Recycle<T>(T obj, string nameSpace = "") where T : class, IPoolObject
        {
            if (obj == null) return;

            string poolName = nameSpace + "_" + typeof(T).Name;
            GenericPool<T> pool;

            if (genericPools.ContainsKey(poolName))
            {
                pool = genericPools[poolName] as GenericPool<T>;
            }
            else
            {
                pool = new GenericPool<T>();
                genericPools.Add(poolName, pool);
            }

            // 放入池子之前先重置对象的数据
            obj.ResetInfo();
            pool.poolObjs.Enqueue(obj);
        }

        #endregion

        #region 私有辅助

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

        #endregion

        #region 泛型池 内部类型

        /// <summary>
        /// 泛型池容器基类 (用于字典存储)
        /// </summary>
        private abstract class GenericPoolBase { }

        /// <summary>
        /// 泛型池容器 - 存储特定类型的可复用对象
        /// </summary>
        private class GenericPool<T> : GenericPoolBase where T : class
        {
            public Queue<T> poolObjs = new Queue<T>();
        }

        #endregion
    }
}
