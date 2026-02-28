using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    //玩家库存类
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        PlayerManager player;
        //玩家武器库存列表
        public List<WeaponItem_SO> weaponInventory;
        public List<HelmetEquipment> headEqiupmentInventory;
        public List<BodyEquipment> bodyEqiupmentInventory;
        public List<LegEquipment> legEqiupmentInventory;
        public List<HandEquipment> handEqiupmentInventory;
        public List<ConsumableItem_SO> consumableInventory;
        public List<SpellItem> spellInventory;
        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            player = characterMgr as PlayerManager;
        }

        #region 槽位循环切换 (通用)

        /// <summary>
        /// 从 currentIndex+1 开始查找下一个非空槽位，超出数组长度时回绕到 0
        /// </summary>
        private T CycleSlot<T>(T[] slots, ref int currentIndex) where T : Item_SO
        {
            int checkIndex = currentIndex + 1;
            while (true)
            {
                if (checkIndex >= slots.Length)
                {
                    currentIndex = 0;
                    return slots[0];
                }
                if (slots[checkIndex] != null)
                {
                    currentIndex = checkIndex;
                    return slots[checkIndex];
                }
                checkIndex++;
            }
        }

        public void ChangeRightWeapon() => rightWeapon = CycleSlot(weaponInRightHandSlots, ref currentRightWeaponIndex);
        public void ChangeLeftWeapon() => leftWeapon = CycleSlot(weaponInLeftHandSlots, ref currentLeftWeaponIndex);
        public void ChangeConsumable() => currentConsumable = CycleSlot(consumableSlots, ref currentConsumableIndex);
        public void ChangeSpell() => currentSpell = CycleSlot(spellSlots, ref currentSpellIndex);

        #endregion

        #region 库存类型路由 (通用)

        /// <summary>
        /// 根据物品运行时类型返回对应的库存列表
        /// </summary>
        private IList GetInventoryForType(Item_SO item)
        {
            if (item is WeaponItem_SO) return weaponInventory;
            if (item is HelmetEquipment) return headEqiupmentInventory;
            if (item is BodyEquipment) return bodyEqiupmentInventory;
            if (item is LegEquipment) return legEqiupmentInventory;
            if (item is HandEquipment) return handEqiupmentInventory;
            if (item is ConsumableItem_SO) return consumableInventory;
            if (item is SpellItem) return spellInventory;
            return null;
        }

        public void AddItemToInventory(Item_SO item) => GetInventoryForType(item)?.Add(item);

        public void RewardTask(TaskData_SO taskData)
        {
            foreach (var reward in taskData.rewardList)
                GetInventoryForType(reward)?.Add(reward);
        }

        #endregion

        #region 装备切换

        //在这里我们规定换下来的物品的itemIcon为null时 表示空槽位！！！
        public void ChangeEquipItem(int slotIndex, Item_SO item)
        {
            if (item is WeaponItem_SO weapon)
            {
                weaponInventory.Remove(weapon);
                if (slotIndex < 4)
                {
                    if (weaponInRightHandSlots[slotIndex].itemIcon != null)
                        weaponInventory.Add(weaponInRightHandSlots[slotIndex]);
                    weaponInRightHandSlots[slotIndex] = weapon;
                    currentRightWeaponIndex--;
                    ChangeRightWeapon();
                }
                else
                {
                    if (weaponInLeftHandSlots[slotIndex - 4].itemIcon != null)
                        weaponInventory.Add(weaponInLeftHandSlots[slotIndex - 4]);
                    weaponInLeftHandSlots[slotIndex - 4] = weapon;
                    currentLeftWeaponIndex--;
                    ChangeLeftWeapon();
                }
                player.playerWeaponSlotManager.LoadBothWeaponsOnSlots();
            }
            else if (item is HelmetEquipment helmet)
            {
                headEqiupmentInventory.Remove(helmet);
                if (currentHelmet.itemIcon != null)
                    headEqiupmentInventory.Add(currentHelmet);
                currentHelmet = helmet;
                player.playerEquipmentManager.EquipAllEquipmentModels();
            }
            else if (item is BodyEquipment body)
            {
                bodyEqiupmentInventory.Remove(body);
                if (currentBody.itemIcon != null)
                    bodyEqiupmentInventory.Add(currentBody);
                currentBody = body;
                player.playerEquipmentManager.EquipAllEquipmentModels();
            }
            else if (item is LegEquipment leg)
            {
                legEqiupmentInventory.Remove(leg);
                if (currentLegs.itemIcon != null)
                    legEqiupmentInventory.Add(currentLegs);
                currentLegs = leg;
                player.playerEquipmentManager.EquipAllEquipmentModels();
            }
            else if (item is HandEquipment hand)
            {
                handEqiupmentInventory.Remove(hand);
                if (currentHands.itemIcon != null)
                    handEqiupmentInventory.Add(currentHands);
                currentHands = hand;
                player.playerEquipmentManager.EquipAllEquipmentModels();
            }
            else if (item is ConsumableItem_SO consumable && slotIndex >= 12 && slotIndex < 16)
            {
                consumableInventory.Remove(consumable);
                if (consumableSlots[slotIndex - 12].itemIcon != null)
                    consumableInventory.Add(consumableSlots[slotIndex - 12]);
                consumableSlots[slotIndex - 12] = consumable;
                currentConsumableIndex--;
                ChangeConsumable();
            }
            else if (item is SpellItem spell && slotIndex >= 16)
            {
                spellInventory.Remove(spell);
                if (spellSlots[slotIndex - 16].itemIcon != null)
                    spellInventory.Add(spellSlots[slotIndex - 16]);
                spellSlots[slotIndex - 16] = spell;
                currentSpellIndex--;
                ChangeSpell();
            }
        }

        #endregion
    }
}

