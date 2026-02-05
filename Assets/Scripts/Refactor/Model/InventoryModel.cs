using System.Collections.Generic;
using Framework;

namespace ARPG
{
    /// <summary>
    /// 背包数据模型实现 - 管理物品ID列表和快速插槽状态
    /// </summary>
    public class InventoryModel : AbstractModel, IInventoryModel
    {
        private const int SLOT_SIZE = 4;

        #region 库存ID列表
        public BindableList<int> WeaponIDs { get; } = new BindableList<int>();
        public BindableList<int> HelmetIDs { get; } = new BindableList<int>();
        public BindableList<int> BodyIDs { get; } = new BindableList<int>();
        public BindableList<int> LegIDs { get; } = new BindableList<int>();
        public BindableList<int> HandIDs { get; } = new BindableList<int>();
        public BindableList<int> ConsumableIDs { get; } = new BindableList<int>();
        public BindableList<int> SpellIDs { get; } = new BindableList<int>();
        #endregion

        #region 快速插槽ID
        public List<int> RightHandSlotIDs { get; private set; }
        public List<int> LeftHandSlotIDs { get; private set; }
        public List<int> ConsumableSlotIDs { get; private set; }
        public List<int> SpellSlotIDs { get; private set; }
        #endregion

        #region 当前装备ID
        public BindableProperty<int> CurrentHelmetID { get; } = new BindableProperty<int>(IInventoryModel.EMPTY_HELMET_ID);
        public BindableProperty<int> CurrentBodyID { get; } = new BindableProperty<int>(IInventoryModel.EMPTY_BODY_ID);
        public BindableProperty<int> CurrentLegsID { get; } = new BindableProperty<int>(IInventoryModel.EMPTY_LEGS_ID);
        public BindableProperty<int> CurrentHandsID { get; } = new BindableProperty<int>(IInventoryModel.EMPTY_HANDS_ID);
        #endregion

        #region 当前索引
        public BindableProperty<int> CurrentRightWeaponIndex { get; } = new BindableProperty<int>(-1);
        public BindableProperty<int> CurrentLeftWeaponIndex { get; } = new BindableProperty<int>(-1);
        public BindableProperty<int> CurrentConsumableIndex { get; } = new BindableProperty<int>(-1);
        public BindableProperty<int> CurrentSpellIndex { get; } = new BindableProperty<int>(-1);
        #endregion

        protected override void OnInit()
        {
            // 初始化快速插槽为空
            RightHandSlotIDs = CreateEmptySlots(IInventoryModel.EMPTY_WEAPON_ID);
            LeftHandSlotIDs = CreateEmptySlots(IInventoryModel.EMPTY_WEAPON_ID);
            ConsumableSlotIDs = CreateEmptySlots(IInventoryModel.EMPTY_CONSUMABLE_ID);
            SpellSlotIDs = CreateEmptySlots(IInventoryModel.EMPTY_SPELL_ID);
        }

        private List<int> CreateEmptySlots(int emptyID)
        {
            var slots = new List<int>(SLOT_SIZE);
            for (int i = 0; i < SLOT_SIZE; i++)
            {
                slots.Add(emptyID);
            }
            return slots;
        }

        #region 操作方法
        public void AddItem(int itemID)
        {
            int type = GetItemType(itemID);
            switch (type)
            {
                case 1: WeaponIDs.Add(itemID); break;
                case 2: HelmetIDs.Add(itemID); break;
                case 3: BodyIDs.Add(itemID); break;
                case 4: LegIDs.Add(itemID); break;
                case 5: HandIDs.Add(itemID); break;
                case 6: ConsumableIDs.Add(itemID); break;
                case 7: SpellIDs.Add(itemID); break;
            }
        }

        public void RemoveItem(int itemID)
        {
            int type = GetItemType(itemID);
            switch (type)
            {
                case 1: WeaponIDs.Remove(itemID); break;
                case 2: HelmetIDs.Remove(itemID); break;
                case 3: BodyIDs.Remove(itemID); break;
                case 4: LegIDs.Remove(itemID); break;
                case 5: HandIDs.Remove(itemID); break;
                case 6: ConsumableIDs.Remove(itemID); break;
                case 7: SpellIDs.Remove(itemID); break;
            }
        }

        public void SetSlotItem(int slotType, int slotIndex, int itemID)
        {
            if (slotIndex < 0 || slotIndex >= SLOT_SIZE) return;

            switch (slotType)
            {
                case 0: RightHandSlotIDs[slotIndex] = itemID; break;
                case 1: LeftHandSlotIDs[slotIndex] = itemID; break;
                case 2: ConsumableSlotIDs[slotIndex] = itemID; break;
                case 3: SpellSlotIDs[slotIndex] = itemID; break;
            }
        }

        public bool IsEmptySlot(int itemID)
        {
            return itemID == IInventoryModel.EMPTY_WEAPON_ID ||
                   itemID == IInventoryModel.EMPTY_HELMET_ID ||
                   itemID == IInventoryModel.EMPTY_BODY_ID ||
                   itemID == IInventoryModel.EMPTY_LEGS_ID ||
                   itemID == IInventoryModel.EMPTY_HANDS_ID ||
                   itemID == IInventoryModel.EMPTY_CONSUMABLE_ID ||
                   itemID == IInventoryModel.EMPTY_SPELL_ID;
        }

        public int GetItemType(int itemID)
        {
            if (itemID >= 1000 && itemID < 2000) return 1; // 武器
            if (itemID >= 2000 && itemID < 3000) return 2; // 头盔
            if (itemID >= 3000 && itemID < 4000) return 3; // 身体
            if (itemID >= 4000 && itemID < 5000) return 4; // 腿部
            if (itemID >= 5000 && itemID < 6000) return 5; // 手部
            if (itemID >= 6000 && itemID < 7000) return 6; // 消耗品
            if (itemID >= 7000 && itemID < 8000) return 7; // 法术
            return 0; // 未知
        }
        #endregion

        #region 存档集成
        public void ImportFromInventoryData(PlayerInventoryData data)
        {
            if (data == null) return;

            // 库存列表
            WeaponIDs.Reset(data.weaponInventoryIDs);
            HelmetIDs.Reset(data.headEquipmentInventoryIDs);
            BodyIDs.Reset(data.bodyEquipmentInventoryIDs);
            LegIDs.Reset(data.legEquipmentInventoryIDs);
            HandIDs.Reset(data.handEquipmentInventoryIDs);
            ConsumableIDs.Reset(data.consumableInventoryIDs);
            SpellIDs.Reset(data.spellInventoryIDs);

            // 快速插槽
            RightHandSlotIDs = new List<int>(data.rightHandSlotIDs ?? CreateEmptySlots(IInventoryModel.EMPTY_WEAPON_ID));
            LeftHandSlotIDs = new List<int>(data.leftHandSlotIDs ?? CreateEmptySlots(IInventoryModel.EMPTY_WEAPON_ID));
            ConsumableSlotIDs = new List<int>(data.consumableSlotIDs ?? CreateEmptySlots(IInventoryModel.EMPTY_CONSUMABLE_ID));
            SpellSlotIDs = new List<int>(data.spellSlotIDs ?? CreateEmptySlots(IInventoryModel.EMPTY_SPELL_ID));

            // 当前装备
            CurrentHelmetID.Value = data.currentHelmetID;
            CurrentBodyID.Value = data.currentBodyID;
            CurrentLegsID.Value = data.currentLegsID;
            CurrentHandsID.Value = data.currentHandsID;

            // 当前索引
            CurrentRightWeaponIndex.Value = data.currentRightWeaponIndex;
            CurrentLeftWeaponIndex.Value = data.currentLeftWeaponIndex;
            CurrentConsumableIndex.Value = data.currentConsumableIndex;
            CurrentSpellIndex.Value = data.currentSpellIndex;
        }

        public void ExportToInventoryData(PlayerInventoryData data)
        {
            if (data == null) return;

            // 库存列表
            data.weaponInventoryIDs = new List<int>(WeaponIDs);
            data.headEquipmentInventoryIDs = new List<int>(HelmetIDs);
            data.bodyEquipmentInventoryIDs = new List<int>(BodyIDs);
            data.legEquipmentInventoryIDs = new List<int>(LegIDs);
            data.handEquipmentInventoryIDs = new List<int>(HandIDs);
            data.consumableInventoryIDs = new List<int>(ConsumableIDs);
            data.spellInventoryIDs = new List<int>(SpellIDs);

            // 快速插槽
            data.rightHandSlotIDs = new List<int>(RightHandSlotIDs);
            data.leftHandSlotIDs = new List<int>(LeftHandSlotIDs);
            data.consumableSlotIDs = new List<int>(ConsumableSlotIDs);
            data.spellSlotIDs = new List<int>(SpellSlotIDs);

            // 当前装备
            data.currentHelmetID = CurrentHelmetID.Value;
            data.currentBodyID = CurrentBodyID.Value;
            data.currentLegsID = CurrentLegsID.Value;
            data.currentHandsID = CurrentHandsID.Value;

            // 当前索引
            data.currentRightWeaponIndex = CurrentRightWeaponIndex.Value;
            data.currentLeftWeaponIndex = CurrentLeftWeaponIndex.Value;
            data.currentConsumableIndex = CurrentConsumableIndex.Value;
            data.currentSpellIndex = CurrentSpellIndex.Value;
        }
        #endregion
    }
}
