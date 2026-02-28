using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ARPG;
using Framework;
using HybridCLR;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

public class Main : MonoBehaviour
{
    public EPlayMode playMode = EPlayMode.HostPlayMode;
    public string packageName = "DefaultPackage";
    public string packageVersion = "";
    public ResourcePackage package = null;

    // 网络相关
    public string defaultHostServer = "http://192.168.3.133:8080/CDN/PC/ARPGv1.0";
    public string fallbackHostServer = "http://192.168.3.133:8080/CDN/PC/ARPGv1.0";
    // 下载相关
    public int downloadingMaxNum = 10;
    public int failedTryAgain = 3;
    public ResourceDownloaderOperation downloader;

    [SerializeField] BootPanel bootPanel;

    // 下载确认弹窗（Bootstrap 场景内直接引用，不经过 UISystem）
    [SerializeField] GameObject confirmDialog;
    [SerializeField] Text confirmText;
    [SerializeField] Button btnConfirm;

    // AOT 补充元数据 DLL 名称列表（不含路径和扩展名）
    // 运行后由 HybridCLR 生成 AOTGenericReferences 后，以 PatchedAOTAssemblyList 为准更新此列表
    // 约定路径：Assets/AB/HotDll/{name}.dll.bytes
    [SerializeField] List<string> aotMetadataDlls = new List<string>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Start()
    {
        yield return null;
        yield return InitializeAndUpdate();
    }

    /// <summary>
    /// 初始化→版本检查→下载流程。
    /// </summary>
    private IEnumerator InitializeAndUpdate()
    {
        // 1. 初始化 YooAsset 资源系统
        YooAssets.Initialize();
        package = YooAssets.TryGetPackage(packageName);
        if (package == null)
            package = YooAssets.CreatePackage(packageName);

        InitializationOperation initializationOperation = package.InitializeAsync(CreateInitParameters());
        yield return initializationOperation;

        if (initializationOperation.Status != EOperationStatus.Succeed)
        {
            ShowRetryDialog("资源系统初始化失败", initializationOperation.Error);
            yield break;
        }
        bootPanel.SetProgress(0.2f, "初始化资源系统...");

        // EditorSimulateMode / OfflinePlayMode 不需要远程版本检查，直接进入游戏
        if (playMode != EPlayMode.HostPlayMode)
        {
            bootPanel.SetProgress(1.0f, "正在进入游戏...");
            UpdateDone();
            yield break;
        }

        // 2. 获取资源版本
        var operation = package.RequestPackageVersionAsync();
        yield return operation;
        if (operation.Status != EOperationStatus.Succeed)
        {
            ShowRetryDialog("获取资源版本失败", operation.Error);
            yield break;
        }
        packageVersion = operation.PackageVersion;
        bootPanel.SetProgress(0.4f, "获取资源版本...");

        // 3. 更新文件清单
        var operationManifest = package.UpdatePackageManifestAsync(packageVersion);
        yield return operationManifest;
        if (operationManifest.Status != EOperationStatus.Succeed)
        {
            ShowRetryDialog("更新资源清单失败", operationManifest.Error);
            yield break;
        }
        bootPanel.SetProgress(0.6f, "更新资源清单...");

        // 4. 创建下载器
        downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        if (downloader.TotalDownloadCount == 0)
        {
            bootPanel.SetProgress(1.0f, "无需更新，正在进入游戏...");
            UpdateDone();
            yield break;
        }

        // 5. 弹出下载确认（直接操作 Bootstrap 场景 UI，不经过 UISystem）
        int count = downloader.TotalDownloadCount;
        string sizeText = FormatBytes(downloader.TotalDownloadBytes);
        bootPanel.SetProgress(0.65f, $"检测到 {count} 个文件 ({sizeText}) 待更新");

        confirmText.text = $"检测到 {count} 个文件待更新，共 {sizeText}，是否立即下载？";
        confirmDialog.SetActive(true);
        btnConfirm.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(() =>
        {
            confirmDialog.SetActive(false);
            StartCoroutine(StartDownload());
        });
    }

    /// <summary>
    /// 根据 playMode 创建对应的初始化参数。
    /// EditorSimulateMode 仅在 Editor 下可用；Build 中会 fallback 到 OfflinePlayMode。
    /// </summary>
    private InitializeParameters CreateInitParameters()
    {
        switch (playMode)
        {
#if UNITY_EDITOR
            case EPlayMode.EditorSimulateMode:
                var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                var editorParams = new EditorSimulateModeParameters();
                editorParams.EditorFileSystemParameters =
                    FileSystemParameters.CreateDefaultEditorFileSystemParameters(buildResult.PackageRootDirectory);
                return editorParams;
#endif
            case EPlayMode.OfflinePlayMode:
                var offlineParams = new OfflinePlayModeParameters();
                offlineParams.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                return offlineParams;

            case EPlayMode.HostPlayMode:
            default:
                IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                var hostParams = new HostPlayModeParameters();
                hostParams.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                hostParams.CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                return hostParams;
        }
    }

    IEnumerator StartDownload()
    {
        downloader.DownloadUpdateCallback = ProgressCallBack;
        downloader.BeginDownload();
        yield return downloader;

        if (downloader.Status != EOperationStatus.Succeed)
        {
            ShowRetryDialog("资源下载失败", downloader.Error);
            yield break;
        }

        bootPanel.SetProgress(0.98f, "清理旧缓存...");
        var operationClear = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
        operationClear.Completed += op =>
        {
            if (op.Status != EOperationStatus.Succeed)
                Debug.LogWarning($"清理旧缓存失败（不影响游戏运行）: {op.Error}");
            UpdateDone();
        };
    }

    private void ProgressCallBack(DownloadUpdateData data)
    {
        if (data.TotalDownloadBytes > 0)
        {
            float t = (float)data.CurrentDownloadBytes / data.TotalDownloadBytes;
            bootPanel.SetProgress(0.65f + t * 0.3f,
                $"正在下载 ({data.CurrentDownloadCount}/{data.TotalDownloadCount})");
        }
    }

    /// <summary>
    /// 弹出错误弹窗，显示失败原因并提供"重试"按钮。
    /// 重试会销毁当前 YooAsset 包并从头执行完整流程。
    /// </summary>
    private void ShowRetryDialog(string title, string error)
    {
        Debug.LogWarning($"{title}: {error}");
        confirmText.text = $"{title}\n{error}\n\n点击确认重试";
        confirmDialog.SetActive(true);
        btnConfirm.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(() =>
        {
            confirmDialog.SetActive(false);
            // 销毁已失败的 package，重新开始
            if (package != null)
            {
                YooAssets.RemovePackage(packageName);
                package = null;
            }
            StartCoroutine(InitializeAndUpdate());
        });
    }

    private void UpdateDone()
    {
        StartCoroutine(LoadAndEnterGame());
    }

    /// <summary>
    /// 补充 AOT 元数据 → 加载热更 DLL → 通过 IGameLauncher 进入游戏。
    /// Editor 下跳过 DLL 加载（HotUpdate 程序集已由 Unity 自动载入）。
    /// </summary>
    private IEnumerator LoadAndEnterGame()
    {
        bootPanel.SetProgress(1.0f, "加载热更模块...");
        yield return null;

#if !UNITY_EDITOR
        // 1. 补充 AOT 泛型元数据
        foreach (string dllName in aotMetadataDlls)
        {
            string path = $"Assets/AB/HotDll/{dllName}.dll.bytes";
            AssetHandle metaHandle = package.LoadAssetSync<TextAsset>(path);
            if (metaHandle.AssetObject is TextAsset metaAsset)
                RuntimeApi.LoadMetadataForAOTAssembly(
                    metaAsset.bytes, HomologousImageMode.SuperSet);
            metaHandle.Release();
        }

        // 2. 加载热更 DLL
        AssetHandle dllHandle = package.LoadAssetSync<TextAsset>(
            "Assets/AB/HotDll/HotUpdate.dll.bytes");
        Assembly hotUpdateAss = Assembly.Load(
            (dllHandle.AssetObject as TextAsset).bytes);
        dllHandle.Release();

        // 3. 在热更程序集中查找 GameLauncher
        Type launcherType = hotUpdateAss.GetType("ARPG.GameLauncher");
        if (launcherType == null)
        {
            Debug.LogError("热更程序集中未找到 ARPG.GameLauncher，无法启动游戏。");
            yield break;
        }
#else
        // Editor：HotUpdate 程序集已由 Unity 自动加载，从 AppDomain 中查找
        Assembly hotUpdateAss = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "HotUpdate");
        if (hotUpdateAss == null)
        {
            Debug.LogError("未找到 HotUpdate 程序集，请检查 HotUpdate.asmdef 配置。");
            yield break;
        }
        Type launcherType = hotUpdateAss.GetType("ARPG.GameLauncher");
        if (launcherType == null)
        {
            Debug.LogError("HotUpdate 程序集中未找到 ARPG.GameLauncher。");
            yield break;
        }
#endif

        // 4. 注册 LaunchGameEvent 监听 — AOT 侧响应热更发来的启动事件
        var arch = GameArchitecture.Interface;
        arch.RegisterEvent<LaunchGameEvent>(e =>
        {
            arch.SendCommand<TransitionToBeginSceneCommand>();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        // 5. 启动游戏 — 热更 GameLauncher 通过 IArchitecture 发送 LaunchGameEvent
        IGameLauncher launcher = (IGameLauncher)Activator.CreateInstance(launcherType);
        launcher.Launch(arch);
    }

    /// <summary>
    /// 将字节数格式化为可读字符串 (KB / MB)。
    /// </summary>
    private static string FormatBytes(long bytes)
    {
        if (bytes < 1024 * 1024)
            return $"{bytes / 1024f:F1}KB";
        return $"{bytes / (1024f * 1024f):F1}MB";
    }

    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }

        string IRemoteServices.GetRemoteMainURL(string fileName)
            => $"{_defaultHostServer}/{fileName}";

        string IRemoteServices.GetRemoteFallbackURL(string fileName)
            => $"{_fallbackHostServer}/{fileName}";
    }
}
