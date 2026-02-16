using Framework;
using UnityEngine;
using UnityEngine.Events;

namespace ARPG
{
    /// <summary>
    /// AssetBundle 资源加载实现
    /// key 格式: "bundleName/assetName"，内部拆分后委托 ABMgr / EditorResMgr
    /// </summary>
    public class AssetBundleLoader : IAssetLoader
    {
        private readonly bool isDebug = true;

        public void LoadAsync<T>(string key, UnityAction<T> callback) where T : Object
        {
            var (bundleName, assetName) = SplitKey(key);
#if UNITY_EDITOR
            if (isDebug)
            {
                T res = EditorResMgr.Instance.LoadEditorRes<T>(key);
                callback(res);
                return;
            }
#endif
            ABMgr.Instance.LoadResAsync<T>(bundleName, assetName, callback, false);
        }

        public T LoadSync<T>(string key) where T : Object
        {
            var (bundleName, assetName) = SplitKey(key);
#if UNITY_EDITOR
            if (isDebug)
                return EditorResMgr.Instance.LoadEditorRes<T>(key);
#endif
            T result = null;
            ABMgr.Instance.LoadResAsync<T>(bundleName, assetName, r => result = r, true);
            return result;
        }

        private static (string bundle, string asset) SplitKey(string key)
        {
            int idx = key.IndexOf('/');
            return (key.Substring(0, idx), key.Substring(idx + 1));
        }
    }
}
