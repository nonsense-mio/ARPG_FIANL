using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

namespace HT
{
    public class BagPanel : BasePanel
    {
        public RectTransform content;
        CustomSV<Item_SO, BagItem> sv;

        private bool pendingInitAllInventory;
        private bool pendingInitWeaponInventory;
        private bool pendingInitHelmetInventory;
        private bool pendingInitBodyInventory;
        private bool pendingInitLegInventory;
        private bool pendingInitHandInventory;
        private bool pendingInitConsumableInventory;
        private bool pendingInitSpellInventory;

        //用于存储外部传入的"选择物品后要做什么"的回调
        private System.Action<Item_SO> onEquipCallback;

        private IInventoryModel inventoryModel;

        protected override void Awake()
        {
            base.Awake();
            inventoryModel = this.GetModel<IInventoryModel>();
        }

        // 提供给外部设置回调的方法
        public void SetSelectCallback(System.Action<Item_SO> callback)
        {
            this.onEquipCallback = callback;
        }

        public void Update()
        {
            //滚动列表的更新
            sv.CheckShowOrHide();
        }

        public override void ShowMe()
        {
            sv = new CustomSV<Item_SO, BagItem>();
            //初始化预设体资源路径
            sv.InitItemResName("UI/btnBagItem");
            //初始化格子间隔大小以及一行的格子个数
            sv.InitItemSizeAndNum(138, 130, 4);
            //初始化content以及可视范围
            sv.InitContentAndSVH(content, 1000);

            // 注册点击事件
            sv.AddListener(OnItemClick);

            //处理初始化哪个面板的方法
            HandleRequestInits();
        }

        public override void HideMe()
        {
            sv.ClearItem();
            //关闭面板时，必须清空回调，防止状态污染
            onEquipCallback = null;
            // 避免下次 ShowMe 意外触发旧请求
            pendingInitAllInventory = false;
        }

        #region 初始化背包相关

        private List<Item_SO> ConvertIDsToItems(BindableList<int> ids)
        {
            var items = new List<Item_SO>(ids.Count);
            foreach (int id in ids)
            {
                if (!inventoryModel.IsEmptySlot(id))
                {
                    var item = ItemDataBase.Instance.GetItemByID(id);
                    if (item != null)
                        items.Add(item);
                }
            }
            return items;
        }

        private void InitWeaponInventory()
        {
            sv.InitInfos(ConvertIDsToItems(inventoryModel.WeaponIDs));
        }
        private void InitHelmetInventory()
        {
            sv.InitInfos(ConvertIDsToItems(inventoryModel.HelmetIDs));
        }
        private void InitBodyInventory()
        {
            sv.InitInfos(ConvertIDsToItems(inventoryModel.BodyIDs));
        }
        private void InitLegInventory()
        {
            sv.InitInfos(ConvertIDsToItems(inventoryModel.LegIDs));
        }
        private void InitHandInventory()
        {
            sv.InitInfos(ConvertIDsToItems(inventoryModel.HandIDs));
        }
        private void InitConsumableInventory()
        {
            sv.InitInfos(ConvertIDsToItems(inventoryModel.ConsumableIDs));
        }
        private void InitSpellInventory()
        {
            sv.InitInfos(ConvertIDsToItems(inventoryModel.SpellIDs));
        }
        private void InitAllInventory()
        {
            var all = new List<Item_SO>();
            all.AddRange(ConvertIDsToItems(inventoryModel.WeaponIDs));
            all.AddRange(ConvertIDsToItems(inventoryModel.HelmetIDs));
            all.AddRange(ConvertIDsToItems(inventoryModel.BodyIDs));
            all.AddRange(ConvertIDsToItems(inventoryModel.LegIDs));
            all.AddRange(ConvertIDsToItems(inventoryModel.HandIDs));
            all.AddRange(ConvertIDsToItems(inventoryModel.ConsumableIDs));
            all.AddRange(ConvertIDsToItems(inventoryModel.SpellIDs));
            sv.InitInfos(all);
        }

        /// <summary>
        /// 外部调用：请求在面板完成 ShowMe 后初始化背包显示
        /// </summary>
        public void RequestInitAllInventory()
        {
            pendingInitAllInventory = true;
        }
        public void RequestInitWeaponInventory()
        {
            pendingInitWeaponInventory = true;
        }
        public void RequestInitHelmetInventory()
        {
            pendingInitHelmetInventory = true;
        }
        public void RequestInitBodyInventory()
        {
            pendingInitBodyInventory = true;
        }
        public void RequestInitLegInventory()
        {
            pendingInitLegInventory = true;
        }
        public void RequestInitHandInventory()
        {
            pendingInitHandInventory = true;
        }
        public void RequestInitConsumableInventory()
        {
            pendingInitConsumableInventory = true;
        }
        public void RequestInitSpellInventory()
        {
            pendingInitSpellInventory = true;
        }
        // 处理延迟初始化请求
        private void HandleRequestInits()
        {
            if (pendingInitAllInventory)
            {
                pendingInitAllInventory = false;
                InitAllInventory();
            }
            if (pendingInitWeaponInventory)
            {
                pendingInitWeaponInventory = false;
                InitWeaponInventory();
            }
            if (pendingInitHelmetInventory)
            {
                pendingInitHelmetInventory = false;
                InitHelmetInventory();
            }
            if (pendingInitBodyInventory)
            {
                pendingInitBodyInventory = false;
                InitBodyInventory();
            }
            if (pendingInitLegInventory)
            {
                pendingInitLegInventory = false;
                InitLegInventory();
            }
            if (pendingInitHandInventory)
            {
                pendingInitHandInventory = false;
                InitHandInventory();
            }
            if (pendingInitConsumableInventory)
            {
                pendingInitConsumableInventory = false;
                InitConsumableInventory();
            }
            if (pendingInitSpellInventory)
            {
                pendingInitSpellInventory = false;
                InitSpellInventory();
            }
        }

        #endregion

        /// <summary>
        /// 点击背包格子时触发的逻辑
        /// </summary>
        /// <param name="item">被点击的物品数据</param>
        private void OnItemClick(Item_SO item)
        {
            Debug.Log("点击了物品：" + item.itemName);

            //如果有回调，说明是处于"选择模式"（比如从装备面板跳过来的）
            if (onEquipCallback != null)
            {
                onEquipCallback(item);
                // 执行完回调后，关闭背包面板，刷新显示
                //选完装备就关闭背包回装备面板
                UIMgr.Instance.HidePanel<BagPanel>();
                UIMgr.Instance.ShowPanel<EquipPanel>(callBack: (equipPanel) =>
                {
                    equipPanel.Refresh();
                });
                //更新快捷栏UI显示
                UIMgr.Instance.GetPanel<GamePanel>((panel) =>
                {
                    panel.UpdateAllQuickSlots();
                });
            }
            else
            {
                // 否则是"查看模式"，什么都不做，或者显示详情
                Debug.Log("当前是查看模式，不执行装备逻辑");
            }
        }
    }
}
