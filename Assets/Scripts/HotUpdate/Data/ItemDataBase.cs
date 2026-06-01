using System.Collections.Generic;
using ARPG;
using UnityEngine;

/// <summary>
/// 物品数据库 - 存储所有物品的ScriptableObject引用
/// 通过 YooAsset 加载，无需放置在场景中
/// </summary>
[CreateAssetMenu(menuName = "Data/ItemDataBase")]
public class ItemDataBase : ScriptableObject
{
    private static ItemDataBase _instance;
    public static ItemDataBase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameArchitecture.Interface.GetUtility<IAssetLoader>().LoadSync<ItemDataBase>("data/ItemDataBase");
                if (_instance == null)
                {
                    Debug.LogError("ItemDataBase not found! Ensure it is configured in YooAsset collector (group_data).");
                }
            }
            return _instance;
        }
    }

    #region 物品列表 - 在Inspector中配置所有物品

    [Header("武器列表")]
    public List<WeaponItem_SO> weaponItems = new List<WeaponItem_SO>();

    [Header("装备列表")]
    public List<HelmetEquipment> helmetItems = new List<HelmetEquipment>();
    public List<BodyEquipment> bodyItems = new List<BodyEquipment>();
    public List<LegEquipment> legItems = new List<LegEquipment>();
    public List<HandEquipment> handItems = new List<HandEquipment>();

    [Header("消耗品和法术列表")]
    public List<ConsumableItem_SO> consumableItems = new List<ConsumableItem_SO>();
    public List<SpellItem> spellItems = new List<SpellItem>();

    #endregion

    #region Dictionary 缓存

    private Dictionary<int, Item_SO> _itemCache;

    private void EnsureCacheBuilt()
    {
        if (_itemCache != null) return;
        _itemCache = new Dictionary<int, Item_SO>();
        CacheList(weaponItems);
        CacheList(helmetItems);
        CacheList(bodyItems);
        CacheList(legItems);
        CacheList(handItems);
        CacheList(consumableItems);
        CacheList(spellItems);
    }

    private void CacheList<T>(List<T> list) where T : Item_SO
    {
        foreach (var item in list)
            if (item != null) _itemCache[item.itemID] = item;
    }

    #endregion

    #region 通过ID获取物品的方法

    /// <summary>
    /// 泛型查找 — O(1) Dictionary 查询，按类型过滤
    /// </summary>
    public T GetItemByID<T>(int id) where T : Item_SO
    {
        if (id < 0) return null;
        EnsureCacheBuilt();
        return _itemCache.TryGetValue(id, out var item) ? item as T : null;
    }

    /// <summary>
    /// 非泛型查找 — 根据 ID 直接返回 Item_SO
    /// </summary>
    public Item_SO GetItemByID(int id) => GetItemByID<Item_SO>(id);

    // 便捷类型方法 (向后兼容)
    public WeaponItem_SO GetWeaponByID(int id) => GetItemByID<WeaponItem_SO>(id);
    public HelmetEquipment GetHelmetByID(int id) => GetItemByID<HelmetEquipment>(id);
    public BodyEquipment GetBodyByID(int id) => GetItemByID<BodyEquipment>(id);
    public LegEquipment GetLegByID(int id) => GetItemByID<LegEquipment>(id);
    public HandEquipment GetHandByID(int id) => GetItemByID<HandEquipment>(id);
    public ConsumableItem_SO GetConsumableByID(int id) => GetItemByID<ConsumableItem_SO>(id);
    public SpellItem GetSpellByID(int id) => GetItemByID<SpellItem>(id);

    #endregion
}
