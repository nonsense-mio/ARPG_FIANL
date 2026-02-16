using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// Resources资源加载实现
    /// 直接委托 Unity Resources.Load
    /// </summary>
    public class ResourceLoader : IResourceLoader
    {
        public T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
    }
}
