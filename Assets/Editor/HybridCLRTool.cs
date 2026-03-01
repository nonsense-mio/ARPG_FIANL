using System.Collections;
using System.Collections.Generic;
using System.IO;
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine;

public class HybridCLRTool
{
    [MenuItem("Tools/HybridCLR工作流")]
    public static void HybridCLRBuild()
    {
        //获取 需要热更新程序集 的信息
        //获取到程序集 打包后 所在的路径
        //获取到指定路径文件夹的 所有文件
        //循环 所有文件 和 热更新的文件对比
        //如果有想要的指定文件 那就把它复制到指定的aa包的资源文件夹中

        List<string> assemblys = SettingsUtil.HotUpdateAssemblyNamesExcludePreserved;

        string Path = ($"{Application.dataPath}/../HybridCLRData/HotUpdateDlls/{EditorUserBuildSettings.activeBuildTarget}");

        DirectoryInfo directoryInfo = new DirectoryInfo(Path);

        FileInfo[] files = directoryInfo.GetFiles();

        string path = ($"{Application.dataPath}/AB/HotDll/");
        foreach (FileInfo file in files)
        {
            //Debug.Log(file.Name);
            if (file.Extension == ".dll" && assemblys.Contains(file.Name.Substring(0, file.Name.Length - 4)))
            {
                //复制 Dll
                file.CopyTo(path + file.Name + ".bytes", true);
            }
        }

        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();

        Debug.Log("Copy Dlls Complete !");


    }
}
