using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.Events;
using YooAsset;

namespace ARPG
{
    /// <summary>
    /// YooAsset 资源加载实现，替代 AssetBundleLoader。
    /// key 格式：支持 "prefix/assetName" 或 "assetName"，取最后一段作为 YooAsset address。
    /// YooAsset 资源收集器中的 address 需配置为纯文件名（无扩展名），与 key 最后段保持一致。
    /// </summary>
    public class YooAssetLoader : IAssetLoader
    {
        private readonly string _packageName;
        // location → 该 location 对应的所有 handle（支持同一资源多次加载）
        private readonly Dictionary<string, List<AssetHandle>> _handles = new Dictionary<string, List<AssetHandle>>();

        public YooAssetLoader(string packageName = "DefaultPackage")
        {
            _packageName = packageName;
        }

        public void LoadAsync<T>(string key, UnityAction<T> callback) where T : Object
        {
            string location = ExtractLocation(key);
            var handle = YooAssets.GetPackage(_packageName).LoadAssetAsync<T>(location);
            TrackHandle(location, handle);
            handle.Completed += h => callback?.Invoke(h.GetAssetObject<T>());
        }

        public T LoadSync<T>(string key) where T : Object
        {
            string location = ExtractLocation(key);
            var handle = YooAssets.GetPackage(_packageName).LoadAssetSync<T>(location);
            TrackHandle(location, handle);
            return handle.GetAssetObject<T>();
        }

        public void Unload(string key)
        {
            string location = ExtractLocation(key);
            if (!_handles.TryGetValue(location, out var list) || list.Count == 0)
                return;

            // 释放最后一次加载的 handle
            var handle = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            handle.Release();

            if (list.Count == 0)
                _handles.Remove(location);
        }

        public void UnloadAll()
        {
            foreach (var list in _handles.Values)
                foreach (var handle in list)
                    handle.Release();
            _handles.Clear();
            YooAssets.GetPackage(_packageName).UnloadUnusedAssetsAsync();
        }

        private void TrackHandle(string location, AssetHandle handle)
        {
            if (!_handles.TryGetValue(location, out var list))
            {
                list = new List<AssetHandle>();
                _handles[location] = list;
            }
            list.Add(handle);
        }

        // "ui/GamePanel" → "GamePanel"，"Player" → "Player"
        private static string ExtractLocation(string key)
        {
            int idx = key.LastIndexOf('/');
            return idx < 0 ? key : key.Substring(idx + 1);
        }
    }
}
