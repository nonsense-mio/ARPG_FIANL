using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;
/// <summary>
/// Lua管理器
/// 提供 Lua解析器
/// 保证解析器的唯一性
/// </summary>
public class LuaMgr : BaseManager<LuaMgr>
{
    //执行lua语言的函数
    //释放垃圾
    //销毁
    //重定向
    private LuaEnv luaEnv;

    /// <summary>
    /// 得到Lua中的_G
    /// </summary>
    public LuaTable Global
    {
        get
        {
            return luaEnv.Global;
        }
    }
    private LuaMgr() { }

    /// <summary>
    /// 初始化解析器
    /// </summary>
    public void Init()
    {
        //已经初始化 就直接返回
        if (luaEnv != null)
            return;
        //初始化
        luaEnv = new LuaEnv();
        //加载lua脚本 重定向
        //luaEnv.AddLoader(MyCustomLoader);
        luaEnv.AddLoader(MyCustomABLoader);
    }

    private byte[] MyCustomLoader(ref string filePath)
    {
        //通过函数中的逻辑加载 lua文件

        //传入的参数 是require执行的lua脚本文件名
        //拼接一个lua文件所在路径
        string path = Application.dataPath + "/Lua/" + filePath + ".lua";

        //有路径 就加载文件
        if (File.Exists(path))
        {
            return File.ReadAllBytes(path);
        }
        else
        {
            Debug.Log("文件不存在" + filePath);
        }

        return null;
    }

    //重定向加载AB包中的Lua脚本
    private byte[] MyCustomABLoader(ref string filepath)
    {

        //通过我们的AB包管理器 加载的lua脚本资源
        byte[] luaBytes = null;
        ABMgr.Instance.LoadResAsync<TextAsset>("lua", filepath + ".lua", (lua) =>
        {
            if (lua != null)
                luaBytes = lua.bytes;             
        },true);
        if (luaBytes != null)
            return luaBytes;
        else
            Debug.Log("MyCustomABLoader重定向失败,文件名：" + filepath);

        return null;
    }

    /// <summary>
    /// 传入lua文件名 执行lua脚本
    /// </summary>
    /// <param name="fileName"></param>
    public void DoLuaFile(string fileName)
    {
        string str = string.Format("require('{0}')", fileName);
        DoString(str);
    }

    //lua脚本会放在AB包
    //最终我们会通过加载AB包再加载其中的lua脚本资源 来执行它
    //AB包中如果要加载文本 后缀还是有一定的限制 .lua不能被识别
    //打包时 要把lua文件后缀改为txt


    /// <summary>
    /// 执行lua语言
    /// </summary>
    /// <param name="str"></param>
    public void DoString(string str)
    {
        if (luaEnv == null)
        {
            Debug.Log("解析器未初始化");
            return;
        }
        luaEnv.DoString(str);
    }
    /// <summary>
    /// 释放lua垃圾
    /// </summary>
    public void Tick()
    {
        if (luaEnv == null)
        {
            Debug.Log("解析器未初始化");
            return;
        }
        luaEnv.Tick();
    }
    /// <summary>
    /// 销毁解析器
    /// </summary>
    public void Dispose()
    {
        if (luaEnv == null)
        {
            Debug.Log("解析器未初始化");
            return;
        }
        luaEnv.Dispose();
        luaEnv = null;
    }
}

