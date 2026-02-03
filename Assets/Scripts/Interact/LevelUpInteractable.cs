using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class LevelUpInteractable : Interactable
    {
        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            //playerManager.uiManager.levelUpWindow.SetActive(true);
            UIMgr.Instance.ShowPanel<LevelUpPanel>();
            UIMgr.Instance.HidePanel<GamePanel>();
        }
    }

}
