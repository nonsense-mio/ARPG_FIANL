using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局存档槽位信息
/// 管理所有存档槽位的元数据，用于在存档列表界面显示
/// 该类本身也会被序列化保存到 SaveSlotInfo.json
/// </summary>
[Serializable]
public class SaveSlotInfo
{
    /// <summary>
    /// 最大存档槽位数量
    /// </summary>
    public static readonly int MAX_SLOTS = 4;

    /// <summary>
    /// 所有存档槽位的元数据列表
    /// </summary>
    public List<SaveSlotData> slots = new List<SaveSlotData>();

    /// <summary>
    /// 上次使用的存档槽位索引（-1表示没有）
    /// </summary>
    public int lastUsedSlot = -1;

    /// <summary>
    /// 构造函数 - 初始化所有槽位为空
    /// </summary>
    public SaveSlotInfo()
    {
        InitializeSlots();
    }

    /// <summary>
    /// 初始化所有存档槽位
    /// </summary>
    private void InitializeSlots()
    {
        slots = new List<SaveSlotData>(MAX_SLOTS);
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            slots.Add(new SaveSlotData());
        }
    }

    /// <summary>
    /// 获取指定槽位的数据
    /// </summary>
    public SaveSlotData GetSlot(int index)
    {
        if (index < 0 || index >= MAX_SLOTS)
        {
            Debug.LogError($"SaveSlotInfo: 无效的槽位索引 {index}");
            return null;
        }
        return slots[index];
    }

    /// <summary>
    /// 检查指定槽位是否为空
    /// </summary>
    public bool IsSlotEmpty(int index)
    {
        if (index < 0 || index >= MAX_SLOTS) return true;
        return slots[index].isEmpty;
    }

    /// <summary>
    /// 获取已使用的存档数量
    /// </summary>
    public int GetUsedSlotCount()
    {
        int count = 0;
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (!slots[i].isEmpty) count++;
        }
        return count;
    }

    /// <summary>
    /// 检查是否有可用存档（用于判断"继续游戏"按钮是否可用）
    /// </summary>
    public bool HasAnySave()
    {
        return GetUsedSlotCount() > 0;
    }
}
