using Framework;
using ARPG;

namespace ARPG
{
    /// <summary>
    /// 添加物品到背包命令 - 双路写入:
    /// 1. PlayerInventoryManager (运行时列表)
    /// 2. IInventoryModel (ID列表，驱动UI响应式更新)
    /// </summary>
    public class AddItemToInventoryCommand : AbstractCommand
    {
        private Item_SO item;

        public AddItemToInventoryCommand() { }

        public AddItemToInventoryCommand(Item_SO item)
        {
            this.item = item;
        }

        protected override void OnExecute()
        {
            var pm = PlayerManager.localPlayer;
            if (pm == null) return;

            // 1) 运行时: 添加到对应类型的库存列表
            pm.playerInventoryManager.AddItemToInventory(item);

            // 2) 同步 IInventoryModel (根据ID范围自动分类)
            var model = this.GetModel<IInventoryModel>();
            model.AddItem(item.itemID);

            // 3) 局部保存背包数据
            this.GetSystem<ISaveSystem>().SaveInventoryData();
        }
    }
}
