using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 对象池系统接口 - QFramework System层
    /// 职责: 管理 GameObject 预制体池 + 非 MonoBehaviour 泛型池
    /// </summary>
    public interface IPoolSystem : ISystem
    {
        #region GameObject 池
        /// <summary>
        /// 从对象池获取 GameObject
        /// </summary>
        GameObject Spawn(string prefabPath);

        /// <summary>
        /// 从对象池获取 GameObject，激活前先设置到指定位置和朝向
        /// 推荐用于含 CharacterController 的对象，确保 CC 在正确位置完成初始化
        /// </summary>
        GameObject Spawn(string prefabPath, Vector3 position, Quaternion rotation);

        /// <summary>
        /// 将 GameObject 回收到池中
        /// </summary>
        void Recycle(GameObject obj);

        /// <summary>
        /// 预热对象池，提前创建一定数量的对象
        /// </summary>
        void WarmupPool(string prefabPath, int count, int maxCapacity);

        /// <summary>
        /// 清空指定的 GameObject 池
        /// </summary>
        void ClearPool(string prefabPath);

        /// <summary>
        /// 清空所有池 (GameObject 池 + 泛型池)
        /// </summary>
        void ClearAllPools();

        /// <summary>
        /// 获取指定 GameObject 池的统计信息
        /// </summary>
        PoolStats GetPoolStats(string prefabPath);
        #endregion

        #region 泛型池 (非 MonoBehaviour 对象)
        /// <summary>
        /// 从泛型池获取对象，池中无可用对象时自动创建新实例
        /// </summary>
        T Spawn<T>(string nameSpace = "") where T : class, IPoolObject, new();

        /// <summary>
        /// 将对象回收到泛型池中，回收前自动调用 ResetInfo()
        /// </summary>
        void Recycle<T>(T obj, string nameSpace = "") where T : class, IPoolObject;
        #endregion
    }

    /// <summary>
    /// 想要被泛型池复用的 数据结构类、逻辑类 必须实现此接口
    /// </summary>
    public interface IPoolObject
    {
        /// <summary>
        /// 重置数据，在回收到池中前调用
        /// </summary>
        void ResetInfo();
    }

    /// <summary>
    /// 可池化 GameObject 的接口 (挂载在预制体上的脚本实现)
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// 从池中取出时调用
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// 回收到池中时调用
        /// </summary>
        void OnRecycle();
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
}
