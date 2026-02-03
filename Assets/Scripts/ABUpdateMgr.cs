using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ABUpdateMgr : MonoBehaviour
{
    private static ABUpdateMgr instance;
    public static ABUpdateMgr Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ABUpdateMgr");
                instance = go.AddComponent<ABUpdateMgr>();
            }
            return instance;
        }
    }

    //远程AB包信息字典
    private Dictionary<string, ABInfo> remoteABInfoDic = new Dictionary<string, ABInfo>();
    //本地AB包信息字典
    private Dictionary<string, ABInfo> localABInfoDic = new Dictionary<string, ABInfo>();
    //待下载的AB包列表 存储AB包的名字
    private List<string> downLoadList = new List<string>();
    private string serverIP = "ftp://127.0.0.1";

    /// <summary>
    /// 用于检测热更新的函数
    /// </summary>
    /// <param name="overCallBack"></param>
    /// <param name="updateInfoCallBack"></param>
    public void CheckUpdate(UnityAction<bool> overCallBack, UnityAction<string> updateInfoCallBack)
    {
        remoteABInfoDic.Clear();
        localABInfoDic.Clear();
        downLoadList.Clear();
        //1.加载远端资源对比文件
        DownLoadABCompareFile((isOver) =>
        {
            if (isOver)
            {
          
                string remoteInfo = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
         
                GetRemoteABCompareFileInfo(remoteInfo, remoteABInfoDic);

                //2.加载本地资源对比文件
                GetLocalABCompareFileInfo((isOver) =>
                {
                    if (isOver)
                    {
                        //3.对比资源 对比出需要下载的AB包列表
                        foreach(string abName in remoteABInfoDic.Keys)
                        {
                            //若本地没有该AB包 则加入下载列表
                            if (!localABInfoDic.ContainsKey(abName))
                            {
                                downLoadList.Add(abName);
                            }
                            //发现本地有同名AB包时 则比较MD5码
                            else
                            {
                                if(localABInfoDic[abName].md5 != remoteABInfoDic[abName].md5)
                                {
                                    downLoadList.Add(abName);
                                }
                                localABInfoDic.Remove(abName);//将比对过的AB包信息从本地字典中移除 剩下的就是多余的AB包
                            }
                        }

                        //删除本地多余的AB包
                        foreach(string abName in localABInfoDic.Keys)
                        {
                            //删除本地多余的AB包
                            if(File.Exists(Application.persistentDataPath + "/" + abName))
                            {
                                File.Delete(Application.persistentDataPath + "/" + abName);
                            }
                        }

                        //4.下载AB包
                        DownLoadABFile((isOver) =>
                        {
                            if (isOver)
                            {
                                //更新本地的AB包对比文件
                                //把远端对比文件信息存储到本地
                                File.WriteAllText(Application.persistentDataPath + "/ABCompareInfo.txt", remoteInfo);
                            } 
                            overCallBack?.Invoke(isOver);  
                        },updateInfoCallBack);
                    }   
                    else
                        overCallBack?.Invoke(false);
                    

                });

            }
            else
            {
                overCallBack?.Invoke(false);
            }
        });




    }


    //下载AB包资源对比文件
    public async void DownLoadABCompareFile(UnityAction<bool> overCallBack)
    {
        bool isOver = false;
        int reDownLoadCount = 5;//最大重试下载次数
        string localPath = Application.persistentDataPath + "/ABCompareInfo_TMP.txt";
        while (!isOver && reDownLoadCount > 0)
        {
            await Task.Run(() =>
            {
                //1.从资源服务器下载资源对比文件
                isOver = DownLoadFile("ABCompareInfo.txt", localPath);
            });
            reDownLoadCount--;
        }
        overCallBack?.Invoke(isOver);
    }


    /// <summary>
    /// 获取下载下来的AB包中的信息
    /// </summary>
    /// <param name="info">资源对比文件中的字符串信息</param>
    /// <param name="abInfoDic">存储AB包信息的字典</param> <summary>
    public void GetRemoteABCompareFileInfo(string info, Dictionary<string, ABInfo> abInfoDic)
    {
        //2.获取资源对比文件中的 字符串信息 进行拆分
        //string info = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
        string[] strs = info.Split('|');
        string[] infos = null;
        for (int i = 0; i < strs.Length; i++)
        {
            infos = strs[i].Split(' ');
            //添加AB包信息到字典中
            abInfoDic.Add(infos[0], new ABInfo(infos[0], infos[1], infos[2]));
        }
    }


    //获取本地AB包信息
    public void GetLocalABCompareFileInfo(UnityAction<bool> overCallBack)
    {
        //
        if (File.Exists(Application.persistentDataPath + "/ABCompareInfo.txt"))
        {
            StartCoroutine(GetLocalABCompareFileInfo("file:///" + Application.persistentDataPath + "/ABCompareInfo.txt", overCallBack));
        }
        //当本地持久化目录没有对比文件时 去StreamingAssets目录下找(第一次运行时会用到)
        else if (File.Exists(Application.streamingAssetsPath + "/ABCompareInfo.txt"))
        {
            string path = 
#if UNITY_ANDROID
Application.streamingAssetsPath;
#else 
"file:///" + Application.streamingAssetsPath;
#endif
            StartCoroutine(GetLocalABCompareFileInfo( path + "/ABCompareInfo.txt", overCallBack));
        }
        //都没有则认为本地没有AB包资源
        else
        {
            overCallBack?.Invoke(true);
        }
    }

    private IEnumerator GetLocalABCompareFileInfo(string filepath, UnityAction<bool> overCallBack)
    {
        //通过 UnityWebRequest 下载本地文件
        UnityWebRequest req = UnityWebRequest.Get(filepath);
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            GetRemoteABCompareFileInfo(req.downloadHandler.text, localABInfoDic);
            overCallBack?.Invoke(true);
        }
        else
        {
            overCallBack?.Invoke(false);
        }

    }


    //下载AB包
    public async void DownLoadABFile(UnityAction<bool> overCallBack, UnityAction<string> progressCallBack = null)
    {
        string localPath = Application.persistentDataPath + "/";
        bool isOver = false;
        List<string> tempList = new List<string>(downLoadList);
        int reDownLoadCount = 5;//最大重试下载次数
        int downLoadOverCount = 0;//记录下载完成的文件数
        int downLoadCount = downLoadList.Count;
        while (downLoadList.Count > 0 && reDownLoadCount > 0)
        {
            for (int i = 0; i < downLoadList.Count; i++)
            {
                isOver = false;
                await Task.Run(() =>
                {
                    isOver = DownLoadFile(downLoadList[i], localPath + downLoadList[i]);
                });
                if (isOver)
                {
                    progressCallBack?.Invoke(++downLoadOverCount + "/" + downLoadCount);
                    tempList.Add(downLoadList[i]);//记录已下载成功的AB包
                }
            }
            //把下载成功的文件名 从待下载列表中移除
            for (int i = 0; i < tempList.Count; i++)
            {
                downLoadList.Remove(tempList[i]);
            }
            reDownLoadCount--;
        }

        //告诉外界下载是否完成
        overCallBack?.Invoke(downLoadList.Count == 0);
    }

    //下载文件
    private bool DownLoadFile(string fileName, string localPath)
    {
        try
        {
            string pInfo = 
#if UNITY_IOS
"IOS";
#elif UNITY_ANDROID
"Android";
#else
"PC";
#endif
            //1.创建一个FTP连接 用于下载
            FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + "/AB/" + pInfo + "/" + fileName)) as FtpWebRequest;
            //2.设置通信凭证
            NetworkCredential n = new NetworkCredential("HHT", "HHT123");
            req.Credentials = n;
            //3.其它设置
            req.Proxy = null;
            req.KeepAlive = false;
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.UseBinary = true;
            //4.下载文件
            FtpWebResponse res = req.GetResponse() as FtpWebResponse;
            Stream downloadStream = res.GetResponseStream();

            using (FileStream fs = File.Create(localPath))
            {
                byte[] bytes = new byte[2048];
                int contentLength = downloadStream.Read(bytes, 0, bytes.Length);

                while (contentLength != 0)
                {
                    //写入到本地文件流中
                    fs.Write(bytes, 0, contentLength);
                    contentLength = downloadStream.Read(bytes, 0, bytes.Length);
                }
                fs.Close();
                downloadStream.Close();
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Log(fileName + "下载失败" + e.Message);
            return false;
        }
    }

    void OnDestroy()
    {
        instance = null;
    }

    public class ABInfo
    {
        public string name;
        public long size;
        public string md5;

        public ABInfo(string name, string size, string md5)
        {
            this.name = name;
            this.size = long.Parse(size);
            this.md5 = md5;
        }
    }

}


