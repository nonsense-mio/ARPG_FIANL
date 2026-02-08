using System;
using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

namespace HT
{
    public class PlayerSaveManager : MonoBehaviour
    {
        PlayerManager player;

        public void Init(PlayerManager playerMgr)
        {
            player = playerMgr;
        }


        #region 数据存取相关
        //保存玩家数据到 Model 层
        public void SyncRuntimeToModel()
        {
            var playerModel = GameArchitecture.Interface.GetModel<IPlayerModel>();
            var stats = player.playerStatsManager;

            playerModel.PlayerName.Value = stats.characterName;
            playerModel.PlayerLevel.Value = stats.playerLevel;
            playerModel.HealthLevel.Value = stats.healthLevel;
            playerModel.StaminaLevel.Value = stats.staminaLevel;
            playerModel.FocusLevel.Value = stats.focusLevel;
            playerModel.PoiseLevel.Value = stats.poiseLevel;
            playerModel.StrengthLevel.Value = stats.strengthLevel;
            playerModel.DexterityLevel.Value = stats.dexterityLevel;
            playerModel.IntelligenceLevel.Value = stats.intelligenceLevel;
            playerModel.FaithLevel.Value = stats.faithLevel;
            playerModel.CurrentSoulCount.Value = stats.currentSoulCount;
            playerModel.PosX.Value = transform.position.x;
            playerModel.PosY.Value = transform.position.y;
            playerModel.PosZ.Value = transform.position.z;

            // 保存库存数据到 Model
            SyncInventoryToModel();
        }

        //从 Model 层加载玩家数据
        public void SyncFromModel()
        {
            var playerModel = GameArchitecture.Interface.GetModel<IPlayerModel>();
            var stats = player.playerStatsManager;

            stats.characterName = playerModel.PlayerName.Value;
            stats.playerLevel = playerModel.PlayerLevel.Value;
            stats.healthLevel = playerModel.HealthLevel.Value;
            stats.staminaLevel = playerModel.StaminaLevel.Value;
            stats.focusLevel = playerModel.FocusLevel.Value;
            stats.poiseLevel = playerModel.PoiseLevel.Value;
            stats.strengthLevel = playerModel.StrengthLevel.Value;
            stats.dexterityLevel = playerModel.DexterityLevel.Value;
            stats.intelligenceLevel = playerModel.IntelligenceLevel.Value;
            stats.faithLevel = playerModel.FaithLevel.Value;
            stats.currentSoulCount = playerModel.CurrentSoulCount.Value;

            // 根据加载的等级重新计算 max 值
            stats.SetMaxHealthFromHealthLevel();
            stats.SetMaxStaminaFromStaminaLevel();
            stats.SetMaxFocusPointsFromFocusLevel();
            // 满状态恢复
            player.playerStatsManager.FullPlayerStats();

            Vector3 loadPos = new Vector3(
                playerModel.RespawnX.Value,
                playerModel.RespawnY.Value,
                playerModel.RespawnZ.Value);
            player.SetPlayerPosition(loadPos);

            // 从 Model 加载库存数据
            SyncInventoryFromModel();
            // 加载武器和装备模型
            player.playerWeaponSlotManager.LoadBothWeaponsOnSlots();
            player.playerEquipmentManager.EquipAllEquipmentModels();
        }


        #endregion


        #region 库存数据存取方法

        /// <summary>
        /// 将当前库存数据保存到 IInventoryModel
        /// </summary>
        private void SyncInventoryToModel()
        {
            var model = GameArchitecture.Interface.GetModel<IInventoryModel>();
            var inv = player.playerInventoryManager;

            // 库存列表
            model.WeaponIDs.Reset(inv.weaponInventory.ConvertAll(w => w.itemID));
            model.HelmetIDs.Reset(inv.headEqiupmentInventory.ConvertAll(h => h.itemID));
            model.BodyIDs.Reset(inv.bodyEqiupmentInventory.ConvertAll(b => b.itemID));
            model.LegIDs.Reset(inv.legEqiupmentInventory.ConvertAll(l => l.itemID));
            model.HandIDs.Reset(inv.handEqiupmentInventory.ConvertAll(h => h.itemID));
            model.ConsumableIDs.Reset(inv.consumableInventory.ConvertAll(c => c.itemID));
            model.SpellIDs.Reset(inv.spellInventory.ConvertAll(s => s.itemID));

            // 快速插槽物品ID
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

            // 当前装备
            model.CurrentHelmetID.Value = (inv.currentHelmet != null && inv.currentHelmet.itemIcon != null)
                ? inv.currentHelmet.itemID : IInventoryModel.EMPTY_HELMET_ID;
            model.CurrentBodyID.Value = (inv.currentBody != null && inv.currentBody.itemIcon != null)
                ? inv.currentBody.itemID : IInventoryModel.EMPTY_BODY_ID;
            model.CurrentLegsID.Value = (inv.currentLegs != null && inv.currentLegs.itemIcon != null)
                ? inv.currentLegs.itemID : IInventoryModel.EMPTY_LEGS_ID;
            model.CurrentHandsID.Value = (inv.currentHands != null && inv.currentHands.itemIcon != null)
                ? inv.currentHands.itemID : IInventoryModel.EMPTY_HANDS_ID;

            // 当前索引 (保存时 -1 偏移，与原 SaveInventoryToData 一致)
            model.CurrentRightWeaponIndex.Value = inv.currentRightWeaponIndex - 1;
            model.CurrentLeftWeaponIndex.Value = inv.currentLeftWeaponIndex - 1;
            model.CurrentConsumableIndex.Value = inv.currentConsumableIndex - 1;
            model.CurrentSpellIndex.Value = inv.currentSpellIndex - 1;
        }

        /// <summary>
        /// 从 IInventoryModel 加载库存数据到运行时
        /// </summary>
        private void SyncInventoryFromModel()
        {
            var model = GameArchitecture.Interface.GetModel<IInventoryModel>();
            var inv = player.playerInventoryManager;
            ItemDataBase itemDB = ItemDataBase.Instance;

            // 清空当前库存
            inv.weaponInventory.Clear();
            inv.headEqiupmentInventory.Clear();
            inv.bodyEqiupmentInventory.Clear();
            inv.legEqiupmentInventory.Clear();
            inv.handEqiupmentInventory.Clear();
            inv.consumableInventory.Clear();
            inv.spellInventory.Clear();

            // 加载库存列表
            foreach (int id in model.WeaponIDs)
            {
                WeaponItem_SO weapon = itemDB.GetWeaponByID(id);
                if (weapon != null) inv.weaponInventory.Add(weapon);
            }
            foreach (int id in model.HelmetIDs)
            {
                HelmetEquipment helmet = itemDB.GetHelmetByID(id);
                if (helmet != null) inv.headEqiupmentInventory.Add(helmet);
            }
            foreach (int id in model.BodyIDs)
            {
                BodyEquipment body = itemDB.GetBodyByID(id);
                if (body != null) inv.bodyEqiupmentInventory.Add(body);
            }
            foreach (int id in model.LegIDs)
            {
                LegEquipment leg = itemDB.GetLegByID(id);
                if (leg != null) inv.legEqiupmentInventory.Add(leg);
            }
            foreach (int id in model.HandIDs)
            {
                HandEquipment hand = itemDB.GetHandByID(id);
                if (hand != null) inv.handEqiupmentInventory.Add(hand);
            }
            foreach (int id in model.ConsumableIDs)
            {
                ConsumableItem_SO consumable = itemDB.GetConsumableByID(id);
                if (consumable != null) inv.consumableInventory.Add(consumable);
            }
            foreach (int id in model.SpellIDs)
            {
                SpellItem spell = itemDB.GetSpellByID(id);
                if (spell != null) inv.spellInventory.Add(spell);
            }

            // 加载快速插槽物品
            for (int i = 0; i < inv.weaponInRightHandSlots.Length && i < model.RightHandSlotIDs.Count; i++)
            {
                inv.weaponInRightHandSlots[i] = itemDB.GetWeaponByID(model.RightHandSlotIDs[i]);
            }
            for (int i = 0; i < inv.weaponInLeftHandSlots.Length && i < model.LeftHandSlotIDs.Count; i++)
            {
                inv.weaponInLeftHandSlots[i] = itemDB.GetWeaponByID(model.LeftHandSlotIDs[i]);
            }
            for (int i = 0; i < inv.consumableSlots.Length && i < model.ConsumableSlotIDs.Count; i++)
            {
                inv.consumableSlots[i] = itemDB.GetConsumableByID(model.ConsumableSlotIDs[i]);
            }
            for (int i = 0; i < inv.spellSlots.Length && i < model.SpellSlotIDs.Count; i++)
            {
                inv.spellSlots[i] = itemDB.GetSpellByID(model.SpellSlotIDs[i]);
            }

            // 加载当前装备
            inv.currentHelmet = itemDB.GetHelmetByID(model.CurrentHelmetID.Value);
            inv.currentBody = itemDB.GetBodyByID(model.CurrentBodyID.Value);
            inv.currentLegs = itemDB.GetLegByID(model.CurrentLegsID.Value);
            inv.currentHands = itemDB.GetHandByID(model.CurrentHandsID.Value);

            // 加载当前索引
            inv.currentRightWeaponIndex = model.CurrentRightWeaponIndex.Value;
            inv.currentLeftWeaponIndex = model.CurrentLeftWeaponIndex.Value;
            inv.currentConsumableIndex = model.CurrentConsumableIndex.Value;
            inv.currentSpellIndex = model.CurrentSpellIndex.Value;

            //更新武器和物品状态
            inv.ChangeLeftWeapon();
            inv.ChangeRightWeapon();
            inv.ChangeConsumable();
            inv.ChangeSpell();

            // Change* 方法会推进索引（因为保存时做了 -1 偏移），同步回 Model 层
            model.CurrentRightWeaponIndex.Value = inv.currentRightWeaponIndex;
            model.CurrentLeftWeaponIndex.Value = inv.currentLeftWeaponIndex;
            model.CurrentConsumableIndex.Value = inv.currentConsumableIndex;
            model.CurrentSpellIndex.Value = inv.currentSpellIndex;
        }

        #endregion


    }
}
