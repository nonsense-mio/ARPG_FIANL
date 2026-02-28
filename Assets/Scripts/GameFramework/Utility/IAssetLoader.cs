using Framework;
using UnityEngine;
using UnityEngine.Events;

namespace ARPG
{
    /// <summary>
    /// 资源加载抽象接口 - Utility层
    /// 通过 key (如 "character/Player") 加载资源，与具体加载后端无关
    /// 切换后端只需在 GameArchitecture 中替换实现类注册
    /// </summary>
    public interface IAssetLoader : IUtility
    {
        void LoadAsync<T>(string key, UnityAction<T> callback) where T : Object;
        T LoadSync<T>(string key) where T : Object;

        /// <summary>释放指定 key 对应的一个资源引用（最后一次加载的 handle）</summary>
        void Unload(string key);

        /// <summary>释放所有已追踪的资源引用，通常在场景切换时调用</summary>
        void UnloadAll();
    }
}
