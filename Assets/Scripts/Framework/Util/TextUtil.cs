using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 提供处理字符串方法的工具类
/// </summary>
public class TextUtil
{
    #region 字符串拆分相关

    /// <summary>
    /// 拆分字符串 返回字符串数组
    /// </summary>
    /// <param name="str">想要被拆分的字符串</param>
    /// <param name="type">拆分字符类型 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <returns></returns>
    public static string[] SplitStr(string str, int type = 1)
    {
        if (str == null)
            return new string[0];
        string newStr = str;
        if (type == 1)
        {
            //为了避免英文符号填成了中文符号 先进行替换
            while (newStr.IndexOf("；") != -1)
            {
                newStr = newStr.Replace("；", ";");
            }
            return newStr.Split(';');
        }
        else if (type == 2)
        {
            while (newStr.IndexOf("%") != -1)
            {
                newStr = newStr.Replace("，", ",");
            }
            return newStr.Split(',');
        }
        else if (type == 3)
        {
            return newStr.Split('%');
        }
        else if (type == 4)
        {
            while (newStr.IndexOf("：") != -1)
            {
                newStr = newStr.Replace("：", ":");
            }
            return newStr.Split(':');
        }
        else if (type == 5)
        {
            return newStr.Split(' ');
        }
        else if (type == 6)
        {
            return newStr.Split('|');
        }
        else if (type == 7)
        {
            return newStr.Split('_');
        }

        return new string[0];
    }

    /// <summary>
    /// 拆分字符串 返回整形数组
    /// </summary>
    /// <param name="str">想要被拆分的字符串</param>
    /// <param name="type">拆分字符类型 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <returns></returns>
    public static int[] SplitStrToIntArr(string str, int type = 1)
    {
        //得到拆分后的字符串数组
        string[] strs = SplitStr(str, type);
        if (str.Length == 0)
            return new int[0];
        //把字符串数组 转换成int 数组
        return Array.ConvertAll<string, int>(strs, (str) =>
        {
            return int.Parse(str);
        });
    }

    /// <summary>
    /// 专门用来拆分多组键值对形式的数据 以int返回
    /// </summary>
    /// <param name="str">待拆分的字符串</param>
    /// <param name="typeOne">组间的分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <param name="typeTwo">键值对分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <param name="callBack">回调函数</param>
    public static void SplitStrToIntArrTwice(string str, int typeOne, int typeTwo, UnityAction<int, int> callBack)
    {
        string[] strs = SplitStr(str, typeOne);
        if (strs.Length == 0)
            return;
        int[] ints;
        for (int i = 0; i < strs.Length; i++)
        {
            //拆分单个道具的ID和数量信息
            ints = SplitStrToIntArr(strs[i], typeTwo);
            if (ints.Length == 0)
                continue;
            callBack.Invoke(ints[0], ints[1]);
        }
    }

    /// <summary>
    /// 专门用来拆分多组键值对形式的数据 以string返回
    /// </summary>
    /// <param name="str">待拆分的字符串</param>
    /// <param name="typeOne">组间的分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <param name="typeTwo">键值对分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_</param>
    /// <param name="callBack">回调函数</param>
    public static void SplitStrTwice(string str, int typeOne, int typeTwo, UnityAction<string, string> callBack)
    {
        string[] strs = SplitStr(str, typeOne);
        if (strs.Length == 0)
            return;
        string[] strs2;
        for (int i = 0; i < strs.Length; i++)
        {
            //拆分单个道具的ID和数量信息
            strs2 = SplitStr(strs[i], typeTwo);
            if (strs2.Length == 0)
                continue;
            callBack.Invoke(strs2[0], strs2[1]);
        }
    }
    #endregion

    #region 数字转字符串相关
    /// <summary>
    /// 得到指定长度的数字转字符串内容  如果长度不够会在数字前补0 如果长度超出会保持原数字长度
    /// </summary>
    /// <param name="value">传入的数字</param>
    /// <param name="len">指定长度</param>
    /// <returns></returns>
    public static string GetNumStr(int value,int len)
    {
        //tostring中传入一个 D{n}的字符串
        //代表想要将数字转换为长度为n的字符串  长度不够会在前面补0
        return value.ToString($"D{len}");
    }

    /// <summary>
    /// 让指定浮点数保留 小数点后n位
    /// </summary>
    /// <param name="value">传入的浮点数</param>
    /// <param name="len">保留小数点后的位数</param>
    /// <returns></returns>
    public static string GetDecimalStr(float value,int len)
    {
        //tostring中传入一个 F{n}的字符串
        //代表想要保留小数点后n位小数 
        return value.ToString($"F{len}");
    }
    /// <summary>
    /// 将较大较长的数 转换为字符串
    /// </summary>
    /// <param name="num">数字</param>
    /// <returns>n亿n千万 或 n万n千</returns>
    public static string GetBigDataToString(int num)
    {
        //如果大于1亿 就显示 n亿n千万
        if(num > 100000000)
        {
            return BigDataChange(num, 100000000, "亿", "千万");
        }
        //如果大于1万 显示 n万n千
        else if(num > 10000)
        {
            return BigDataChange(num, 10000, "万", "千");
        }
        else 
            return num.ToString();
        
    }
    /// <summary>
    /// 把大数据转换成对应的字符串拼接
    /// </summary>
    /// <param name="num">数值</param>
    /// <param name="company">分割单位</param>
    /// <param name="bigCompany">大单位 亿 万</param>
    /// <param name="littleCompany">小单位 千万 千</param>
    /// <returns></returns>
    private static string BigDataChange(int num,int company,string bigCompany,string littleCompany)
    {
        resultStr.Clear();
        //有几亿 几万
        resultStr.Append(num / company);
        resultStr.Append(bigCompany);
        //有几千万 几千
        int tempNum = num % company;
        tempNum /= (company/10);
        if (tempNum != 0)
        {
            resultStr.Append(tempNum);
            resultStr.Append(littleCompany);
        }
        return resultStr.ToString();
    }

    #endregion

    #region 时间转换相关
    private static StringBuilder resultStr = new StringBuilder("");
    /// <summary>
    /// 秒转时分秒
    /// </summary>
    /// <param name="s">秒数</param>
    /// <param name="egZero">是否忽略0</param>
    /// <param name="isKeepLen">是否保留至少两位</param>
    /// <param name="hourStr">小时的拼接字符</param>
    /// <param name="minuteStr">分钟的拼接字符</param>
    /// <param name="secondStr">秒的拼接字符</param>
    /// <returns></returns>
    public static string SecondToHMS(int s,bool egZero = false,bool isKeepLen = false ,string hourStr = "时",string minuteStr = "分",string secondStr = "秒")
    {
        if(s<0)
            s= 0;
        //计算小时
        int hour = s / 3600;
        //计算分钟
        int second = s % 3600;
        int minute = second / 60;
        //计算秒
        second = s % 60;
        //拼接
        resultStr.Clear();
        if(hour !=0 || !egZero)
        {
            resultStr.Append(isKeepLen ? GetNumStr(hour,2) : hour);
            resultStr.Append(hourStr);
        }
        if(minute !=0 || !egZero || hour!=0)
        {
            resultStr.Append(isKeepLen ? GetNumStr(minute, 2) : minute);
            resultStr.Append(minuteStr);
        }
        if(second !=0 || ! egZero || hour!=0 || second != 0)
        {
            resultStr.Append(isKeepLen ? GetNumStr(second, 2) : second);
            resultStr.Append(secondStr);
        } 
        //如果是0秒时
        if(resultStr.Length == 0)
        {
            resultStr.Append(isKeepLen ? GetNumStr(second, 2) : second);
            resultStr.Append(secondStr);
        }
        return resultStr.ToString();
    }
    /// <summary>
    /// 秒转 00:00:00格式
    /// </summary>
    /// <param name="s"></param>
    /// <param name="egZero"></param>
    /// <returns></returns>
    public static string SecondToHMS2(int s,bool egZero = false)
    {
        return SecondToHMS(s,egZero,true,":",":","");
    }
    #endregion


}
