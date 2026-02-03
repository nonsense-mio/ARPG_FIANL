using System.Collections;
using System.Collections.Generic;
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

        //用于存储外部传入的“选择物品后要做什么”的回调
        private System.Action<Item_SO> onEquipCallback;

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
        private void EnsureFacade()
        {
            if (playerFacade == null)
                Debug.LogError($"{nameof(BagPanel)} 未绑定 playerFacade，请在 ShowPanel/GetPanel 回调里调用 panel.Bind(facade)");
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
        private void InitWeaponInventory()
        {
            EnsureFacade();
            sv.InitInfos(new List<Item_SO>(playerFacade.GetWeaponInventory()));
        }
        private void InitHelmetInventory()
        {
            EnsureFacade();
            sv.InitInfos(new List<Item_SO>(playerFacade.GetHelmetInventory()));
        }
        private void InitBodyInventory()
        {
            EnsureFacade();
            sv.InitInfos(new List<Item_SO>(playerFacade.GetBodyInventory()));
        }
        private void InitLegInventory()
        {
            EnsureFacade();
            sv.InitInfos(new List<Item_SO>(playerFacade.GetLegInventory()));
        }
        private void InitHandInventory()
        {
            EnsureFacade();
            sv.InitInfos(new List<Item_SO>(playerFacade.GetHandInventory()));
        }
        private void InitConsumableInventory()
        {
            EnsureFacade();
            sv.InitInfos(new List<Item_SO>(playerFacade.GetConsumableInventory()));
        }
        private void InitSpellInventory()
        {
            EnsureFacade();
            sv.InitInfos(new List<Item_SO>(playerFacade.GetSpellInventory()));
        }
        private void InitAllInventory()
        {
            EnsureFacade();
            sv.InitInfos(playerFacade.GetAllInventorySnapshot());
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
        // 每帧调用以处理请求
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

            //如果有回调，说明是处于“选择模式”（比如从装备面板跳过来的）
            if (onEquipCallback != null)
            {
                onEquipCallback(item);
                // 执行完回调后，关闭背包面板，刷新显示
                //选完装备就关闭背包回装备面板
                UIMgr.Instance.HidePanel<BagPanel>();
                UIMgr.Instance.ShowPanel<EquipPanel>(callBack: (equipPanel) =>
                {
                    // 把 facade 继续传给 EquipPanel，避免 EquipPanel 再去找 player
                    equipPanel.Bind(playerFacade);
                    // EquipPanel.ShowMe 已经调用过了；这里主动刷新一次显示
                    equipPanel.Refresh();
                });
                //更新快捷栏UI显示
                UIMgr.Instance.GetPanel<GamePanel>((panel) =>
                {
                    panel.Bind(playerFacade);
                    panel.UpdateAllQuickSlots();
                });
            }
            else
            {
                // 否则是“查看模式”，什么都不做，或者显示详情
                Debug.Log("当前是查看模式，不执行装备逻辑");
            }
        }
    }
}

