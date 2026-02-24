using System.Collections;
using ARPG;
using UnityEngine;
using YooAsset;

public class Main : MonoBehaviour
{
    public EPlayMode playMode = EPlayMode.HostPlayMode;//运行模式
    public string packageName = "DefaultPackage"; //包名
    public string packageVersion = ""; //资源版本号
    public ResourcePackage package = null;

    //网络相关
    public string defaultHostServer = "http://192.168.3.133:8080/CDN/PC/v1.0";
    public string fallbackHostServer = "http://192.168.3.133:8080/CDN/PC/v1.0";
    //下载相关
    public int downloadingMaxNum = 10;
    public int failedTryAgain = 3;
    public ResourceDownloaderOperation downloader;

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
        else
        {
            Debug.Log("初始化成功");
        }

        //2.获取资源版本
        var operation = package.RequestPackageVersionAsync();
        yield return operation;
        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            yield break;
        }
        else
        {
            Debug.Log($"请求的版本:{operation.PackageVersion}");
            packageVersion = operation.PackageVersion;
        }

        //3.获取文件清单
        var operationManifest = package.UpdatePackageManifestAsync(packageVersion);
        yield return operationManifest;
        if (operationManifest.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operationManifest.Error);
            yield break;
        }
        else
        {
            Debug.Log("更新资源清单成功");
        }

        //4.创建下载器
        downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("没有需要更新的文件");
            UpdateDone();
            yield break;
        }
        else
        {
            int count = downloader.TotalDownloadCount;
            long bytes = downloader.TotalDownloadBytes;
            Debug.Log($"需要更新{count}个文件,大小是{bytes / 1024 / 1024}MB");
        }

        //5.开始下载
        downloader.DownloadUpdateCallback = ProgressCallBack;
        downloader.BeginDownload();
        yield return downloader;
        if (downloader.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(downloader.Error);
            yield break;
        }
        else
        {
            Debug.Log("下载成功");
        }

        //6.清理过期缓存文件
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
        Debug.Log($"需要更新{data.TotalDownloadCount}个文件,当前已更新{data.CurrentDownloadCount}," +
        $"大小是{data.TotalDownloadBytes / 1024 / 1024}MB,已下载{data.CurrentDownloadBytes / 1024 / 1024}MB");
    }


    //热更新结束
    private void UpdateDone()
    {
        Debug.Log("热更新结束");
        GameArchitecture.Interface.SendCommand<InitBeginSceneCommand>();
    }

    // Update is called once per frame
    void Update()
    {

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
