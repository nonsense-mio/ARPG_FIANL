using System.Collections.Generic;
using System.IO;
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine;

public class HybridCLRTool
{
    private static readonly string HotDllOutputPath = $"{Application.dataPath}/AB/HotDll/";

    [MenuItem("Tools/HybridCLR工作流/复制全部 DLL (热更+AOT)")]
    public static void CopyAllDlls()
    {
        CopyHotUpdateDlls();
        CopyAOTMetadataDlls();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[HybridCLR工作流] 全部 DLL 复制完成（热更 + AOT 元数据）");
    }

    [MenuItem("Tools/HybridCLR工作流/仅复制热更 DLL")]
    public static void CopyHotUpdateDlls()
    {
        List<string> assemblys = SettingsUtil.HotUpdateAssemblyNamesExcludePreserved;
        string srcDir = $"{Application.dataPath}/../HybridCLRData/HotUpdateDlls/{EditorUserBuildSettings.activeBuildTarget}";

        if (!Directory.Exists(srcDir))
        {
            Debug.LogError($"[HybridCLR工作流] 热更 DLL 目录不存在: {srcDir}\n请先执行 HybridCLR > CompileDll");
            return;
        }

        EnsureOutputDirectory();

        int count = 0;
        foreach (FileInfo file in new DirectoryInfo(srcDir).GetFiles("*.dll"))
        {
            string nameWithoutExt = Path.GetFileNameWithoutExtension(file.Name);
            if (assemblys.Contains(nameWithoutExt))
            {
                file.CopyTo(HotDllOutputPath + file.Name + ".bytes", true);
                count++;
                Debug.Log($"  [热更] {file.Name} → {file.Name}.bytes");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[HybridCLR工作流] 热更 DLL 复制完成，共 {count} 个");
    }

    [MenuItem("Tools/HybridCLR工作流/仅复制 AOT 元数据 DLL")]
    public static void CopyAOTMetadataDlls()
    {
        string srcDir = $"{Application.dataPath}/../HybridCLRData/AssembliesPostIl2CppStrip/{EditorUserBuildSettings.activeBuildTarget}";

        if (!Directory.Exists(srcDir))
        {
            Debug.LogError($"[HybridCLR工作流] AOT 裁剪目录不存在: {srcDir}\n请先执行一次 Build Player 生成裁剪后的 AOT 程序集");
            return;
        }

        EnsureOutputDirectory();

        int count = 0;
        foreach (string dllName in AOTGenericReferences.PatchedAOTAssemblyList)
        {
            string srcFile = Path.Combine(srcDir, dllName);
            if (File.Exists(srcFile))
            {
                string destFile = HotDllOutputPath + dllName + ".bytes";
                File.Copy(srcFile, destFile, true);
                count++;
                Debug.Log($"  [AOT] {dllName} → {dllName}.bytes");
            }
            else
            {
                Debug.LogWarning($"  [AOT] 未找到 {dllName}，跳过（路径: {srcFile}）");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[HybridCLR工作流] AOT 元数据 DLL 复制完成，共 {count} 个");
    }

    private static void EnsureOutputDirectory()
    {
        if (!Directory.Exists(HotDllOutputPath))
            Directory.CreateDirectory(HotDllOutputPath);
    }
}
