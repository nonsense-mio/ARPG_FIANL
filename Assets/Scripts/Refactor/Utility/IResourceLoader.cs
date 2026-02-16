using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// Resources资源加载抽象接口 - Utility层
    /// 封装 Unity Resources.Load 同步加载
    /// </summary>
    public interface IResourceLoader : Framework.IUtility
    {
        T Load<T>(string path) where T : Object;
    }
}
