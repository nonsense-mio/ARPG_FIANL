using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ABTools : EditorWindow
{
    private int nowSelIndex = 0;
    private string[] targetStrings = new string[] { "PC", "Android", "iOS" };
    private string serverIP = "ftp://127.0.0.1";

    [MenuItem("AB包工具/打开工具窗口")]
    private static void OpenWindow()
    {
        //获取一个ABTools 编辑器窗口对象
        ABTools window = EditorWindow.GetWindowWithRect<ABTools>(new Rect(0, 0, 300, 200));
        window.Show();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 150, 20), "平台选择");
        nowSelIndex = GUI.Toolbar(new Rect(10, 30, 280, 20), nowSelIndex, targetStrings);
        //资源服务器IP
        GUI.Label(new Rect(10, 60, 150, 20), "资源服务器IP");
        serverIP = GUI.TextField(new Rect(10, 80, 280, 20), serverIP);

        //创建对比文件按钮
        if (GUI.Button(new Rect(10, 112, 140, 30), "创建对比文件"))
        {
            CreateABCompareFile();
        }
        //保存默认资源到StreamingAssets目录按钮
        if (GUI.Button(new Rect(150, 112, 140, 30), "保存默认资源"))
        {
            MoveABToStreamingAssets();
        }
        //上传AB包和对比文件按钮
        if (GUI.Button(new Rect(10, 155, 140, 30), "上传AB包和对比文件"))
        {
            UploadAllABFile();
        }
        //把Lua文件拷贝到Txt文件中 并设置AB包名
        if (GUI.Button(new Rect(150, 155, 140, 30), "拷贝Lua文件"))
        {
            CopyLuaToTxt();
        }
    }
    //把Lua文件拷贝到Txt文件中 并设置AB包名
    public static void CopyLuaToTxt()
    {
        //首先找到所有的lua文件
        string path = Application.dataPath + "/ArtRes/Lua/";
        //判断路径是否存在
        if (!Directory.Exists(path))
            return;
        //得到每一个lua文件的路径 才能进行迁移拷贝
        string[] strs = Directory.GetFiles(path, "*.lua");

        //然后把lua文件拷贝到一个新的文件夹中
        //首先定一个新路径
        string newPath = Application.dataPath + "/Editor/ArtRes/lua/";
        //判断新路径文件夹是否存在
        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
        //文件夹存在 先清空其中的内容
        else
        {
            //得到该路径中 所有后缀为.txt的文件 把他们全删除了
            string[] oldFileStrs = Directory.GetFiles(newPath, "*.txt");
            for (int i = 0; i < oldFileStrs.Length; i++)
            {
                File.Delete(oldFileStrs[i]);
            }
        }
        List<string> newFileNames = new List<string>();
        string fileName;
        for (int i = 0; i < strs.Length; i++)
        {
            //得到新的文件路径 用于拷贝
            fileName = newPath + strs[i].Substring(strs[i].LastIndexOf("/") + 1) + ".txt";
            newFileNames.Add(fileName);
            File.Copy(strs[i], fileName);
        }
        AssetDatabase.Refresh();
        //遍历新的文件路径
        for (int i = 0; i < newFileNames.Count; i++)
        {
            //该API传入的路径 必须是 相对 Assets文件夹的 Assets/.../
            AssetImporter importer = AssetImporter.GetAtPath(newFileNames[i].Substring(newFileNames[i].IndexOf("Assets")));
            if (importer != null)
            {
                importer.assetBundleName = "lua";
            }
        }
    }
    //创建对比文件
    private void CreateABCompareFile()
    {
        //获取文件夹信息
        //根据选择的平台读取对应平台文件夹下的内容
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/" + targetStrings[nowSelIndex] + "/");
        //获取该文件夹下的所有文件信息
        FileInfo[] fileInfos = directory.GetFiles();
        //用于存储信息的字符串
        string abCompareInfo = "";
        foreach (FileInfo info in fileInfos)
        {
            //没有后缀的才是AB包
            if (info.Extension == "")
            {
                Debug.Log("处理文件：" + info.Name);
                //拼接一个AB包的信息 行格式：文件名 文件大小 MD5码
                abCompareInfo += info.Name + " " + info.Length + " " + GetMD5(info.FullName);
                //用一个分隔符分开不同文件之间的信息
                abCompareInfo += '|';

            }
        }

        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);

        //将拼接好的AB包资源信息 写入到对比文件中                  所选平台
        File.WriteAllText(Application.dataPath + "/ArtRes/AB/" + targetStrings[nowSelIndex] + "/ABCompareInfo.txt", abCompareInfo);
        AssetDatabase.Refresh();
        Debug.Log("创建AB包对比文件完成");
    }
    //生成文件的MD5码
    private static string GetMD5(string filePath)
    {
        //将文件以流的形式打开
        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            //声明一个MD5对象 用于生成MD5码
            MD5 md5 = new MD5CryptoServiceProvider();
            //利用API 得到数据的MD5码 16个字节 数组
            byte[] md5Info = md5.ComputeHash(file);
            //关闭文件流
            file.Close();

            //把16个字节 转换为16进制 拼接成字符串 减少md5码的长度
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Info.Length; i++)
            {
                sb.Append(md5Info[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }

    //移动选中的AB包资源到StreamingAssets目录下
    private void MoveABToStreamingAssets()
    {
        //获取在Project窗口中选中的资源
        UnityEngine.Object[] selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectedAssets.Length == 0)
        {
            return;
        }
        //用于拼接本地默认AB包资源信息的字符串
        string abCompareInfo = "";


        //遍历选中的资源
        foreach (UnityEngine.Object obj in selectedAssets)
        {
            //获取资源路径
            string assetPath = AssetDatabase.GetAssetPath(obj);
            //截取路径当中的文件名 用于作为StreamingAssets中的文件名
            string fileName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
            //过滤掉有后缀的文件 只处理没有后缀的AB包文件
            if (fileName.IndexOf('.') != -1)
            {
                continue;
            }


            //将选中文件复制到StreamingAssets目录下
            AssetDatabase.CopyAsset(assetPath, "Assets/StreamingAssets/" + fileName);

            //获取拷贝到StreamingAssets目录下的文件信息
            FileInfo fileInfo = new FileInfo(Application.streamingAssetsPath + "/" + fileName);

            abCompareInfo += fileInfo.Name + " " + fileInfo.Length + " " + GetMD5(fileInfo.FullName);
            abCompareInfo += '|';

        }
        //去掉最后一个分隔符
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);
        File.WriteAllText(Application.streamingAssetsPath + "/ABCompareInfo.txt", abCompareInfo);
        AssetDatabase.Refresh();
    }

    //上传AB包和对比文件
    private void UploadAllABFile()
    {
        //获取文件夹信息
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/" + targetStrings[nowSelIndex] + "/");
        //获取该文件夹下的所有文件信息
        FileInfo[] fileInfos = directory.GetFiles();
        foreach (FileInfo info in fileInfos)
        {
            //没有后缀的才是AB包 资源对比文件后缀txt
            if (info.Extension == "" || info.Extension == ".txt")
            {
                //上传文件
                UploadFile(info.FullName, info.Name);
            }
        }
    }

    private async void UploadFile(string filePath, string fileName)
    {
        await Task.Run(() =>
        {
            try
            {
                //1.创建一个FTP连接 用于上传
                FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + "/AB/" + targetStrings[nowSelIndex] + "/" + fileName)) as FtpWebRequest;
                //2.设置通信凭证
                NetworkCredential n = new NetworkCredential("HHT", "HHT123");
                req.Credentials = n;
                //3.其它设置
                req.Proxy = null;
                req.KeepAlive = false;
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.UseBinary = true;
                //4.上传文件
                Stream upLoadStream = req.GetRequestStream();
                //读取文件内容 写入到上传流中
                using (FileStream fs = File.OpenRead(filePath))
                {
                    byte[] bytes = new byte[2048];
                    int contentLength = fs.Read(bytes, 0, bytes.Length);

                    while (contentLength != 0)
                    {
                        //写入到上传流中
                        upLoadStream.Write(bytes, 0, contentLength);
                        contentLength = fs.Read(bytes, 0, bytes.Length);
                    }
                    fs.Close();
                    upLoadStream.Close();
                }
                Debug.Log("上传文件完成：" + fileName);
            }
            catch (Exception e)
            {
                Debug.Log(fileName + "上传文件失败：" + e.Message);
            }
        });



    }
}
