// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public class PoolSystem //: AbstractSystem
// {
//     // 每种预制体对应一个对象池
//     private Dictionary<string, Queue<GameObject>> pools;

//     // 记录每个对象属于哪个池
//     private Dictionary<GameObject, string> objectToPool;

//     // 预制体缓存
//     private Dictionary<string, GameObject> prefabs;

//     protected override void OnInit()
//     {
//         pools = new Dictionary<string, Queue<GameObject>>();
//         objectToPool = new Dictionary<GameObject, string>();
//         prefabs = new Dictionary<string, GameObject>();

//         // 监听场景切换事件，清理对象池
//         this.RegisterEvent<SceneChangedEvent>(OnSceneChanged);
//     }

//     // 预热对象池，提前创建一定数量的对象
//     public void WarmupPool(string prefabPath, int count)
//     {
//         // 加载预制体
//         GameObject prefab = GetOrLoadPrefab(prefabPath);
//         if (prefab == null) return;

//         // 如果池不存在，创建它
//         if (!pools.ContainsKey(prefabPath))
//         {
//             pools[prefabPath] = new Queue<GameObject>();
//         }

//         // 创建指定数量的对象
//         for (int i = 0; i < count; i++)
//         {
//             GameObject obj = GameObject.Instantiate(prefab);
//             obj.SetActive(false);
//             pools[prefabPath].Enqueue(obj);
//             objectToPool[obj] = prefabPath;
//         }
//     }

//     // 从对象池获取对象
//     public GameObject Spawn(string prefabPath, Vector3 position, Quaternion rotation)
//     {
//         GameObject obj;

//         // 检查池中是否有可用对象
//         if (pools.ContainsKey(prefabPath) && pools[prefabPath].Count > 0)
//         {
//             // 从池中取出对象
//             obj = pools[prefabPath].Dequeue();
//         }
//         else
//         {
//             // 池中没有可用对象，创建新的
//             GameObject prefab = GetOrLoadPrefab(prefabPath);
//             if (prefab == null) return null;

//             obj = GameObject.Instantiate(prefab);
//             objectToPool[obj] = prefabPath;
//         }

//         // 设置对象的位置和旋转
//         obj.transform.position = position;
//         obj.transform.rotation = rotation;
//         obj.SetActive(true);

//         // 通知对象被生成，对象可以监听这个来初始化自己
//         var poolable = obj.GetComponent<IPoolable>();
//         poolable?.OnSpawn();

//         // 发送对象生成事件
//         //this.SendEvent(new ObjectSpawnedEvent { Obj = obj, PrefabPath = prefabPath });

//         return obj;
//     }

//     // 将对象回收到池中
//     public void Recycle(GameObject obj)
//     {
//         if (!objectToPool.ContainsKey(obj))
//         {
//             // 这个对象不属于任何池，直接销毁
//             GameObject.Destroy(obj);
//             return;
//         }

//         string poolName = objectToPool[obj];

//         // 通知对象被回收，对象可以在这里清理状态
//         var poolable = obj.GetComponent<IPoolable>();
//         poolable?.OnRecycle();

//         // 禁用对象并放回池中
//         obj.SetActive(false);
//         pools[poolName].Enqueue(obj);

//         // 发送对象回收事件
//         //this.SendEvent(new ObjectRecycledEvent { Obj = obj, PrefabPath = poolName });
//     }

//     // 延迟回收对象
//     public void RecycleAfterDelay(GameObject obj, float delay)
//     {
//         // 这里需要一个协程，但 System 不是 MonoBehaviour
//         // 可以通过其他方式实现，比如使用定时器系统
//         //this.GetSystem<TimerSystem>().Schedule(delay, () => Recycle(obj));
//     }

//     // 清空指定的对象池
//     public void ClearPool(string prefabPath)
//     {
//         if (!pools.ContainsKey(prefabPath)) return;

//         var pool = pools[prefabPath];
//         while (pool.Count > 0)
//         {
//             GameObject obj = pool.Dequeue();
//             objectToPool.Remove(obj);
//             GameObject.Destroy(obj);
//         }
//     }

//     // 清空所有对象池
//     public void ClearAllPools()
//     {
//         foreach (var poolName in pools.Keys.ToList())
//         {
//             ClearPool(poolName);
//         }
//     }

//     // 场景切换时的处理
//     // private void OnSceneChanged(SceneChangedEvent e)
//     // {
//     //     // 根据配置决定是否清空对象池
//     //     // 某些全局的对象池可能需要保留
//     //     if (e.ShouldClearPools)
//     //     {
//     //         ClearAllPools();
//     //     }
//     // }

//     // 加载或获取缓存的预制体
//     private GameObject GetOrLoadPrefab(string prefabPath)
//     {
//         if (prefabs.ContainsKey(prefabPath))
//         {
//             return prefabs[prefabPath];
//         }

//         GameObject prefab = Resources.Load<GameObject>(prefabPath);
//         if (prefab != null)
//         {
//             prefabs[prefabPath] = prefab;
//         }

//         return prefab;
//     }
// }

// // 可池化对象的接口
// public interface IPoolable
// {
//     void OnSpawn();   // 从池中取出时调用
//     void OnRecycle(); // 回收到池中时调用
// }