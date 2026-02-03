using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
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

        //在这里我们规定换下来的物品的itemIcon为null时 表示空槽位！！！
        public void ChangeEquipItem(int slotIndex, Item_SO item)
        {
            //从背包移除要装备的物品            
            //添加换下来的物品到背包
            if (item is WeaponItem_SO)
            {
                weaponInventory.Remove(item as WeaponItem_SO);
                if (slotIndex < 4)
                {
                    if (weaponInRightHandSlots[slotIndex].itemIcon != null)
                        weaponInventory.Add(weaponInRightHandSlots[slotIndex]);
                    weaponInRightHandSlots[slotIndex] = item as WeaponItem_SO;
                    currentRightWeaponIndex--;
                    ChangeRightWeapon();

                }
                else
                {
                    if (weaponInLeftHandSlots[slotIndex - 4].itemIcon != null)
                        weaponInventory.Add(weaponInLeftHandSlots[slotIndex - 4]);
                    weaponInLeftHandSlots[slotIndex - 4] = item as WeaponItem_SO;
                    currentLeftWeaponIndex--;
                    ChangeLeftWeapon();
                }
                //装备武器
                player.playerWeaponSlotManager.LoadBothWeaponsOnSlots();
            }
            else if (item is HelmetEquipment)
            {
                headEqiupmentInventory.Remove(item as HelmetEquipment);
                if (currentHelmet.itemIcon != null)
                    headEqiupmentInventory.Add(currentHelmet);
                currentHelmet = item as HelmetEquipment;
                player.playerEquipmentManager.EquipAllEquipmentModels();
            }
            else if (item is BodyEquipment)
            {
                bodyEqiupmentInventory.Remove(item as BodyEquipment);
                if (currentBody.itemIcon != null)
                    bodyEqiupmentInventory.Add(currentBody);
                currentBody = item as BodyEquipment;
                player.playerEquipmentManager.EquipAllEquipmentModels();
            }
            else if (item is LegEquipment)
            {
                legEqiupmentInventory.Remove(item as LegEquipment);
                if (currentLegs.itemIcon != null)
                    legEqiupmentInventory.Add(currentLegs);
                currentLegs = item as LegEquipment;
                player.playerEquipmentManager.EquipAllEquipmentModels();
            }
            else if (item is HandEquipment)
            {
                handEqiupmentInventory.Remove(item as HandEquipment);
                if (currentHands.itemIcon != null)
                    handEqiupmentInventory.Add(currentHands);
                currentHands = item as HandEquipment;
                player.playerEquipmentManager.EquipAllEquipmentModels();
            }
            else if (item is ConsumableItem_SO)
            {
                if (slotIndex >= 12 && slotIndex < 16)
                {
                    consumableInventory.Remove(item as ConsumableItem_SO);
                    if (consumableSlots[slotIndex - 12].itemIcon != null)
                        consumableInventory.Add(consumableSlots[slotIndex - 12]);
                    consumableSlots[slotIndex - 12] = item as ConsumableItem_SO;
                    currentConsumableIndex--;
                    ChangeConsumable();
                }

            }
            else if (item is SpellItem)
            {
                if (slotIndex >= 16)
                {
                    spellInventory.Remove(item as SpellItem);
                    if (spellSlots[slotIndex - 16].itemIcon != null)
                        spellInventory.Add(spellSlots[slotIndex - 16]);
                    spellSlots[slotIndex - 16] = item as SpellItem;
                    currentSpellIndex--;
                    ChangeSpell();
                }
            }

        }


        /// <summary>
        /// 切换右手武器的方法
        /// </summary>
        public void ChangeRightWeapon()
        {
            int checkIndex = currentRightWeaponIndex + 1;
            while (true)
            {
                //  如果索引超出了数组长度,索引归零
                if (checkIndex >= weaponInRightHandSlots.Length)
                {
                    currentRightWeaponIndex = 0;
                    rightWeapon = weaponInRightHandSlots[0];
                    return;
                }

                //  如果当前槽位有物品，加载它
                if (weaponInRightHandSlots[checkIndex] != null)
                {
                    currentRightWeaponIndex = checkIndex;
                    rightWeapon = weaponInRightHandSlots[checkIndex];
                    return;
                }
                checkIndex++;
            }


        }

        /// <summary>
        /// 切换左手武器的方法
        /// </summary>
        public void ChangeLeftWeapon()
        {
            int checkIndex = currentLeftWeaponIndex + 1;
            while (true)
            {
                // 如果索引超出了数组长度,索引归零
                if (checkIndex >= weaponInLeftHandSlots.Length)
                {
                    currentLeftWeaponIndex = 0;
                    leftWeapon = weaponInLeftHandSlots[0];
                    return;
                }

                // 如果当前槽位有物品，加载它
                if (weaponInLeftHandSlots[checkIndex] != null)
                {
                    currentLeftWeaponIndex = checkIndex;
                    leftWeapon = weaponInLeftHandSlots[checkIndex];
                    return;
                }
                checkIndex++;
            }
        }

        /// <summary>
        /// 通用的切换武器逻辑
        /// </summary>
        /// <param name="currentIndex">当前的武器索引（引用传递，因为要修改它）</param>
        /// <param name="weaponSlots">对应的武器槽数组</param>
        /// <param name="isLeft">是否是左手</param>

        //切换消耗品
        public void ChangeConsumable()
        {
            int checkIndex = currentConsumableIndex + 1;
            while (true)
            {
                // 如果索引超出了数组长度,索引归零
                if (checkIndex >= consumableSlots.Length)
                {
                    currentConsumableIndex = 0;
                    currentConsumable = consumableSlots[0];
                    return;
                }

                // 如果当前槽位有物品，加载它
                if (consumableSlots[checkIndex] != null)
                {
                    currentConsumableIndex = checkIndex;
                    currentConsumable = consumableSlots[checkIndex];
                    return;
                }
                checkIndex++;
            }
        }
        //切换法术
        public void ChangeSpell()
        {
            int checkIndex = currentSpellIndex + 1;
            while (true)
            {
                // 如果索引超出了数组长度,索引归零
                if (checkIndex >= spellSlots.Length)
                {
                    currentSpellIndex = 0;
                    currentSpell = spellSlots[0];
                    return;
                }

                // 如果当前槽位有物品，加载它
                if (spellSlots[checkIndex] != null)
                {
                    currentSpellIndex = checkIndex;
                    currentSpell = spellSlots[checkIndex];
                    return;
                }
                checkIndex++;
            }
        }

        #region 任务奖励方法
        public void RewardTask(TaskData_SO taskData)
        {
            foreach (var reward in taskData.rewardList)
            {
                if (reward is WeaponItem_SO)
                {
                    weaponInventory.Add(reward as WeaponItem_SO);
                }
                else if (reward is HelmetEquipment)
                {
                    headEqiupmentInventory.Add(reward as HelmetEquipment);
                }
                else if (reward is BodyEquipment)
                {
                    bodyEqiupmentInventory.Add(reward as BodyEquipment);
                }
                else if (reward is LegEquipment)
                {
                    legEqiupmentInventory.Add(reward as LegEquipment);
                }
                else if (reward is HandEquipment)
                {
                    handEqiupmentInventory.Add(reward as HandEquipment);
                }
                else if (reward is ConsumableItem_SO)
                {
                    consumableInventory.Add(reward as ConsumableItem_SO);
                }
                else if (reward is SpellItem)
                {
                    spellInventory.Add(reward as SpellItem);
                }
            }
        }

        #endregion
        public void AddItemToInventory(Item_SO item)
        {
            if (item is WeaponItem_SO)
            {
                weaponInventory.Add(item as WeaponItem_SO);
            }
            else if (item is HelmetEquipment)
            {
                headEqiupmentInventory.Add(item as HelmetEquipment);
            }
            else if (item is BodyEquipment)
            {
                bodyEqiupmentInventory.Add(item as BodyEquipment);
            }
            else if (item is LegEquipment)
            {
                legEqiupmentInventory.Add(item as LegEquipment);
            }
            else if (item is HandEquipment)
            {
                handEqiupmentInventory.Add(item as HandEquipment);
            }
            else if (item is ConsumableItem_SO)
            {
                consumableInventory.Add(item as ConsumableItem_SO);
            }
            else if (item is SpellItem)
            {
                spellInventory.Add(item as SpellItem);
            }
        }

    }
}

