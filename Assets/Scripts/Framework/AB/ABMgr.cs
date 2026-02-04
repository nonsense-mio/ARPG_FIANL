using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

//知识点
//字典
//协程
//AB包相关API
//委托
//lambda表达式
//单例模式基类
public class ABMgr : SingletonAutoMono<ABMgr>
{
    //主包
    private AssetBundle mainAB = null;
    //主包依赖获取配置文件
    private AssetBundleManifest manifest = null;

    //选择存储 AB包的容器
    //AB包不能够重复加载 否则会报错
    //字典知识 用来存储 AB包对象
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 获取AB包加载路径
    /// </summary>
    private string PathUrl
    {
        get
        {

            return Application.streamingAssetsPath + "/";

        }
    }

    /// <summary>
    /// 主包名 根据平台不同 报名不同
    /// </summary>
    private string MainName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }

    /// <summary>
    /// 加载主包 和 配置文件
    /// 因为加载所有包是 都得判断 通过它才能得到依赖信息
    /// 所以写一个方法
    /// </summary>
    private void LoadMainAB()
    {
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    // 返回指定AB包的可用路径：优先 persistentDataPath，其次 streamingAssetsPath
    private string GetABPath(string abName)
    {
        string persistentPath = Path.Combine(Application.persistentDataPath, abName);
        if (File.Exists(persistentPath))
            return persistentPath;

        return Path.Combine(Application.streamingAssetsPath, abName);
    }


    /// <summary>
    /// 泛型异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false) where T : Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack, isSync));
    }
    //正儿八经的 协程函数
    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync) where T : Object
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                //同步加载
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(GetABPath(strs[i]));
                    abDic.Add(strs[i], ab);
                }
                //异步加载
                else
                {
                    //一开始异步加载 就先记录 如果值为null说明该ab包在异步加载中
                    abDic.Add(strs[i], null);
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(GetABPath(strs[i]));
                    yield return abcr;
                    abDic[strs[i]] = abcr.assetBundle;
                }

            }
            else
            {
                //如果字典中记录的信息为null 说明该包正在加载中
                //等待其加载结束
                while (abDic[strs[i]] == null)
                {
                    //等待一帧
                    yield return 0;
                }
            }
        }
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            //同步加载
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(GetABPath(abName));
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(GetABPath(abName));
                yield return abcr;
                abDic[abName] = abcr.assetBundle;
            }

        }
        else
        {
            //如果字典中记录的信息为null 说明该包正在加载中
            //等待其加载结束
            while (abDic[abName] == null)
            {
                //等待一帧
                yield return 0;
            }
        }

        if (isSync)
        {
            //同步加载AB包中资源
            T res = abDic[abName].LoadAsset<T>(resName);
            callBack(res);
        }
        else
        {
            //异步加载包中资源
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync<T>(resName);
            yield return abq;

            callBack(abq.asset as T);
        }

    }

    /// <summary>
    /// Type异步加载资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack, bool isSync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, type, callBack, isSync));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack, bool isSync)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                //同步加载
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], ab);
                }
                //异步加载
                else
                {
                    //一开始异步加载 就先记录 如果值为null说明该ab包在异步加载中
                    abDic.Add(strs[i], null);
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return abcr;
                    abDic[strs[i]] = abcr.assetBundle;
                }

            }
            else
            {
                //如果字典中记录的信息为null 说明该包正在加载中
                //等待其加载结束
                while (abDic[strs[i]] == null)
                {
                    //等待一帧
                    yield return 0;
                }
            }
        }
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            //同步加载
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return abcr;
                abDic[abName] = abcr.assetBundle;
            }

        }
        else
        {
            //如果字典中记录的信息为null 说明该包正在加载中
            //等待其加载结束
            while (abDic[abName] == null)
            {
                //等待一帧
                yield return 0;
            }
        }
        if (isSync)
        {
            //同步加载AB包中资源
            Object res = abDic[abName].LoadAsset(resName, type);
            callBack(res);
        }
        else
        {
            //异步加载包中资源
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName, type);
            yield return abq;
            callBack(abq.asset);
        }

    }

    /// <summary>
    /// 名字 异步加载 指定资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack, bool isSync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, callBack, isSync));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack, bool isSync)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                //同步加载
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], ab);
                }
                //异步加载
                else
                {
                    //一开始异步加载 就先记录 如果值为null说明该ab包在异步加载中
                    abDic.Add(strs[i], null);
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return abcr;
                    abDic[strs[i]] = abcr.assetBundle;
                }

            }
            else
            {
                //如果字典中记录的信息为null 说明该包正在加载中
                //等待其加载结束
                while (abDic[strs[i]] == null)
                {
                    //等待一帧
                    yield return 0;
                }
            }
        }
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            //同步加载
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return abcr;
                abDic[abName] = abcr.assetBundle;
            }

        }
        else
        {
            //如果字典中记录的信息为null 说明该包正在加载中
            //等待其加载结束
            while (abDic[abName] == null)
            {
                //等待一帧
                yield return 0;
            }
        }
        if (isSync)
        {
            //同步加载包中资源
            Object res = abDic[abName].LoadAsset(resName);
            callBack(res);
        }
        else
        {
            //异步加载包中资源
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName);
            yield return abq;
            callBack(abq.asset);
        }

    }

    //卸载AB包的方法
    public void UnLoadAB(string name, UnityAction<bool> callBackResult)
    {
        if (abDic.ContainsKey(name))
        {
            if (abDic[name] == null)
            {
                //代表正在异步加载 没有卸载成功
                callBackResult(false);
                return;
            }
            abDic[name].Unload(false);
            abDic.Remove(name);
            //卸载成功
            callBackResult(true);

        }
    }

    //清空AB包的方法
    public void ClearAB()
    {
        //由于AB包都是异步加载 因此在清理前 停止协同程序
        StopAllCoroutines();
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        //卸载主包
        mainAB = null;
    }
}
