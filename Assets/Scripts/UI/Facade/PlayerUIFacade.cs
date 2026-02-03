using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    /// <summary>
    /// UI 外观实现：内部仍然使用 PlayerStatsManager / PlayerInventoryManager，
    /// 但 UI 不直接接触它们。
    /// </summary>
    public sealed class PlayerUIFacade : IPlayerUIFacade
    {
        private readonly PlayerStatsManager stats;
        private readonly PlayerInventoryManager inventory;

        public PlayerUIFacade(PlayerStatsManager stats, PlayerInventoryManager inventory)
        {
            this.stats = stats;
            this.inventory = inventory;
        }

        // ===== Stats =====
        public int MaxHealth => stats.maxHealth;
        public int CurrentHealth => stats.currentHealth;
        public float MaxStamina => stats.maxStamina;
        public float CurrentStamina => stats.currentStamina;
        public float MaxFocus => stats.maxFocus;
        public float CurrentFocus => stats.currentFocus;
        //魂数
        public int CurrentSoulCount { get => stats.currentSoulCount; set => stats.currentSoulCount = value; }

        //玩家等级及各属性等级
        public int PlayerLevel { get => stats.playerLevel; set => stats.playerLevel = value; }
        public int HealthLevel { get => stats.healthLevel; set => stats.healthLevel = value; }
        public int StaminaLevel { get => stats.staminaLevel; set => stats.staminaLevel = value; }
        public int FocusLevel { get => stats.focusLevel; set => stats.focusLevel = value; }
        public int PoiseLevel { get => stats.poiseLevel; set => stats.poiseLevel = value; }
        public int StrengthLevel { get => stats.strengthLevel; set => stats.strengthLevel = value; }
        public int DexterityLevel { get => stats.dexterityLevel; set => stats.dexterityLevel = value; }
        public int IntelligenceLevel { get => stats.intelligenceLevel; set => stats.intelligenceLevel = value; }
        public int FaithLevel { get => stats.faithLevel; set => stats.faithLevel = value; }

        public int SetMaxHealthFromHealthLevel() => stats.SetMaxHealthFromHealthLevel();
        public float SetMaxStaminaFromStaminaLevel() => stats.SetMaxStaminaFromStaminaLevel();
        public float SetMaxFocusPointsFromFocusLevel() => stats.SetMaxFocusPointsFromFocusLevel();

        // ===== Inventory lists =====
        public IReadOnlyList<Item_SO> GetWeaponInventory() => inventory.weaponInventory;
        public IReadOnlyList<Item_SO> GetHelmetInventory() => inventory.headEqiupmentInventory;
        public IReadOnlyList<Item_SO> GetBodyInventory() => inventory.bodyEqiupmentInventory;
        public IReadOnlyList<Item_SO> GetLegInventory() => inventory.legEqiupmentInventory;
        public IReadOnlyList<Item_SO> GetHandInventory() => inventory.handEqiupmentInventory;
        public IReadOnlyList<Item_SO> GetConsumableInventory() => inventory.consumableInventory;
        public IReadOnlyList<Item_SO> GetSpellInventory() => inventory.spellInventory;

        public List<Item_SO> GetAllInventorySnapshot()
        {
            var all = new List<Item_SO>(128);
            all.AddRange(inventory.weaponInventory);
            all.AddRange(inventory.headEqiupmentInventory);
            all.AddRange(inventory.bodyEqiupmentInventory);
            all.AddRange(inventory.legEqiupmentInventory);
            all.AddRange(inventory.handEqiupmentInventory);
            all.AddRange(inventory.consumableInventory);
            all.AddRange(inventory.spellInventory);
            return all;
        }

        // ===== Equipped / quick slots =====
        public Item_SO LeftWeapon => inventory.leftWeapon;
        public Item_SO RightWeapon => inventory.rightWeapon;
        public Item_SO CurrentConsumable => inventory.currentConsumable;
        public Item_SO CurrentSpell => inventory.currentSpell;

        public Item_SO CurrentHelmet => inventory.currentHelmet;
        public Item_SO CurrentBody => inventory.currentBody;
        public Item_SO CurrentLegs => inventory.currentLegs;
        public Item_SO CurrentHands => inventory.currentHands;

        // 获取指定槽位的已装备物品
        public Item_SO GetEquipSlotItem(int slotIndex)
        {
            // 0-3 右手武器，4-7 左手武器，8-11 头/身/腿/手，12-15 消耗品，16+ 法术
            if (slotIndex >= 0 && slotIndex < 4) return inventory.weaponInRightHandSlots[slotIndex];
            if (slotIndex >= 4 && slotIndex < 8) return inventory.weaponInLeftHandSlots[slotIndex - 4];

            if (slotIndex == 8) return inventory.currentHelmet;
            if (slotIndex == 9) return inventory.currentBody;
            if (slotIndex == 10) return inventory.currentLegs;
            if (slotIndex == 11) return inventory.currentHands;

            if (slotIndex >= 12 && slotIndex < 16) return inventory.consumableSlots[slotIndex - 12];
            if (slotIndex >= 16) return inventory.spellSlots[slotIndex - 16];

            return null;
        }

        public void ChangeEquipItem(int slotIndex, Item_SO item) => inventory.ChangeEquipItem(slotIndex, item);
    }
}