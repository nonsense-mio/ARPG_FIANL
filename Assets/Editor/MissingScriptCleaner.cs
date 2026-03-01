using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 批量查找并移除 Assets/AB/ 下所有预制体中的 Missing Script 组件。
/// 菜单: Tools/Missing Script Cleaner
/// </summary>
public static class MissingScriptCleaner
{
    [MenuItem("Tools/Find Missing Scripts in AB Prefabs")]
    public static void FindMissingScripts()
    {
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/AB" });
        var found = new List<string>();

        foreach (string guid in prefabPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = PrefabUtility.LoadPrefabContents(path);
            int count = CountMissingScripts(prefab);
            PrefabUtility.UnloadPrefabContents(prefab);

            if (count > 0)
                found.Add($"{path}  ({count} missing)");
        }

        if (found.Count == 0)
        {
            Debug.Log("[MissingScriptCleaner] 未发现任何 Missing Script，全部干净。");
        }
        else
        {
            Debug.LogWarning($"[MissingScriptCleaner] 发现 {found.Count} 个预制体含 Missing Script:\n"
                             + string.Join("\n", found));
        }
    }

    [MenuItem("Tools/Remove Missing Scripts in AB Prefabs")]
    public static void RemoveMissingScripts()
    {
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/AB" });
        int totalFixed = 0;
        int fileCount  = 0;

        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (string guid in prefabPaths)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = PrefabUtility.LoadPrefabContents(path);

                int removed = RemoveMissingScriptsRecursive(prefab);

                if (removed > 0)
                {
                    PrefabUtility.SaveAsPrefabAsset(prefab, path);
                    fileCount++;
                    totalFixed += removed;
                    Debug.Log($"[MissingScriptCleaner] 已清理 {path}，移除 {removed} 个 Missing Script");
                }

                PrefabUtility.UnloadPrefabContents(prefab);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }

        Debug.Log($"[MissingScriptCleaner] 完成：共处理 {fileCount} 个预制体，移除 {totalFixed} 个 Missing Script 组件。");
    }

    // ── 递归统计 Missing Script 数量 ──────────────────────────────────────
    private static int CountMissingScripts(GameObject go)
    {
        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
        foreach (Transform child in go.transform)
            count += CountMissingScripts(child.gameObject);
        return count;
    }

    // ── 递归移除 Missing Script，返回实际移除数量 ─────────────────────────
    private static int RemoveMissingScriptsRecursive(GameObject go)
    {
        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        foreach (Transform child in go.transform)
            count += RemoveMissingScriptsRecursive(child.gameObject);
        return count;
    }
}
