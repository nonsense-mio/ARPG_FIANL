using System.Collections.Generic;
using Framework;

namespace ARPG
{
    /// <summary>
    /// 背包数据模型接口 - QFramework Model层
    /// 职责: 管理物品ID列表和快速插槽状态
    /// </summary>
    public interface IInventoryModel : IModel
    {
        #region 空槽位ID常量
        const int EMPTY_WEAPON_ID = 1000;
        const int EMPTY_HELMET_ID = 2000;
        const int EMPTY_BODY_ID = 3000;
        const int EMPTY_LEGS_ID = 4000;
        const int EMPTY_HANDS_ID = 5000;
        const int EMPTY_CONSUMABLE_ID = 6000;
        const int EMPTY_SPELL_ID = 7000;
        #endregion

        #region 库存ID列表
        BindableList<int> WeaponIDs { get; }
        BindableList<int> HelmetIDs { get; }
        BindableList<int> BodyIDs { get; }
        BindableList<int> LegIDs { get; }
        BindableList<int> HandIDs { get; }
        BindableList<int> ConsumableIDs { get; }
        BindableList<int> SpellIDs { get; }
        #endregion

        #region 快速插槽ID (固定4格)
        BindableList<int> RightHandSlotIDs { get; }
        BindableList<int> LeftHandSlotIDs { get; }
        BindableList<int> ConsumableSlotIDs { get; }
        BindableList<int> SpellSlotIDs { get; }
        #endregion

        #region 当前装备ID
        BindableProperty<int> CurrentHelmetID { get; }
        BindableProperty<int> CurrentBodyID { get; }
        BindableProperty<int> CurrentLegsID { get; }
        BindableProperty<int> CurrentHandsID { get; }
        #endregion

        #region 当前索引
        BindableProperty<int> CurrentRightWeaponIndex { get; }
        BindableProperty<int> CurrentLeftWeaponIndex { get; }
        BindableProperty<int> CurrentConsumableIndex { get; }
        BindableProperty<int> CurrentSpellIndex { get; }
        #endregion

        #region 操作方法
        /// <summary>
        /// 添加物品到库存（根据ID范围自动分类）
        /// </summary>
        void AddItem(int itemID);

        /// <summary>
        /// 从库存移除物品
        /// </summary>
        void RemoveItem(int itemID);

        /// <summary>
        /// 设置快速插槽物品
        /// </summary>
        /// <param name="slotType">槽位类型: 0=右手, 1=左手, 2=消耗品, 3=法术</param>
        /// <param name="slotIndex">槽位索引 (0-3)</param>
        /// <param name="itemID">物品ID</param>
        void SetSlotItem(int slotType, int slotIndex, int itemID);

        /// <summary>
        /// 判断物品ID是否为空槽位
        /// </summary>
        bool IsEmptySlot(int itemID);

        /// <summary>
        /// 根据ID获取物品类型 (1=武器, 2=头盔, 3=身体, 4=腿, 5=手, 6=消耗品, 7=法术)
        /// </summary>
        int GetItemType(int itemID);
        #endregion

        /// <summary>
        /// 获取指定装备槽位的物品ID
        /// 槽位映射: 0-3 右手武器, 4-7 左手武器, 8 头盔, 9 身体, 10 腿部, 11 手部,
        ///          12-15 消耗品, 16-19 法术
        /// </summary>
        int GetEquipSlotID(int slotIndex);

        #region 存档集成
        /// <summary>
        /// 从 PlayerInventoryData 导入数据 (存档加载时调用)
        /// </summary>
        void ImportFromInventoryData(PlayerInventoryData data);

        /// <summary>
        /// 导出到 PlayerInventoryData (存档保存时调用)
        /// </summary>
        void ExportToInventoryData(PlayerInventoryData data);
        #endregion
    }
}
