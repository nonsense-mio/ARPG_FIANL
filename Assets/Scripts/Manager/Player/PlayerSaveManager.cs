using System.Collections;
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
        //保存玩家数据
        public void SaveDataToGameDataMgr()
        {
            PlayerData data = CurrentGameDataMgr.Instance.playerData;
            data.playerName = player.playerStatsManager.characterName;
            data.playerLevel = player.playerStatsManager.playerLevel;
            data.healthLevel = player.playerStatsManager.healthLevel;
            data.staminaLevel = player.playerStatsManager.staminaLevel;
            data.focusLevel = player.playerStatsManager.focusLevel;
            data.poiseLevel = player.playerStatsManager.poiseLevel;
            data.strengthLevel = player.playerStatsManager.strengthLevel;
            data.dexterityLevel = player.playerStatsManager.dexterityLevel;
            data.intelligenceLevel = player.playerStatsManager.intelligenceLevel;
            data.faithLevel = player.playerStatsManager.faithLevel;
            data.currentSoulCount = player.playerStatsManager.currentSoulCount;
            data.xPos = transform.position.x;
            data.yPos = transform.position.y;
            data.zPos = transform.position.z;

            // 保存库存数据
            SaveInventoryToData();
        }

        //加载玩家数据
        public void LoadDataFromGameDataMgr()
        {
            var data = CurrentGameDataMgr.Instance.playerData;
            var stats = player.playerStatsManager;

            stats.characterName = data.playerName;
            stats.playerLevel = data.playerLevel;
            stats.healthLevel = data.healthLevel;
            stats.staminaLevel = data.staminaLevel;
            stats.focusLevel = data.focusLevel;
            stats.poiseLevel = data.poiseLevel;
            stats.strengthLevel = data.strengthLevel;
            stats.dexterityLevel = data.dexterityLevel;
            stats.intelligenceLevel = data.intelligenceLevel;
            stats.faithLevel = data.faithLevel;
            stats.currentSoulCount = data.currentSoulCount;
            Vector3 loadPos = new Vector3(data.respawnX, data.respawnY, data.respawnZ);
            player.SetPlayerPosition(loadPos);
            // 加载库存数据
            LoadInventoryFromData();
            // 加载武器和装备模型
            player.playerWeaponSlotManager.LoadBothWeaponsOnSlots();
            player.playerEquipmentManager.EquipAllEquipmentModels();
        }


        #endregion


        #region 库存数据存取方法

        /// <summary>
        /// 将当前库存数据保存到GameDataMgr中的PlayerInventoryData
        /// </summary>
        public void SaveInventoryToData()
        {
            var data = CurrentGameDataMgr.Instance.playerInventoryData;
            var inv = player.playerInventoryManager;

            // 清空旧数据
            data.weaponInventoryIDs.Clear();
            data.headEquipmentInventoryIDs.Clear();
            data.bodyEquipmentInventoryIDs.Clear();
            data.legEquipmentInventoryIDs.Clear();
            data.handEquipmentInventoryIDs.Clear();
            data.consumableInventoryIDs.Clear();
            data.spellInventoryIDs.Clear();


            // 保存库存列表
            foreach (var weapon in inv.weaponInventory)
            {
                if (weapon != null && weapon.itemIcon != null)
                    data.weaponInventoryIDs.Add(weapon.itemID);
            }
            foreach (var helmet in inv.headEqiupmentInventory)
            {
                if (helmet != null && helmet.itemIcon != null)
                    data.headEquipmentInventoryIDs.Add(helmet.itemID);
            }
            foreach (var body in inv.bodyEqiupmentInventory)
            {
                if (body != null && body.itemIcon != null)
                    data.bodyEquipmentInventoryIDs.Add(body.itemID);
            }
            foreach (var leg in inv.legEqiupmentInventory)
            {
                if (leg != null && leg.itemIcon != null)
                    data.legEquipmentInventoryIDs.Add(leg.itemID);
            }
            foreach (var hand in inv.handEqiupmentInventory)
            {
                if (hand != null && hand.itemIcon != null)
                    data.handEquipmentInventoryIDs.Add(hand.itemID);
            }
            foreach (var consumable in inv.consumableInventory)
            {
                if (consumable != null && consumable.itemIcon != null)
                    data.consumableInventoryIDs.Add(consumable.itemID);
            }
            foreach (var spell in inv.spellInventory)
            {
                if (spell != null && spell.itemIcon != null)
                    data.spellInventoryIDs.Add(spell.itemID);
            }

            // 保存快速插槽物品ID
            for (int i = 0; i < data.rightHandSlotIDs.Count; i++)
            {
                data.rightHandSlotIDs[i] = inv.weaponInRightHandSlots[i].itemID;
            }
            for (int i = 0; i < data.leftHandSlotIDs.Count; i++)
            {
                data.leftHandSlotIDs[i] = inv.weaponInLeftHandSlots[i].itemID;
            }
            for (int i = 0; i < data.consumableSlotIDs.Count; i++)
            {
                data.consumableSlotIDs[i] = inv.consumableSlots[i].itemID;
            }
            for (int i = 0; i < data.spellSlotIDs.Count; i++)
            {
                data.spellSlotIDs[i] = inv.spellSlots[i].itemID;
            }

            // 保存当前装备
            data.currentHelmetID = inv.currentHelmet.itemID;
            data.currentBodyID = inv.currentBody.itemID;
            data.currentLegsID = inv.currentLegs.itemID;
            data.currentHandsID = inv.currentHands.itemID;

            // 保存当前索引
            data.currentRightWeaponIndex = inv.currentRightWeaponIndex - 1;
            data.currentLeftWeaponIndex = inv.currentLeftWeaponIndex - 1;
            data.currentConsumableIndex = inv.currentConsumableIndex - 1;
            data.currentSpellIndex = inv.currentSpellIndex - 1;
        }

        /// <summary>
        /// 从GameDataMgr中的PlayerInventoryData加载库存数据
        /// </summary>
        public void LoadInventoryFromData()
        {
            var data = CurrentGameDataMgr.Instance.playerInventoryData;
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
            foreach (int id in data.weaponInventoryIDs)
            {
                WeaponItem_SO weapon = itemDB.GetWeaponByID(id);
                if (weapon != null) inv.weaponInventory.Add(weapon);
            }
            foreach (int id in data.headEquipmentInventoryIDs)
            {
                HelmetEquipment helmet = itemDB.GetHelmetByID(id);
                if (helmet != null) inv.headEqiupmentInventory.Add(helmet);
            }
            foreach (int id in data.bodyEquipmentInventoryIDs)
            {
                BodyEquipment body = itemDB.GetBodyByID(id);
                if (body != null) inv.bodyEqiupmentInventory.Add(body);
            }
            foreach (int id in data.legEquipmentInventoryIDs)
            {
                LegEquipment leg = itemDB.GetLegByID(id);
                if (leg != null) inv.legEqiupmentInventory.Add(leg);
            }
            foreach (int id in data.handEquipmentInventoryIDs)
            {
                HandEquipment hand = itemDB.GetHandByID(id);
                if (hand != null) inv.handEqiupmentInventory.Add(hand);
            }
            foreach (int id in data.consumableInventoryIDs)
            {
                ConsumableItem_SO consumable = itemDB.GetConsumableByID(id);
                if (consumable != null) inv.consumableInventory.Add(consumable);
            }
            foreach (int id in data.spellInventoryIDs)
            {
                SpellItem spell = itemDB.GetSpellByID(id);
                if (spell != null) inv.spellInventory.Add(spell);
            }

            // 加载快速插槽物品
            for (int i = 0; i < inv.weaponInRightHandSlots.Length && i < data.rightHandSlotIDs.Count; i++)
            {
                inv.weaponInRightHandSlots[i] = itemDB.GetWeaponByID(data.rightHandSlotIDs[i]);
            }
            for (int i = 0; i < inv.weaponInLeftHandSlots.Length && i < data.leftHandSlotIDs.Count; i++)
            {
                inv.weaponInLeftHandSlots[i] = itemDB.GetWeaponByID(data.leftHandSlotIDs[i]);
            }
            for (int i = 0; i < inv.consumableSlots.Length && i < data.consumableSlotIDs.Count; i++)
            {
                inv.consumableSlots[i] = itemDB.GetConsumableByID(data.consumableSlotIDs[i]);
            }
            for (int i = 0; i < inv.spellSlots.Length && i < data.spellSlotIDs.Count; i++)
            {
                inv.spellSlots[i] = itemDB.GetSpellByID(data.spellSlotIDs[i]);
            }

            // 加载当前装备
            inv.currentHelmet = itemDB.GetHelmetByID(data.currentHelmetID);
            inv.currentBody = itemDB.GetBodyByID(data.currentBodyID);
            inv.currentLegs = itemDB.GetLegByID(data.currentLegsID);
            inv.currentHands = itemDB.GetHandByID(data.currentHandsID);

            // 加载当前索引
            inv.currentRightWeaponIndex = data.currentRightWeaponIndex;
            inv.currentLeftWeaponIndex = data.currentLeftWeaponIndex;
            inv.currentConsumableIndex = data.currentConsumableIndex;
            inv.currentSpellIndex = data.currentSpellIndex;

            //更新武器和物品状态
            inv.ChangeLeftWeapon();
            inv.ChangeRightWeapon();
            inv.ChangeConsumable();
            inv.ChangeSpell();

            // Change* 方法会推进索引（因为保存时做了 -1 偏移），同步回 Model 层
            var inventoryModel = GameArchitecture.Interface.GetModel<IInventoryModel>();
            inventoryModel.CurrentRightWeaponIndex.Value = inv.currentRightWeaponIndex;
            inventoryModel.CurrentLeftWeaponIndex.Value = inv.currentLeftWeaponIndex;
            inventoryModel.CurrentConsumableIndex.Value = inv.currentConsumableIndex;
            inventoryModel.CurrentSpellIndex.Value = inv.currentSpellIndex;
        }

        #endregion


    }
}

