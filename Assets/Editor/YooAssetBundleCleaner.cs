using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// YooAsset 打包产物清理工具 —— 只保留最近 N 个「版本快照目录」，删除更早的。
///
/// 说明：
/// - 增量构建依赖的是 OutputCache（构建缓存），而非这些版本快照目录，
///   因此删除旧版本目录不影响下次增量构建速度，也不影响游戏运行。
/// - OutputCache / Simulate 属于受保护目录，永不删除。
/// - 版本目录按「最后修改时间」倒序排列（因为版本名不一定可排序，如 v6.7 / new1）。
/// - 始终只删「比最新 N 个更旧」的目录，最新构建出来的那个永远在保留之列，故执行时机安全。
/// </summary>
public static class YooAssetBundleCleaner
{
    /// <summary>保留最近的版本目录数量（按修改时间倒序）。按需修改。</summary>
    public const int KeepRecentCount = 3;

    /// <summary>YooAsset 包裹名（与运行时一致）。</summary>
    private const string PackageName = "DefaultPackage";

    /// <summary>打包输出根目录（相对工程根，与 YooAsset 构建设置一致）。</summary>
    private const string BuildOutputRoot = "Bundles";

    /// <summary>永不删除的特殊目录。</summary>
    private static readonly string[] ProtectedDirs = { "OutputCache", "Simulate" };

    /// <summary>当前平台的包裹输出目录，如 .../Bundles/StandaloneWindows64/DefaultPackage。</summary>
    public static string GetPackageDir()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string platform = EditorUserBuildSettings.activeBuildTarget.ToString();
        return Path.Combine(projectRoot, BuildOutputRoot, platform, PackageName);
    }

    [MenuItem("Tools/YooAsset工作流/清理旧版本包体")]
    public static void CleanOldVersionsMenu() => PruneOldVersions(confirm: true);

    /// <summary>
    /// 删除旧版本快照目录，保留最近 KeepRecentCount 个，返回删除数量。
    /// confirm=true 弹确认框（手动菜单用）；false 静默执行（自动清理用）。
    /// </summary>
    public static int PruneOldVersions(bool confirm)
    {
        string packageDir = GetPackageDir();
        if (!Directory.Exists(packageDir))
        {
            if (confirm) EditorUtility.DisplayDialog("清理失败", $"未找到打包目录：\n{packageDir}", "确定");
            return 0;
        }

        // 收集版本目录（排除受保护目录），按修改时间倒序：最新在前。
        var versionDirs = new DirectoryInfo(packageDir)
            .GetDirectories()
            .Where(d => !ProtectedDirs.Contains(d.Name))
            .OrderByDescending(d => d.LastWriteTime)
            .ToList();

        if (versionDirs.Count <= KeepRecentCount)
        {
            if (confirm) EditorUtility.DisplayDialog("无需清理",
                $"当前版本目录 {versionDirs.Count} 个，未超过保留数 {KeepRecentCount}。", "确定");
            return 0;
        }

        var keep = versionDirs.Take(KeepRecentCount).ToList();
        var toDelete = versionDirs.Skip(KeepRecentCount).ToList();

        if (confirm)
        {
            string msg =
                $"目录：{packageDir}\n\n" +
                $"保留最近 {keep.Count} 个：\n  {string.Join("\n  ", keep.Select(d => d.Name))}\n\n" +
                $"删除 {toDelete.Count} 个旧版本：\n  {string.Join("\n  ", toDelete.Select(d => d.Name))}\n\n" +
                "（OutputCache / Simulate 不受影响）确认删除？";
            if (!EditorUtility.DisplayDialog("清理旧版本包体", msg, "删除", "取消"))
                return 0;
        }

        int deleted = 0;
        foreach (var dir in toDelete)
        {
            try
            {
                dir.Delete(true);
                deleted++;
                Debug.Log($"[YooAsset清理] 已删除旧版本：{dir.Name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[YooAsset清理] 删除失败 {dir.Name}：{e.Message}");
            }
        }

        if (deleted > 0)
            Debug.Log($"[YooAsset清理] 完成：删除 {deleted}/{toDelete.Count} 个旧版本，保留最近 {keep.Count} 个。");
        return deleted;
    }
}

/// <summary>
/// 打包后自动清理：监视包裹输出目录，发现新版本快照写入后（去抖）自动调用清理。
/// 不依赖任何打包入口（无论从 YooAsset 窗口还是脚本打包都生效），且只删旧目录，安全。
/// 可通过菜单 Tools/YooAsset工作流/打包后自动清理 开关，状态存 EditorPrefs，默认开启。
/// </summary>
[InitializeOnLoad]
public static class YooAssetAutoCleanWatcher
{
    private const string PrefEnabled = "YooAsset_AutoClean_Enabled";
    private const string MenuPath = "Tools/YooAsset工作流/打包后自动清理";
    private const double DebounceSeconds = 4.0; // 等构建写完再清，避免误触发

    private static FileSystemWatcher _watcher;
    private static volatile bool _dirty;          // 后台线程置位：检测到目录变动
    private static DateTime _lastEventUtc;         // 最近一次事件时间（去抖用）

    static YooAssetAutoCleanWatcher()
    {
        // 域重载后重建监视器
        EditorApplication.delayCall += Setup;
        AssemblyReloadEvents.beforeAssemblyReload += DisposeWatcher;
    }

    private static bool IsEnabled() => EditorPrefs.GetBool(PrefEnabled, true); // 默认开启

    private static void Setup()
    {
        DisposeWatcher();
        if (!IsEnabled()) return;

        string dir = YooAssetBundleCleaner.GetPackageDir();
        if (!Directory.Exists(dir))
            return; // 还没在该平台打过包；下次域重载（如脚本编译后）会再尝试

        _watcher = new FileSystemWatcher(dir)
        {
            NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
            IncludeSubdirectories = false,
            EnableRaisingEvents = true,
        };
        _watcher.Created += OnFsEvent;
        _watcher.Changed += OnFsEvent;
        EditorApplication.update += OnUpdate;
    }

    private static void DisposeWatcher()
    {
        EditorApplication.update -= OnUpdate;
        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Created -= OnFsEvent;
            _watcher.Changed -= OnFsEvent;
            _watcher.Dispose();
            _watcher = null;
        }
    }

    // 后台线程回调：只置标志与时间戳，真正的删除放主线程去抖后执行。
    private static void OnFsEvent(object sender, FileSystemEventArgs e)
    {
        _lastEventUtc = DateTime.UtcNow;
        _dirty = true;
    }

    private static void OnUpdate()
    {
        if (!_dirty) return;
        // 去抖：最近一次变动后静默 DebounceSeconds 秒，才认为构建写完。
        if ((DateTime.UtcNow - _lastEventUtc).TotalSeconds < DebounceSeconds) return;

        _dirty = false;
        YooAssetBundleCleaner.PruneOldVersions(confirm: false);
    }

    [MenuItem(MenuPath)]
    private static void Toggle()
    {
        bool v = !IsEnabled();
        EditorPrefs.SetBool(PrefEnabled, v);
        Menu.SetChecked(MenuPath, v);
        Setup(); // 立即生效
        Debug.Log($"[YooAsset清理] 打包后自动清理已{(v ? "开启" : "关闭")}（保留最近 {YooAssetBundleCleaner.KeepRecentCount} 个版本）。");
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleValidate()
    {
        Menu.SetChecked(MenuPath, IsEnabled());
        return true;
    }
}
