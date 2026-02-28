using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动挂载式的 继承MonoBehaviour的单例模式基类
/// 推荐使用
/// 无需手动挂载 无需关心切场景带来的问题
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        { 
            if (instance == null)
            {
                //动态创建 动态挂载
                //在场景上创建空物体
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();
                //动态挂在单例模式脚本
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
}
