
using UnityEngine;

namespace ARPG
{
    public class ConsumableItem_SO : Item_SO
    {
        [Header("物品数量")]
        public int maxItemAmount;
        public int currentItemAmount;
        [Header("物品模型")]
        public GameObject itemModel;
        public string itemModelName;
        [Header("动画")]
        public string consumeAnimation;
        public bool isInteracting;

        public virtual void AttemptToConsumeItem(PlayerManager player)
        {
           if(currentItemAmount > 0)
           {
               Debug.Log("你尝试使用消耗品");
               player.isUsingComsumable = true;
               player.playerAnimatorManager.PlayTargetAnimation(consumeAnimation, isInteracting,true);
               
           }
           else
           {
               player.playerAnimatorManager.PlayTargetAnimation("Shrug", true);
           }
        }
    }

}
