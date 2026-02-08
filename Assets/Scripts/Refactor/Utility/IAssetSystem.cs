using Framework;
using UnityEngine;
using UnityEngine.Events;

namespace ARPG
{
    /// <summary>
    /// AB资源加载抽象接口 - 替代 ABResMgr (BaseManager 单例)
    /// Editor模式通过EditorResMgr加载，Runtime通过ABMgr加载
    /// </summary>
    public interface IAssetSystem : IUtility
    {
        void LoadAssetAsync<T>(string bundleName, string assetName,
                               UnityAction<T> callback, bool isSync = false) where T : Object;
    }
}
