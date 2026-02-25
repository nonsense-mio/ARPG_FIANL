using System.Collections;
using ARPG;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

public class Main : MonoBehaviour
{
    public EPlayMode playMode = EPlayMode.HostPlayMode;//运行模式
    public string packageName = "DefaultPackage"; //包名
    public string packageVersion = ""; //资源版本号
    public ResourcePackage package = null;

    //网络相关
    public string defaultHostServer = "http://192.168.3.133:8080/CDN/PC/ARPGv1.0";
    public string fallbackHostServer = "http://192.168.3.133:8080/CDN/PC/ARPGv1.0";
    //下载相关
    public int downloadingMaxNum = 10;
    public int failedTryAgain = 3;
    public ResourceDownloaderOperation downloader;

    [SerializeField] BootPanel bootPanel;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Start()
    {
        yield return null;
        //1.初始化YooAsset资源系统
        YooAssets.Initialize();
        package = YooAssets.TryGetPackage(packageName);
        if (package == null)
        {
            package = YooAssets.CreatePackage(packageName);
        }
        IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);

        var createParameters = new HostPlayModeParameters();
        //创建文件系统内置参数
        createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
        //创建缓存系统参数
        createParameters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
        //执行异步初始化
        InitializationOperation initializationOperation = package.InitializeAsync(createParameters);
        yield return initializationOperation;

        if (initializationOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(initializationOperation.Error);
            yield break;
        }
        bootPanel.SetProgress(0.2f, "初始化资源系统...");

        //2.获取资源版本
        var operation = package.RequestPackageVersionAsync();
        yield return operation;
        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            yield break;
        }
        packageVersion = operation.PackageVersion;
        bootPanel.SetProgress(0.4f, "获取资源版本...");

        //3.获取文件清单
        var operationManifest = package.UpdatePackageManifestAsync(packageVersion);
        yield return operationManifest;
        if (operationManifest.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operationManifest.Error);
            yield break;
        }
        bootPanel.SetProgress(0.6f, "更新资源清单...");

        //4.创建下载器
        downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        if (downloader.TotalDownloadCount == 0)
        {
            bootPanel.SetProgress(1.0f, "无需更新，正在进入游戏...");
            UpdateDone();
            yield break;
        }
        //5.开始下载
        int count = downloader.TotalDownloadCount;
        long mb = downloader.TotalDownloadBytes / 1024 / 1024;
        bootPanel.SetProgress(0.65f, $"检测到 {count} 个文件 ({mb}MB) 待更新");

        GameArchitecture.Interface.GetSystem<IUISystem>().ShowPanel<TipPanel>(tip =>
        {
            tip.GetControl<Button>("btnClose").gameObject.SetActive(false);
            tip.SetTipInfo($"检测到 {count} 个文件待更新，共 {mb}MB，是否立即下载？",
                () => StartCoroutine(StartDownload()));
        });
        yield break;
    }

    IEnumerator StartDownload()
    {
        downloader.DownloadUpdateCallback = ProgressCallBack;
        downloader.BeginDownload();
        yield return downloader;

        if (downloader.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(downloader.Error);
            yield break;
        }

        bootPanel.SetProgress(0.98f, "清理旧缓存...");
        var operationClear = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
        operationClear.Completed += Operation_Completed;
    }

    //文件清理完成
    private void Operation_Completed(AsyncOperationBase @base)
    {
        UpdateDone();
    }

    //下载进度回调
    private void ProgressCallBack(DownloadUpdateData data)
    {
        if (data.TotalDownloadBytes > 0)
        {
            float t = (float)data.CurrentDownloadBytes / data.TotalDownloadBytes;
            bootPanel.SetProgress(0.65f + t * 0.3f,
                $"正在下载 ({data.CurrentDownloadCount}/{data.TotalDownloadCount})");
        }
    }

    //热更新结束
    private void UpdateDone()
    {
        bootPanel.SetProgress(1.0f, "加载完成，正在进入游戏...");
        GameArchitecture.Interface.SendCommand<TransitionToBeginSceneCommand>();
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
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
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }
}
