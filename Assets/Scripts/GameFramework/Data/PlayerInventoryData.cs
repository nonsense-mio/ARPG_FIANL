using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家库存数据类 - 用于JSON序列化存储
/// 存储所有物品的ID而非物品对象本身（ScriptableObject无法直接序列化）
/// </summary>
public class PlayerInventoryData
{
    private const int SLOT_SIZE = 4;
    private const int EMPTY_WEAPON_ID = 1000;    // 空武器槽位ID
    private const int EMPTY_HELMET_ID = 2000; // 空头盔槽位ID
    private const int EMPTY_BODY_ID = 3000;   // 空身体装备槽位ID
    private const int EMPTY_LEGS_ID = 4000;   // 空腿部装备槽位ID
    private const int EMPTY_HANDS_ID = 5000;  // 空手部装备槽位ID
    private const int EMPTY_CONSUMABLE_ID = 6000; // 空消耗品槽位ID
    private const int EMPTY_SPELL_ID = 7000;      // 空法术槽位ID

    #region 库存列表 - 存储物品ID列表

    /// <summary>
    /// 武器库存ID列表
    /// </summary>
    public List<int> weaponInventoryIDs = new List<int>();

    /// <summary>
    /// 头盔装备库存ID列表
    /// </summary>
    public List<int> headEquipmentInventoryIDs = new List<int>();

    /// <summary>
    /// 身体装备库存ID列表
    /// </summary>
    public List<int> bodyEquipmentInventoryIDs = new List<int>();

    /// <summary>
    /// 腿部装备库存ID列表
    /// </summary>
    public List<int> legEquipmentInventoryIDs = new List<int>();

    /// <summary>
    /// 手部装备库存ID列表
    /// </summary>
    public List<int> handEquipmentInventoryIDs = new List<int>();

    /// <summary>
    /// 消耗品库存ID列表
    /// </summary>
    public List<int> consumableInventoryIDs = new List<int>();

    /// <summary>
    /// 法术库存ID列表
    /// </summary>
    public List<int> spellInventoryIDs = new List<int>();

    #endregion

    #region 快速插槽 - 存储当前装备槽位的物品ID（-1表示空槽位）

    /// <summary>
    /// 右手武器槽位ID数组
    /// </summary>
    public List<int> rightHandSlotIDs = new List<int>();

    /// <summary>
    /// 左手武器槽位ID数组
    /// </summary>
    public List<int> leftHandSlotIDs = new List<int>();

    /// <summary>
    /// 消耗品槽位ID数组
    /// </summary>
    public List<int> consumableSlotIDs = new List<int>();

    /// <summary>
    /// 法术槽位ID数组
    /// </summary>
    public List<int> spellSlotIDs = new List<int>(){7001,7002};

    #endregion

    #region 当前装备 - 存储当前穿戴的装备ID（-1表示未装备）

    /// <summary>
    /// 当前头盔ID
    /// </summary>
    public int currentHelmetID = EMPTY_HELMET_ID;

    /// <summary>
    /// 当前身体装备ID
    /// </summary>
    public int currentBodyID = EMPTY_BODY_ID;

    /// <summary>
    /// 当前腿部装备ID
    /// </summary>
    public int currentLegsID = EMPTY_LEGS_ID;

    /// <summary>
    /// 当前手部装备ID
    /// </summary>
    public int currentHandsID = EMPTY_HANDS_ID;

    #endregion

    #region 当前索引 - 存储快速插槽的当前选中索引

    /// <summary>
    /// 当前右手武器索引
    /// </summary>
    public int currentRightWeaponIndex = 0;

    /// <summary>
    /// 当前左手武器索引
    /// </summary>
    public int currentLeftWeaponIndex = 0;

    /// <summary>
    /// 当前消耗品索引
    /// </summary>
    public int currentConsumableIndex = 0;

    /// <summary>
    /// 当前法术索引
    /// </summary>
    public int currentSpellIndex = 0;

    #endregion


    #region 构造函数

    /// <summary>
    /// 默认构造函数 - 初始化所有快速插槽为固定4格空槽位
    /// </summary>
    public PlayerInventoryData()
    {
        InitializeSlots();
    }

    /// <summary>
    /// 初始化所有快速插槽为固定大小的空槽位
    /// </summary>
    private void InitializeSlots()
    {
        // 清空并填充固定大小的空槽位
        rightHandSlotIDs = CreateEmptySlots(EMPTY_WEAPON_ID);
        leftHandSlotIDs = CreateEmptySlots(EMPTY_WEAPON_ID);
        consumableSlotIDs = CreateEmptySlots(EMPTY_CONSUMABLE_ID);
        spellSlotIDs = CreateEmptySlots(EMPTY_SPELL_ID);
    }

    /// <summary>
    /// 创建一个固定大小的空槽位列表
    /// </summary>
    private List<int> CreateEmptySlots(int emptyID)
    {
        List<int> slots = new List<int>(SLOT_SIZE);
        for (int i = 0; i < SLOT_SIZE; i++)
        {
            slots.Add(emptyID);
        }
        return slots;
    }

    #endregion
}
