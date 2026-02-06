using ARPG;


namespace HT
{
    //物品交互类
    public class ItemPickUp : Interactable
    {
        public Item_SO item;

        protected override void Awake()
        {
            base.Awake();
            
        }
        private void Start()
        {
            interactionPrompt = "拾取" + item.itemName;
        }
        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);

            //捡起武器
            PickUpItem(playerManager);
        }
        /// <summary>
        /// 拾取物品的方法
        /// </summary>
        /// <param name="playerManager"></param>
        private void PickUpItem(PlayerManager player)
        {

            //播放拾取物品的动画
            player.playerAnimatorManager.PlayTargetAnimation(animationName, true);
            //把拾取到的物品添加到玩家库存中 (三路写入: Runtime + Model + 持久化)
            GameArchitecture.Interface.SendCommand(new AddItemToInventoryCommand(item));

            GameArchitecture.Interface.GetSystem<PoolSystem>().Recycle(gameObject);
        }
    }
}

