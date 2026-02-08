using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// Resources资源加载抽象接口 - 替代 ResMgr (BaseManager 单例)
    /// 封装 Unity Resources.Load 同步加载
    /// </summary>
    public interface IResourceSystem : Framework.IUtility
    {
        T Load<T>(string path) where T : Object;
    }
}
