using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

namespace HT
{
    /// <summary>
    /// 存档面板：显示4个存档槽位，点击空槽位开始新游戏，点击有存档的槽位进入游戏
    /// </summary>
    public class SavePanel : BasePanel
    {
        [SerializeField] private List<SaveSlotUI> slotUIList = new List<SaveSlotUI>();

        //添加按钮音效
        private void Start()
        {
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnBack"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnBack"));
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnSaveSlot1"));
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnSaveSlot2"));
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnSaveSlot3"));
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnSaveSlot4"));
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnDeleteSlot1"));
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnDeleteSlot2"));
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnDeleteSlot3"));
            UIMgr.AddUISelectSound(GetControl<UnityEngine.UI.Button>("btnDeleteSlot4"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnSaveSlot1"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnSaveSlot2"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnSaveSlot3"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnSaveSlot4"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnDeleteSlot1"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnDeleteSlot2"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnDeleteSlot3"));
            UIMgr.AddUIConfirmSound(GetControl<UnityEngine.UI.Button>("btnDeleteSlot4"));

        }

       
        //添加按钮点击事件
        protected override void ClickBtn(string btnName)
        {
            switch (btnName)
            {
                case "btnBack":
                    UIMgr.Instance.HidePanel<SavePanel>();
                    UIMgr.Instance.ShowPanel<BeginPanel>();
                    break;
                //槽位按钮
                case "btnSaveSlot1":
                    OnSlotClicked(0);
                    break;
                case "btnSaveSlot2":
                    OnSlotClicked(1);
                    break;
                case "btnSaveSlot3":
                    OnSlotClicked(2);
                    break;
                case "btnSaveSlot4":
                    OnSlotClicked(3);
                    break;
                //删除按钮 
                case "btnDeleteSlot1":
                    DeleteSaveSlot(0);
                    break;
                case "btnDeleteSlot2":
                    DeleteSaveSlot(1);
                    break;
                case "btnDeleteSlot3":
                    DeleteSaveSlot(2);
                    break;
                case "btnDeleteSlot4":
                    DeleteSaveSlot(3);
                    break;
            }
        }

        /// <summary>
        /// 刷新所有槽位的显示
        /// </summary>
        private void RefreshAllSlots()
        {
            SaveSlotInfo slotInfo = this.GetSystem<ISaveSystem>().SlotInfo;
            // 更新每个槽位的显示
            for (int i = 0; i < slotUIList.Count && i < SaveSlotInfo.MAX_SLOTS; i++)
            {
                slotUIList[i].UpdateDisplay(slotInfo.GetSlot(i));
            }
        }

        /// <summary>
        /// 槽位点击：空槽位开始新游戏，有存档则加载
        /// </summary>
        private void OnSlotClicked(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slotUIList.Count) return;

            bool isEmpty = slotUIList[slotIndex].IsEmpty();

            UIMgr.Instance.HidePanel<SavePanel>();
            UIMgr.Instance.HidePanel<BeginPanel>();
            GameManager.Instance.ClearInfo();

            if (isEmpty)
            {
                // 空槽位：开始新游戏
                GameManager.Instance.StartNewGame(slotIndex, "Player");
            }
            else
            {
                // 有存档：加载游戏
                GameManager.Instance.ContinueGame(slotIndex);
            }
        }

        private void DeleteSaveSlot(int slotIndex)
        {
            UIMgr.Instance.ShowPanel<TipPanel>(E_UILayer.Top, (panel) =>
            {
                panel.SetTipInfo("确定要删除该存档吗？", () =>
                {
                    this.GetSystem<ISaveSystem>().DeleteSave(slotIndex);
                    RefreshAllSlots();
                });
            });

        }

        public override void ShowMe()
        {
            RefreshAllSlots();
        }

        public override void HideMe()
        {
        }
    }
}

