using System.Collections;
using System.Collections.Generic;
using ARPG;
using UnityEngine;

namespace ARPG
{
    public class LevelUpInteractable : Interactable
    {
        private IUISystem uiSystem;
        private IUISystem UISystem =>
            uiSystem ?? (uiSystem = GameArchitecture.Interface.GetSystem<IUISystem>());

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            //playerManager.uiManager.levelUpWindow.SetActive(true);
            UISystem.ShowPanel<LevelUpPanel>();
            UISystem.HidePanel<GamePanel>();
        }
    }

}
