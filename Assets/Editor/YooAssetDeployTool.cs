using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class YooAssetDeployTool : EditorWindow
{
    private const string PREF_SOURCE  = "YooAssetDeploy_SourcePath";
    private const string PREF_TARGET  = "YooAssetDeploy_TargetPath";
    private const string PREF_CLEANUP = "YooAssetDeploy_EnableCleanup";

    private string _sourcePath;
    private string _targetPath;
    private bool   _enableCleanup;

    private Vector2 _scrollPos;
    private readonly List<DeployEntry> _pending = new();  // 待上传
    private readonly List<string>      _orphans = new();  // 待删除（服务器多余bundle）
    private bool _scanned;

    private struct DeployEntry
    {
        public string FullPath;
        public string FileName;
        public long   Bytes;
        public bool   IsManifest; // true=清单文件(内容比对), false=bundle(仅查文件名)
    }

    [MenuItem("Tools/AB包资源部署工具")]
    private static void OpenWindow()
    {
        var win = GetWindow<YooAssetDeployTool>("YooAsset 增量部署");
        win.minSize = new Vector2(520, 460);
        win.Show();
    }

    private void OnEnable()
    {
        string defaultSource = Path.GetFullPath(
            Path.Combine(Application.dataPath, "../Bundles/StandaloneWindows/DefaultPackage/"));
        _sourcePath    = EditorPrefs.GetString(PREF_SOURCE,  defaultSource);
        _targetPath    = EditorPrefs.GetString(PREF_TARGET,  "");
        _enableCleanup = EditorPrefs.GetBool  (PREF_CLEANUP, false);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6);
        GUILayout.Label("YooAsset 增量部署工具", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        DrawPathField("本地构建目录", ref _sourcePath, PREF_SOURCE);
        DrawPathField("服务器目录",   ref _targetPath, PREF_TARGET);

        EditorGUILayout.Space(6);

        // 清理过期包开关
        bool newCleanup = EditorGUILayout.ToggleLeft(
            "同时清理服务器上已过期的 bundle（本地构建不再包含的旧文件）",
            _enableCleanup);
        if (newCleanup != _enableCleanup)
        {
            _enableCleanup = newCleanup;
            EditorPrefs.SetBool(PREF_CLEANUP, _enableCleanup);
            _scanned = false; // 选项变化后需要重新扫描
        }

        EditorGUILayout.Space(6);

        using (new EditorGUI.DisabledScope(
            string.IsNullOrEmpty(_sourcePath) || string.IsNullOrEmpty(_targetPath)))
        {
            if (GUILayout.Button("扫描差异", GUILayout.Height(28)))
                Scan();
        }

        if (!_scanned) return;

        EditorGUILayout.Space(6);

        bool hasWork = _pending.Count > 0 || (_enableCleanup && _orphans.Count > 0);
        if (!hasWork)
        {
            EditorGUILayout.HelpBox("服务器已是最新，无需部署。", MessageType.Info);
            return;
        }

        // 统计上传量
        long totalBytes = 0;
        foreach (var e in _pending) totalBytes += e.Bytes;
        string sizeStr = FormatBytes(totalBytes);

        string summary = $"待上传: {_pending.Count} 个文件，共 {sizeStr}";
        if (_enableCleanup && _orphans.Count > 0)
            summary += $"    待删除: {_orphans.Count} 个过期 bundle";
        EditorGUILayout.LabelField(summary, EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        // 文件列表（上传 + 删除混合展示）
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos,
            GUILayout.ExpandHeight(true), GUILayout.MaxHeight(260));

        foreach (var entry in _pending)
        {
            DrawRow(
                tag:   entry.IsManifest ? "[更新]" : "[新增]",
                color: entry.IsManifest ? new Color(1f, 0.7f, 0f) : new Color(0.4f, 0.85f, 0.4f),
                name:  entry.FileName,
                size:  FormatBytes(entry.Bytes));
        }

        if (_enableCleanup)
        {
            foreach (var fileName in _orphans)
            {
                DrawRow(
                    tag:   "[删除]",
                    color: new Color(1f, 0.4f, 0.4f),
                    name:  fileName,
                    size:  "");
            }
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(8);

        // 删除操作风险提示
        if (_enableCleanup && _orphans.Count > 0)
        {
            EditorGUILayout.HelpBox(
                "注意: 过期 bundle 将被从服务器永久删除。若有旧版本客户端仍在使用这些文件，" +
                "请确认所有客户端已完成更新后再执行清理。",
                MessageType.Warning);
        }

        Color prevBg = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
        if (GUILayout.Button("一键部署", GUILayout.Height(32)))
            Deploy();
        GUI.backgroundColor = prevBg;
    }

    // ── 绘制辅助 ─────────────────────────────────────────────────────────────

    private static void DrawRow(string tag, Color color, string name, string size)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            var style = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = color } };
            GUILayout.Label(tag, style, GUILayout.Width(50));
            GUILayout.Label(name, EditorStyles.miniLabel);
            GUILayout.FlexibleSpace();
            if (!string.IsNullOrEmpty(size))
                GUILayout.Label(size, EditorStyles.miniLabel, GUILayout.Width(70));
        }
    }

    private void DrawPathField(string label, ref string path, string prefKey)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(90));
            string newPath = EditorGUILayout.TextField(path);
            if (newPath != path)
            {
                path = newPath;
                EditorPrefs.SetString(prefKey, path);
                _scanned = false;
            }
            if (GUILayout.Button("选择", GUILayout.Width(46)))
            {
                string selected = EditorUtility.OpenFolderPanel(label, path, "");
                if (!string.IsNullOrEmpty(selected) && selected != path)
                {
                    path = selected;
                    EditorPrefs.SetString(prefKey, path);
                    _scanned = false;
                }
            }
        }
    }

    // ── 扫描 ─────────────────────────────────────────────────────────────────

    private void Scan()
    {
        _pending.Clear();
        _orphans.Clear();
        _scanned = true;

        if (!Directory.Exists(_sourcePath))
        {
            EditorUtility.DisplayDialog("错误", $"本地构建目录不存在:\n{_sourcePath}", "确定");
            return;
        }

        // ① 建立本地文件集合（文件名 → 完整路径）
        var localFiles = new Dictionary<string, string>();
        foreach (string filePath in Directory.GetFiles(_sourcePath, "*", SearchOption.TopDirectoryOnly))
        {
            string ext      = Path.GetExtension(filePath).ToLower();
            string fileName = Path.GetFileName(filePath);
            if (ext == ".meta") continue;
            if (fileName.StartsWith("buildlog")) continue;
            localFiles[fileName] = filePath;
        }

        // ② 对本地文件逐一判断是否需要上传
        foreach (var kv in localFiles)
        {
            string fileName = kv.Key;
            string filePath = kv.Value;
            string ext      = Path.GetExtension(fileName).ToLower();

            bool isManifest = ext == ".version" || ext == ".bytes" || ext == ".json";

            if (isManifest)
            {
                // 清单文件：服务器不存在 或 内容不同 → 上传
                // 原理：直接字节比对。清单文件通常很小（< 1MB），开销可忽略。
                string targetFile = Path.Combine(_targetPath, fileName);
                if (File.Exists(targetFile) && FilesAreIdentical(filePath, targetFile))
                    continue;

                _pending.Add(MakeEntry(filePath, fileName, isManifest: true));
            }
            else if (ext == ".bundle")
            {
                // Bundle 文件：文件名本身就是内容哈希（FileHash），
                // 服务器有同名文件 ≡ 服务器已有相同内容 → 无需上传。
                // 只要服务器不存在此文件名，就一定是新增或内容变更后的新版本。
                string targetFile = Path.Combine(_targetPath, fileName);
                if (!File.Exists(targetFile))
                    _pending.Add(MakeEntry(filePath, fileName, isManifest: false));
            }
        }

        // ③ 扫描服务器上多余的过期 bundle（本地不存在的，只针对 .bundle）
        if (_enableCleanup && Directory.Exists(_targetPath))
        {
            foreach (string serverFile in Directory.GetFiles(_targetPath, "*.bundle", SearchOption.TopDirectoryOnly))
            {
                string fileName = Path.GetFileName(serverFile);
                if (!localFiles.ContainsKey(fileName))
                    _orphans.Add(fileName);
            }
        }

        // 清单文件排最后上传（确保 bundle 全部就位后再更新清单，
        // 避免客户端拿到新清单但 bundle 还没传完的窗口期）
        _pending.Sort((a, b) => a.IsManifest.CompareTo(b.IsManifest));
    }

    // ── 部署 ─────────────────────────────────────────────────────────────────

    private void Deploy()
    {
        bool hasUploads  = _pending.Count > 0;
        bool hasDeletions = _enableCleanup && _orphans.Count > 0;
        if (!hasUploads && !hasDeletions) return;

        string msg = $"上传 {_pending.Count} 个文件";
        if (hasDeletions) msg += $"\n删除 {_orphans.Count} 个过期 bundle";
        msg += $"\n\n目标目录:\n{_targetPath}\n\n确定继续？";

        if (!EditorUtility.DisplayDialog("确认部署", msg, "部署", "取消"))
            return;

        try
        {
            Directory.CreateDirectory(_targetPath);
            int total = _pending.Count + (hasDeletions ? _orphans.Count : 0);
            int done  = 0;

            // 先上传新 bundle，再上传清单（_pending 已按此顺序排序）
            foreach (var entry in _pending)
            {
                EditorUtility.DisplayProgressBar("部署中...", $"[上传] {entry.FileName}", (float)done / total);
                File.Copy(entry.FullPath, Path.Combine(_targetPath, entry.FileName), overwrite: true);
                done++;
            }

            // 最后删除过期 bundle
            if (hasDeletions)
            {
                foreach (string fileName in _orphans)
                {
                    EditorUtility.DisplayProgressBar("部署中...", $"[删除] {fileName}", (float)done / total);
                    File.Delete(Path.Combine(_targetPath, fileName));
                    done++;
                }
            }

            EditorUtility.ClearProgressBar();

            string result = $"上传 {_pending.Count} 个文件";
            if (hasDeletions) result += $"，删除 {_orphans.Count} 个过期 bundle";
            EditorUtility.DisplayDialog("部署完成", result + "。", "确定");

            Scan();
            Repaint();
        }
        catch (System.Exception ex)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("部署失败", ex.Message, "确定");
            Debug.LogError($"[YooAssetDeploy] 部署失败: {ex}");
        }
    }

    // ── 工具方法 ──────────────────────────────────────────────────────────────

    private static DeployEntry MakeEntry(string fullPath, string fileName, bool isManifest) =>
        new()
        {
            FullPath   = fullPath,
            FileName   = fileName,
            Bytes      = new FileInfo(fullPath).Length,
            IsManifest = isManifest
        };

    /// <summary>
    /// 判断两个文件内容是否完全相同。
    /// 先比大小（O(1) 快速剪枝），再逐字节比对。
    /// 仅用于小型清单文件（.version/.bytes/.json），bundle 靠文件名哈希直接判断。
    /// </summary>
    private static bool FilesAreIdentical(string path1, string path2)
    {
        var info1 = new FileInfo(path1);
        var info2 = new FileInfo(path2);
        if (info1.Length != info2.Length) return false;
        byte[] b1 = File.ReadAllBytes(path1);
        byte[] b2 = File.ReadAllBytes(path2);
        for (int i = 0; i < b1.Length; i++)
            if (b1[i] != b2[i]) return false;
        return true;
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes >= 1024 * 1024) return $"{bytes / 1024f / 1024f:F2} MB";
        return $"{bytes / 1024f:F1} KB";
    }
}
