using System;
using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace HT
{
    public class EquipPanel : BasePanel
    {
        private Image weaponGroup;
        private Image armorGroup;
        private Image scGroup;
        public List<BagItem> equipSlotList = new List<BagItem>();

        private IInventoryModel inventoryModel;

        protected override void Awake()
        {
            base.Awake();
            weaponGroup = GetControl<Image>("weaponGroup");
            armorGroup = GetControl<Image>("armorGroup");
            scGroup = GetControl<Image>("scGroup");
            inventoryModel = this.GetModel<IInventoryModel>();
        }

        /// <summary>
        /// 外部可调用：刷新装备槽显示
        /// </summary>
        public void Refresh()
        {
            LoadItemImage();
        }

        public override void ShowMe()
        {
            for (int i = 0; i < 8; i++)
            {
                GameObject obj = PoolMgr.Instance.GetObj("UI/btnBagItem");
                obj.transform.SetParent(weaponGroup.transform, false);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                BagItem bagItem = obj.GetComponent<BagItem>();
                equipSlotList.Add(bagItem);
            }
            for (int i = 0; i < 4; i++)
            {
                GameObject obj = PoolMgr.Instance.GetObj("UI/btnBagItem");
                obj.transform.SetParent(armorGroup.transform, false);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                BagItem bagItem = obj.GetComponent<BagItem>();
                equipSlotList.Add(bagItem);
            }
            for (int i = 0; i < 8; i++)
            {
                GameObject obj = PoolMgr.Instance.GetObj("UI/btnBagItem");
                obj.transform.SetParent(scGroup.transform, false);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                BagItem bagItem = obj.GetComponent<BagItem>();
                equipSlotList.Add(bagItem);
            }
            equipSlotList[0].btnSlot.Select();
            LoadItemImage();
        }

        public override void HideMe()
        {
            for (int i = 0; i < equipSlotList.Count; i++)
            {
                equipSlotList[i].iconImage.sprite = null;
                equipSlotList[i].btnSlot.onClick.RemoveAllListeners();
                PoolMgr.Instance.PushObj(equipSlotList[i].gameObject);
            }
            equipSlotList.Clear();
        }

        //打开背包并选择
        private void OpenBagAndSelect(Action<BagPanel> initBag)
        {
            UIMgr.Instance.HidePanel<EquipPanel>();
            UIMgr.Instance.ShowPanel<BagPanel>(callBack: (bagPanel) =>
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

            //为每个装备槽绑定点击事件
            for (int i = 0; i < equipSlotList.Count; i++)
            {
                int index = i;
                #region 左右手武器装备槽
                // 右手武器 0-3 / 左手武器 4-7
                if (index < 8)
                {
                    equipSlotList[index].btnSlot.onClick.AddListener(() =>
                    {
                        OpenBagAndSelect((bagPanel) =>
                        {
                            bagPanel.RequestInitWeaponInventory();
                            bagPanel.SetSelectCallback((selectedItem) =>
                            {
                                if (selectedItem is WeaponItem_SO)
                                    this.SendCommand(new ChangeEquipItemCommand(index, selectedItem));
                                else
                                    Debug.LogWarning("选中的不是武器！");
                            });
                        });
                    });
                    continue;
                }
                #endregion
                #region 防具装备槽
                // 头/身/腿/手 8-11
                if (index >= 8 && index < 12)
                {
                    if (index == 8) // helmet
                    {
                        equipSlotList[index].btnSlot.onClick.AddListener(() =>
                        {
                            OpenBagAndSelect((bagPanel) =>
                            {
                                bagPanel.RequestInitHelmetInventory();
                                bagPanel.SetSelectCallback((selectedItem) =>
                                {
                                    if (selectedItem is HelmetEquipment)
                                        this.SendCommand(new ChangeEquipItemCommand(8, selectedItem));
                                    else
                                        Debug.LogWarning("选中的不是头盔！");
                                });
                            });
                        });
                    }
                    else if (index == 9) // body
                    {
                        equipSlotList[index].btnSlot.onClick.AddListener(() =>
                        {
                            OpenBagAndSelect((bagPanel) =>
                            {
                                bagPanel.RequestInitBodyInventory();
                                bagPanel.SetSelectCallback((selectedItem) =>
                                {
                                    if (selectedItem is BodyEquipment)
                                        this.SendCommand(new ChangeEquipItemCommand(9, selectedItem));
                                    else
                                        Debug.LogWarning("选中的不是盔甲！");
                                });
                            });
                        });
                    }
                    else if (index == 10) // legs
                    {
                        equipSlotList[index].btnSlot.onClick.AddListener(() =>
                        {
                            OpenBagAndSelect((bagPanel) =>
                            {
                                bagPanel.RequestInitLegInventory();
                                bagPanel.SetSelectCallback((selectedItem) =>
                                {
                                    if (selectedItem is LegEquipment)
                                        this.SendCommand(new ChangeEquipItemCommand(10, selectedItem));
                                    else
                                        Debug.LogWarning("选中的不是腿甲！");
                                });
                            });
                        });
                    }
                    else if (index == 11) // hands
                    {
                        equipSlotList[index].btnSlot.onClick.AddListener(() =>
                        {
                            OpenBagAndSelect((bagPanel) =>
                            {
                                bagPanel.RequestInitHandInventory();
                                bagPanel.SetSelectCallback((selectedItem) =>
                                {
                                    if (selectedItem is HandEquipment)
                                        this.SendCommand(new ChangeEquipItemCommand(11, selectedItem));
                                    else
                                        Debug.LogWarning("选中的不是手甲！");
                                });
                            });
                        });
                    }
                    continue;
                }
                #endregion
                #region 消耗品装备槽
                // 消耗品 12-15
                if (index >= 12 && index < 16)
                {
                    equipSlotList[index].btnSlot.onClick.AddListener(() =>
                    {
                        OpenBagAndSelect((bagPanel) =>
                        {
                            bagPanel.RequestInitConsumableInventory();
                            bagPanel.SetSelectCallback((selectedItem) =>
                            {
                                if (selectedItem is ConsumableItem_SO)
                                    this.SendCommand(new ChangeEquipItemCommand(index, selectedItem));
                                else
                                    Debug.LogWarning("选中的不是消耗品！");
                            });
                        });
                    });
                    continue;
                }
                #endregion
                #region 法术装备槽
                // 法术 16+
                if (index >= 16)
                {
                    equipSlotList[index].btnSlot.onClick.AddListener(() =>
                    {
                        OpenBagAndSelect((bagPanel) =>
                        {
                            bagPanel.RequestInitSpellInventory();
                            bagPanel.SetSelectCallback((selectedItem) =>
                            {
                                if (selectedItem is SpellItem)
                                    this.SendCommand(new ChangeEquipItemCommand(index, selectedItem));
                                else
                                    Debug.LogWarning("选中的不是法术！");
                            });
                        });
                    });
                }
                #endregion
            }
        }
    }
}
