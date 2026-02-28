using System.Collections.Generic;
using UnityEngine;
namespace ARPG
{
    /// <summary>
    /// 对象池容器 - 封装单个对象池的所有逻辑
    /// 包含父节点管理和LRU缓存功能
    /// </summary>
    public class PoolContainer
    {
        // 可用对象队列(未使用的对象)
        private Queue<GameObject> availableObjects;

        // 正在使用的对象列表(用于LRU管理)
        private LinkedList<GameObject> activeObjects;

        // 对象到链表节点的映射(用于快速查找)
        private Dictionary<GameObject, LinkedListNode<GameObject>> objectToNode;

        // 容器名称
        private string containerName;

        // 最大容量(0表示无限制)
        private int maxCapacity;

        // 容器的父节点(用于组织层级)
        private GameObject containerRoot;

        public PoolContainer(string name, Transform parent)
        {
            this.containerName = name;

            availableObjects = new Queue<GameObject>();
            activeObjects = new LinkedList<GameObject>();
            objectToNode = new Dictionary<GameObject, LinkedListNode<GameObject>>();

            // 如果提供了父节点,创建容器根对象
            if (parent != null)
            {
                containerRoot = new GameObject($"Pool_{name}");
                containerRoot.transform.SetParent(parent, false);
            }
        }

        /// <summary>
        /// 设置最大容量 (0=无限制)
        /// </summary>
        public void SetMaxCapacity(int max)
        {
            maxCapacity = max;
        }

        /// <summary>
        /// 预热对象(将对象放入可用队列)
        /// </summary>
        public void PrewarmObject(GameObject obj)
        {
            obj.SetActive(false);

            if (containerRoot != null)
            {
                obj.transform.SetParent(containerRoot.transform, false);
            }

            availableObjects.Enqueue(obj);
        }

        /// <summary>
        /// 生成对象
        /// </summary>
        public GameObject Spawn()
        {
            GameObject obj = null;

            // 优先从可用队列中获取
            if (availableObjects.Count > 0)
            {
                obj = availableObjects.Dequeue();
            }
            // 如果没有可用对象且达到最大容量,使用LRU策略复用最久未使用的对象
            else if (maxCapacity > 0 && activeObjects.Count >= maxCapacity)
            {
                obj = RecycleLRUObject();
            }

            // 如果获取到对象,添加到活动列表
            if (obj != null)
            {
                AddToActiveList(obj);
            }

            return obj;
        }

        /// <summary>
        /// 注册新创建的对象
        /// </summary>
        public void RegisterNewObject(GameObject obj)
        {
            AddToActiveList(obj);
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Recycle(GameObject obj)
        {
            // 从活动列表中移除
            if (objectToNode.TryGetValue(obj, out LinkedListNode<GameObject> node))
            {
                activeObjects.Remove(node);
                objectToNode.Remove(obj);
            }

            // 放入可用队列
            availableObjects.Enqueue(obj);
        }

        /// <summary>
        /// 清空容器
        /// </summary>
        public void Clear()
        {
            // 销毁所有可用对象
            while (availableObjects.Count > 0)
            {
                GameObject obj = availableObjects.Dequeue();
                if (obj != null)
                {
                    GameObject.Destroy(obj);
                }
            }

            // 销毁所有活动对象
            foreach (var obj in activeObjects)
            {
                if (obj != null)
                {
                    GameObject.Destroy(obj);
                }
            }

            activeObjects.Clear();
            objectToNode.Clear();

            // 销毁容器根节点
            if (containerRoot != null)
            {
                GameObject.Destroy(containerRoot);
                containerRoot = null;
            }
        }

        /// <summary>
        /// 获取容器根节点
        /// </summary>
        public Transform GetContainerRoot()
        {
            return containerRoot?.transform;
        }

        /// <summary>
        /// 获取统计信息
        /// </summary>
        public PoolStats GetStats()
        {
            return new PoolStats
            {
                AvailableCount = availableObjects.Count,
                ActiveCount = activeObjects.Count,
                TotalCount = availableObjects.Count + activeObjects.Count,
                MaxCapacity = maxCapacity
            };
        }

        // 将对象添加到活动列表(最近使用)
        private void AddToActiveList(GameObject obj)
        {
            // 如果对象已在列表中,先移除(更新使用时间)
            if (objectToNode.TryGetValue(obj, out LinkedListNode<GameObject> existingNode))
            {
                activeObjects.Remove(existingNode);
                objectToNode.Remove(obj);
            }

            // 添加到列表末尾(表示最近使用)
            LinkedListNode<GameObject> newNode = activeObjects.AddLast(obj);
            objectToNode[obj] = newNode;
        }

        // 使用LRU策略回收最久未使用的对象
        private GameObject RecycleLRUObject()
        {
            if (activeObjects.Count == 0) return null;

            // 获取最久未使用的对象(链表头部)
            GameObject lruObject = activeObjects.First.Value;

            // 强制回收这个对象
            var poolable = lruObject.GetComponent<IPoolable>();
            poolable?.OnRecycle();

            lruObject.SetActive(false);

            // 从活动列表移除
            activeObjects.RemoveFirst();
            objectToNode.Remove(lruObject);

            return lruObject;
        }
    }
}