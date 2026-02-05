using ARPG;
using Framework;
using UnityEngine;

namespace HT
{
    //可交互物品的基类
    public class Interactable : MonoBehaviour
    {
        public float radius = 0.6f;
        public string interactionPrompt = "交互";
        public string soundEffectName = "interact";
        public string animationName = "Pick Up Item";
        protected virtual void Awake()
        {
            soundEffectName = "interact";
        }

        /// <summary>
        /// 交互方法 (需要与与玩家交互时调用)
        /// </summary>
        /// <param name="playerManager"></param>
        public virtual void Interact(PlayerManager playerManager)
        {
            GameArchitecture.Interface.SendEvent(new PerformInteractionEvent { Target = this });
            //交互后保存游戏数据
            playerManager.playerSaveManager.SaveDataToGameDataMgr();
        }
    }
}

