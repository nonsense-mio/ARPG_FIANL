using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// Resources资源加载实现 - 替代 ResMgr (BaseManager 单例)
    /// 直接委托 Unity Resources.Load
    /// </summary>
    public class ResourceSystem : IResourceSystem
    {
        public T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
    }
}
