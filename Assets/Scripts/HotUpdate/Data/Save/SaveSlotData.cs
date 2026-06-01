using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单个存档槽位的元数据
/// 用于在存档列表中显示每个存档的基本信息
/// </summary>
[Serializable]
public class SaveSlotData
{
    /// <summary>
    /// 该槽位是否为空（未使用）
    /// </summary>
    public bool isEmpty = true;

    /// <summary>
    /// 玩家角色名称
    /// </summary>
    public string playerName = "";

    /// <summary>
    /// 玩家等级
    /// </summary>
    public int playerLevel = 1;

    /// <summary>
    /// 存档时间（格式化字符串，便于显示）
    /// </summary>
    public string saveTime = "";

    /// <summary>
    /// 累计游戏时长（秒）
    /// </summary>
    public float playTimeSeconds = 0f;

    /// <summary>
    /// 当前所在场景名称
    /// </summary>
    public string sceneName = "";

 

    /// <summary>
    /// 最大生命值
    /// </summary>
    public int maxHealth = 100;

    /// <summary>
    /// 最大耐力值
    /// </summary>
    public int maxStamina = 100;

    /// <summary>
    /// 最大专注值
    /// </summary>
    public int maxFocus = 100;

    /// <summary>
    /// 当前魂量
    /// </summary>
    public int currentSouls = 0;



    /// <summary>
    /// 获取格式化的游戏时长字符串
    /// </summary>
    public string GetFormattedPlayTime()
    {
        int hours = (int)(playTimeSeconds / 3600);
        int minutes = (int)((playTimeSeconds % 3600) / 60);
        return $"{hours}H {minutes}M";
    }


    /// <summary>
    /// 更新存档时间为当前时间
    /// </summary>
    public void UpdateSaveTime()
    {
        saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// 重置为空槽位状态
    /// </summary>
    public void Reset()
    {
        isEmpty = true;
        playerName = "";
        playerLevel = 1;
        saveTime = "";
        playTimeSeconds = 0f;
        sceneName = "";
        maxHealth = 100;
        maxStamina = 100;
        maxFocus = 100;
        currentSouls = 0;
    }
}
