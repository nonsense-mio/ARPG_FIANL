using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// 单例模式基类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseManager<T> where T : class
{
    private static T instance;
    //用于加锁的对象
    protected static readonly object lockObj = new object();
    
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        //利用反射得到无参私有的构造函数来用于对象的实例化
                        Type type = typeof(T);

                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                                                   null,
                                                                   Type.EmptyTypes,
                                                                   null);
                        if (info != null)
                            instance = info.Invoke(null) as T;
                        else
                            Debug.LogError("没有得到对应的无参构造");
                    }
                }
            }
            
            return instance;
        }
    }

}
