using System.Collections.Generic;
using System.Linq;
using HT;
using UnityEngine;

/// <summary>
/// 物品数据库 - 存储所有物品的ScriptableObject引用
/// 通过Resources加载，无需放置在场景中
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
                // 从Resources/Data目录加载
                _instance = ResMgr.Instance.Load<ItemDataBase>("Data/ItemDataBase");
                if (_instance == null)
                {
                    Debug.LogError("ItemDataBase not found in Resources/Data/ItemDataBase!");
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

    #region 通过ID获取物品的方法

    public WeaponItem_SO GetWeaponByID(int id)
    {
        if (id < 0) return null;
        return weaponItems.FirstOrDefault(item => item != null && item.itemID == id);
    }

    public HelmetEquipment GetHelmetByID(int id)
    {
        if (id < 0) return null;
        return helmetItems.FirstOrDefault(item => item != null && item.itemID == id);
    }

    public BodyEquipment GetBodyByID(int id)
    {
        if (id < 0) return null;
        return bodyItems.FirstOrDefault(item => item != null && item.itemID == id);
    }

    public LegEquipment GetLegByID(int id)
    {
        if (id < 0) return null;
        return legItems.FirstOrDefault(item => item != null && item.itemID == id);
    }

    public HandEquipment GetHandByID(int id)
    {
        if (id < 0) return null;
        return handItems.FirstOrDefault(item => item != null && item.itemID == id);
    }

    public ConsumableItem_SO GetConsumableByID(int id)
    {
        if (id < 0) return null;
        return consumableItems.FirstOrDefault(item => item != null && item.itemID == id);
    }

    public SpellItem GetSpellByID(int id)
    {
        if (id < 0) return null;
        return spellItems.FirstOrDefault(item => item != null && item.itemID == id);
    }

    #endregion
}
