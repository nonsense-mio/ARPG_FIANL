using Framework;
using ARPG;
using UnityEngine;
using UnityEngine.Events;

namespace ARPG
{
    /// <summary>
    /// AB资源加载实现 - 替代 ABResMgr (BaseManager 单例)
    /// Editor模式委托 EditorResMgr，Runtime模式委托 ABMgr
    /// </summary>
    public class AssetSystem : IAssetSystem
    {
        private readonly bool isDebug = true;

        public void LoadAssetAsync<T>(string bundleName, string assetName,
                                      UnityAction<T> callback, bool isSync = false) where T : Object
        {
#if UNITY_EDITOR
            if (isDebug)
            {
                T res = EditorResMgr.Instance.LoadEditorRes<T>($"{bundleName}/{assetName}");
                callback(res);
                return;
            }
#endif
            ABMgr.Instance.LoadResAsync<T>(bundleName, assetName, callback, isSync);
        }
    }
}
