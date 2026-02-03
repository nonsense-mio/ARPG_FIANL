using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 加密工具类
/// </summary>
public class EncryptionUtil
{
    //1.获取随机密钥
    public static int GetRandomKey()
    {
        return Random.Range(1, 10000) + 5;
    }
    //2.加密数据
    public static int LockValue(int value,int key)
    {
        //主要采用异或加密
        value = value ^ (key % 9);
        value += key;
        return value;
    }
    public static long LockValue(long value, int key)
    {
        //主要采用异或加密
        value = value ^ (key % 9);
        value += key;
        return value;
    }
    //3.解密数据
    public static int UnLockValue(int value,int key)
    {
        if (value == 0)
            return value;
        value -= key;
        value = value ^ (key % 9);
        return value;
    }
    public static long UnLockValue(long value, int key)
    {
        if (value == 0)
            return value;
        value -= key;
        value = value ^ (key % 9);
        return value;
    }
}
