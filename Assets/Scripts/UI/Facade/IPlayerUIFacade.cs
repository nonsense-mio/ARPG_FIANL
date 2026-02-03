using System.Collections.Generic;

namespace HT
{
    /// <summary>
    /// UI 访问玩家数据与执行玩家相关命令的统一门面。
    /// UI 只依赖这个接口，不依赖 PlayerManager / PlayerInventoryManager 的具体实现。
    /// </summary>
    public interface IPlayerUIFacade
    {
        // ===== Stats (给 GamePanel / LevelUpPanel 等) =====
        #region 角色属性相关
        int MaxHealth { get; }
        int CurrentHealth { get; }
        float MaxStamina { get; }
        float CurrentStamina { get; }
        float MaxFocus { get; }
        float CurrentFocus { get; }

        int CurrentSoulCount { get; set; }
        #endregion

        #region 玩家等级及各属性等级
        int PlayerLevel { get; set; }
        int HealthLevel { get; set; }
        int StaminaLevel { get; set; }
        int FocusLevel { get; set; }
        int PoiseLevel { get; set; }
        int StrengthLevel { get; set; }
        int DexterityLevel { get; set; }
        int IntelligenceLevel { get; set; }
        int FaithLevel { get; set; }
        #endregion

        int SetMaxHealthFromHealthLevel();
        float SetMaxStaminaFromStaminaLevel();
        float SetMaxFocusPointsFromFocusLevel();


        // ===== Inventory Read (BagPanel) =====
        #region 背包面板读取库存列表的方法
        IReadOnlyList<Item_SO> GetWeaponInventory();
        IReadOnlyList<Item_SO> GetHelmetInventory();
        IReadOnlyList<Item_SO> GetBodyInventory();
        IReadOnlyList<Item_SO> GetLegInventory();
        IReadOnlyList<Item_SO> GetHandInventory();
        IReadOnlyList<Item_SO> GetConsumableInventory();
        IReadOnlyList<Item_SO> GetSpellInventory();
        List<Item_SO> GetAllInventorySnapshot();
        #endregion
        // ===== Current Equipped / Quick Slots (GamePanel / EquipPanel 展示) =====
        #region 当前装备
        Item_SO LeftWeapon { get; }
        Item_SO RightWeapon { get; }
        Item_SO CurrentConsumable { get; }
        Item_SO CurrentSpell { get; }

        Item_SO CurrentHelmet { get; }
        Item_SO CurrentBody { get; }
        Item_SO CurrentLegs { get; }
        Item_SO CurrentHands { get; }
        #endregion

        // 获取指定槽位的已装备物品
        Item_SO GetEquipSlotItem(int slotIndex);
        // ===== Commands (EquipPanel -> ChangeEquipItem) =====
        // 更换装备（武器/防具/消耗品/法术）
        void ChangeEquipItem(int slotIndex, Item_SO item);
    }
}