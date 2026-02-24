using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 资源分析工具
/// 从场景、Resources、AB 目录出发，递归分析依赖链，区分已使用/未使用资源。
/// 菜单: Tools/Asset Analyzer
/// </summary>
public class UnusedAssetFinder : EditorWindow
{
    // 扫描根目录（只在这些目录下查找资源）
    private static readonly string[] ScanRoots = { "Assets/ArtRes", "Assets/Resources", "Assets/AB" };

    // 忽略的扩展名（脚本、元数据、文档等不算"资源"）
    private static readonly HashSet<string> IgnoredExtensions = new HashSet<string>
    {
        ".cs", ".meta", ".asmdef", ".asmref", ".md", ".txt", ".json", ".xml", ".dll", ".so", ".bundle"
    };

    private enum ViewMode { Unused, Used }

    private ViewMode currentView = ViewMode.Unused;
    private List<string> unusedAssets = new List<string>();
    private List<string> usedAssets = new List<string>();
    private Vector2 scrollPos;
    private bool analysisComplete;
    private string statusMessage = "";
    private string filterText = "";

    [MenuItem("Tools/Asset Analyzer")]
    public static void ShowWindow()
    {
        GetWindow<UnusedAssetFinder>("Asset Analyzer");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("资源分析工具", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "从 Build Settings 中的场景 + Resources/ + AB/ 目录出发，\n" +
            "递归追踪所有依赖，区分 ScanRoots 下已使用和未使用的资源。\n\n" +
            "注意：分析结果仅供参考，删除前请人工确认。",
            MessageType.Info);

        EditorGUILayout.Space(5);

        if (GUILayout.Button("开始分析", GUILayout.Height(30)))
        {
            Analyze();
        }

        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.LabelField(statusMessage);
        }

        if (!analysisComplete)
            return;

        EditorGUILayout.Space(5);

        // ========== 视图切换 ==========
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Toggle(currentView == ViewMode.Used, $"已使用 ({usedAssets.Count})", "Button"))
            currentView = ViewMode.Used;
        if (GUILayout.Toggle(currentView == ViewMode.Unused, $"未使用 ({unusedAssets.Count})", "Button"))
            currentView = ViewMode.Unused;
        EditorGUILayout.EndHorizontal();

        // ========== 搜索过滤 ==========
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("过滤:", GUILayout.Width(35));
        filterText = EditorGUILayout.TextField(filterText);
        if (GUILayout.Button("清除", GUILayout.Width(45)))
            filterText = "";
        EditorGUILayout.EndHorizontal();

        // ========== 导出按钮 ==========
        List<string> displayList = GetFilteredList();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"显示 {displayList.Count} 条", EditorStyles.miniLabel);
        if (displayList.Count > 0 && GUILayout.Button("导出列表到 txt", GUILayout.Width(120)))
        {
            ExportToFile(displayList, currentView == ViewMode.Unused ? "unused_assets" : "used_assets");
        }
        EditorGUILayout.EndHorizontal();

        // ========== 资源列表 ==========
        EditorGUILayout.Space(3);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var asset in displayList)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("定位", GUILayout.Width(40)))
            {
                var obj = AssetDatabase.LoadAssetAtPath<Object>(asset);
                if (obj != null)
                {
                    EditorGUIUtility.PingObject(obj);
                    Selection.activeObject = obj;
                }
            }

            EditorGUILayout.LabelField(asset);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private List<string> GetFilteredList()
    {
        List<string> source = currentView == ViewMode.Unused ? unusedAssets : usedAssets;
        if (string.IsNullOrEmpty(filterText))
            return source;

        string filter = filterText.ToLower();
        return source.Where(a => a.ToLower().Contains(filter)).ToList();
    }

    private void Analyze()
    {
        unusedAssets.Clear();
        usedAssets.Clear();
        analysisComplete = false;
        statusMessage = "正在分析...";

        // ========== Step 1: 收集所有入口资源 ==========
        HashSet<string> entryAssets = new HashSet<string>();

        // 1a. Build Settings 中的所有场景
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && !string.IsNullOrEmpty(scene.path))
                entryAssets.Add(scene.path);
        }

        // 1b. Resources/ 下的所有资源（Unity 会无条件打入包体）
        AddAllAssetsInFolder("Assets/Resources", entryAssets);

        // 1c. AB/ 下的所有资源（YooAsset Collector 会收集）
        AddAllAssetsInFolder("Assets/AB", entryAssets);

        // 1d. StreamingAssets 下的资源
        AddAllAssetsInFolder("Assets/StreamingAssets", entryAssets);

        statusMessage = $"入口资源: {entryAssets.Count} 个，正在追踪依赖链...";

        // ========== Step 2: 递归获取所有依赖 ==========
        string[] allEntries = entryAssets.ToArray();
        string[] allDependencies = AssetDatabase.GetDependencies(allEntries, true);

        HashSet<string> referencedAssets = new HashSet<string>(allDependencies);
        foreach (var entry in entryAssets)
            referencedAssets.Add(entry);

        statusMessage = $"被引用资源: {referencedAssets.Count} 个，正在对比扫描目录...";

        // ========== Step 3: 扫描目录，分为已使用/未使用 ==========
        foreach (var scanRoot in ScanRoots)
        {
            if (!Directory.Exists(scanRoot))
                continue;

            string[] allFiles = Directory.GetFiles(scanRoot, "*.*", SearchOption.AllDirectories);
            foreach (var filePath in allFiles)
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (IgnoredExtensions.Contains(ext))
                    continue;

                string assetPath = filePath.Replace("\\", "/");
                if (!assetPath.StartsWith("Assets/"))
                {
                    int idx = assetPath.IndexOf("Assets/");
                    if (idx >= 0) assetPath = assetPath.Substring(idx);
                    else continue;
                }

                if (referencedAssets.Contains(assetPath))
                    usedAssets.Add(assetPath);
                else
                    unusedAssets.Add(assetPath);
            }
        }

        unusedAssets.Sort();
        usedAssets.Sort();
        analysisComplete = true;
        currentView = ViewMode.Unused;
        statusMessage = $"分析完成。入口 {entryAssets.Count} → 依赖链 {referencedAssets.Count} | 已使用 {usedAssets.Count} | 未使用 {unusedAssets.Count}";
    }

    private static void AddAllAssetsInFolder(string folder, HashSet<string> set)
    {
        if (!Directory.Exists(folder))
            return;

        string[] guids = AssetDatabase.FindAssets("", new[] { folder });
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(path) && !AssetDatabase.IsValidFolder(path))
                set.Add(path);
        }
    }

    private void ExportToFile(List<string> list, string defaultName)
    {
        string path = EditorUtility.SaveFilePanel("导出资源列表", "", defaultName, "txt");
        if (string.IsNullOrEmpty(path))
            return;

        File.WriteAllLines(path, list);
        Debug.Log($"已导出 {list.Count} 条记录到: {path}");
    }
}
