using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            //把拾取到的武器添加到玩家武器库存中
            player.playerInventoryManager.AddItemToInventory(item);

            PoolMgr.Instance.PushObj(gameObject);
        }
    }
}

