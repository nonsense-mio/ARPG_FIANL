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
    }
}
