using System;
using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace ARPG
{
    public class EquipPanel : BasePanel
    {
        private Image weaponGroup;
        private Image armorGroup;
        private Image scGroup;
        public List<BagItem> equipSlotList = new List<BagItem>();

        private IInventoryModel inventoryModel;
        private IPoolSystem poolSystem;

        #region 槽位绑定描述

        /// <summary>
        /// 描述一组装备槽的绑定规则：索引范围、背包分类、类型校验
        /// </summary>
        private struct SlotBinding
        {
            public int startIndex, endIndex;
            public InventoryCategory category;
            public Func<Item_SO, bool> typeCheck;
            public string warningMsg;
        }

        private static readonly SlotBinding[] slotBindings = new SlotBinding[]
        {
            new SlotBinding { startIndex = 0,  endIndex = 8,  category = InventoryCategory.Weapon,     typeCheck = i => i is WeaponItem_SO,    warningMsg = "选中的不是武器！" },
            new SlotBinding { startIndex = 8,  endIndex = 9,  category = InventoryCategory.Helmet,     typeCheck = i => i is HelmetEquipment,   warningMsg = "选中的不是头盔！" },
            new SlotBinding { startIndex = 9,  endIndex = 10, category = InventoryCategory.Body,       typeCheck = i => i is BodyEquipment,     warningMsg = "选中的不是盔甲！" },
            new SlotBinding { startIndex = 10, endIndex = 11, category = InventoryCategory.Leg,        typeCheck = i => i is LegEquipment,      warningMsg = "选中的不是腿甲！" },
            new SlotBinding { startIndex = 11, endIndex = 12, category = InventoryCategory.Hand,       typeCheck = i => i is HandEquipment,     warningMsg = "选中的不是手甲！" },
            new SlotBinding { startIndex = 12, endIndex = 16, category = InventoryCategory.Consumable, typeCheck = i => i is ConsumableItem_SO, warningMsg = "选中的不是消耗品！" },
            new SlotBinding { startIndex = 16, endIndex = 20, category = InventoryCategory.Spell,      typeCheck = i => i is SpellItem,         warningMsg = "选中的不是法术！" },
        };

        private static SlotBinding FindBinding(int index)
        {
            foreach (var b in slotBindings)
                if (index >= b.startIndex && index < b.endIndex)
                    return b;
            return slotBindings[0];
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            weaponGroup = GetControl<Image>("weaponGroup");
            armorGroup = GetControl<Image>("armorGroup");
            scGroup = GetControl<Image>("scGroup");
            inventoryModel = this.GetModel<IInventoryModel>();
            poolSystem = this.GetSystem<IPoolSystem>();
        }

        /// <summary>
        /// 外部可调用：刷新装备槽显示
        /// </summary>
        public void Refresh()
        {
            LoadItemImage();
        }

        #region ShowMe / HideMe

        private void SpawnSlots(Transform parent, int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject obj = poolSystem.Spawn("UI/btnBagItem");
                obj.transform.SetParent(parent, false);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                equipSlotList.Add(obj.GetComponent<BagItem>());
            }
        }

        public override void ShowMe()
        {
            SpawnSlots(weaponGroup.transform, 8);  // 右手武器 0-3, 左手武器 4-7
            SpawnSlots(armorGroup.transform, 4);    // 头/身/腿/手 8-11
            SpawnSlots(scGroup.transform, 8);       // 消耗品 12-15, 法术 16-19
            equipSlotList[0].btnSlot.Select();
            LoadItemImage();
        }

        public override void HideMe()
        {
            for (int i = 0; i < equipSlotList.Count; i++)
            {
                equipSlotList[i].iconImage.sprite = null;
                equipSlotList[i].btnSlot.onClick.RemoveAllListeners();
                poolSystem.Recycle(equipSlotList[i].gameObject);
            }
            equipSlotList.Clear();
        }

        #endregion

        //打开背包并选择
        private void OpenBagAndSelect(Action<BagPanel> initBag)
        {
            this.GetSystem<IUISystem>().HidePanel<EquipPanel>();
            this.GetSystem<IUISystem>().ShowPanel<BagPanel>(callBack: (bagPanel) =>
            {
                bagPanel.RequestInitAllInventory();
                initBag?.Invoke(bagPanel);
            });
        }

        //显示图片 + 绑定点击事件
        private void LoadItemImage()
        {
            // 先清掉旧监听并刷新图标
            for (int i = 0; i < equipSlotList.Count; i++)
            {
                equipSlotList[i].btnSlot.onClick.RemoveAllListeners();

                int id = inventoryModel.GetEquipSlotID(i);
                Item_SO item = inventoryModel.IsEmptySlot(id) ? null : ItemDataBase.Instance.GetItemByID(id);
                if (item != null && item.itemIcon != null)
                {
                    equipSlotList[i].iconImage.sprite = item.itemIcon;
                    equipSlotList[i].iconImage.enabled = true;
                }
                else
                {
                    equipSlotList[i].iconImage.sprite = null;
                    equipSlotList[i].iconImage.enabled = false;
                }
            }

            // 为每个装备槽绑定点击事件 (数据驱动)
            for (int i = 0; i < equipSlotList.Count; i++)
            {
                int index = i;
                var binding = FindBinding(index);

                equipSlotList[index].btnSlot.onClick.AddListener(() =>
                {
                    OpenBagAndSelect(bagPanel =>
                    {
                        bagPanel.RequestInitInventory(binding.category);
                        bagPanel.SetSelectCallback(selectedItem =>
                        {
                            if (binding.typeCheck(selectedItem))
                                this.SendCommand(new ChangeEquipItemCommand(index, selectedItem));
                            else
                                Debug.LogWarning(binding.warningMsg);
                        });
                    });
                });
            }
        }
    }
}
