using System;
using System.Collections.Generic;
using Framework;
using ARPG;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 更换装备命令 - 双路写入:
    /// 1. PlayerInventoryManager (运行时逻辑 + 视觉更新)
    /// 2. IInventoryModel (ID列表，驱动UI响应式更新)
    /// </summary>
    public class ChangeEquipItemCommand : AbstractCommand
    {
        private int slotIndex;
        private Item_SO newItem;

        public ChangeEquipItemCommand() { }

        public ChangeEquipItemCommand(int slotIndex, Item_SO newItem)
        {
            this.slotIndex = slotIndex;
            this.newItem = newItem;
        }

        protected override void OnExecute()
        {
            var player = PlayerManager.localPlayer;
            if (player == null) return;

            var inventory = player.playerInventoryManager;

            // 1) 运行时逻辑: 库存交换 + 视觉/武器切换
            inventory.ChangeEquipItem(slotIndex, newItem);

            // 2) 同步 IInventoryModel (从运行时读回最终状态)
            var model = this.GetModel<IInventoryModel>();
            SyncModelFromRuntime(model, inventory);
        }

        /// <summary>
        /// 从运行时 PlayerInventoryManager 读回所有状态，同步到 Model
        /// </summary>
        private void SyncModelFromRuntime(IInventoryModel model, PlayerInventoryManager inv)
        {
            // --- 库存列表 ---
            model.WeaponIDs.Reset(inv.weaponInventory.ConvertAll(w => w.itemID));
            model.HelmetIDs.Reset(inv.headEqiupmentInventory.ConvertAll(h => h.itemID));
            model.BodyIDs.Reset(inv.bodyEqiupmentInventory.ConvertAll(b => b.itemID));
            model.LegIDs.Reset(inv.legEqiupmentInventory.ConvertAll(l => l.itemID));
            model.HandIDs.Reset(inv.handEqiupmentInventory.ConvertAll(h => h.itemID));
            model.ConsumableIDs.Reset(inv.consumableInventory.ConvertAll(c => c.itemID));
            model.SpellIDs.Reset(inv.spellInventory.ConvertAll(s => s.itemID));

            // --- 快速槽位 ---
            int slotSize = 4;
            for (int i = 0; i < Math.Min(slotSize, inv.weaponInRightHandSlots.Length); i++)
            {
                var so = inv.weaponInRightHandSlots[i];
                model.SetSlotItem(0, i, (so != null && so.itemIcon != null)
                    ? so.itemID : IInventoryModel.EMPTY_WEAPON_ID);
            }
            for (int i = 0; i < Math.Min(slotSize, inv.weaponInLeftHandSlots.Length); i++)
            {
                var so = inv.weaponInLeftHandSlots[i];
                model.SetSlotItem(1, i, (so != null && so.itemIcon != null)
                    ? so.itemID : IInventoryModel.EMPTY_WEAPON_ID);
            }
            for (int i = 0; i < Math.Min(slotSize, inv.consumableSlots.Length); i++)
            {
                var so = inv.consumableSlots[i];
                model.SetSlotItem(2, i, (so != null && so.itemIcon != null)
                    ? so.itemID : IInventoryModel.EMPTY_CONSUMABLE_ID);
            }
            for (int i = 0; i < Math.Min(slotSize, inv.spellSlots.Length); i++)
            {
                var so = inv.spellSlots[i];
                model.SetSlotItem(3, i, (so != null && so.itemIcon != null)
                    ? so.itemID : IInventoryModel.EMPTY_SPELL_ID);
            }

            // --- 当前防具 ---
            model.CurrentHelmetID.Value = (inv.currentHelmet != null && inv.currentHelmet.itemIcon != null)
                ? inv.currentHelmet.itemID : IInventoryModel.EMPTY_HELMET_ID;
            model.CurrentBodyID.Value = (inv.currentBody != null && inv.currentBody.itemIcon != null)
                ? inv.currentBody.itemID : IInventoryModel.EMPTY_BODY_ID;
            model.CurrentLegsID.Value = (inv.currentLegs != null && inv.currentLegs.itemIcon != null)
                ? inv.currentLegs.itemID : IInventoryModel.EMPTY_LEGS_ID;
            model.CurrentHandsID.Value = (inv.currentHands != null && inv.currentHands.itemIcon != null)
                ? inv.currentHands.itemID : IInventoryModel.EMPTY_HANDS_ID;

            // --- 当前索引 ---
            model.CurrentRightWeaponIndex.Value = inv.currentRightWeaponIndex;
            model.CurrentLeftWeaponIndex.Value = inv.currentLeftWeaponIndex;
            model.CurrentConsumableIndex.Value = inv.currentConsumableIndex;
            model.CurrentSpellIndex.Value = inv.currentSpellIndex;
        }
    }
}
