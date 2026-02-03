using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class PassThroughFogWall : Interactable
    {

        [SerializeField]
         private BossFightManager bossFight;
        protected override void Awake()
        {
            base.Awake();
        }
        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            playerManager.PassThroughFogWallInteraction(transform);
            // 激活 Boss 战
            bossFight?.ActivateBossFight();
        }
        public void SetActive(bool activate)
        {
            gameObject.SetActive(activate);
        }
    }
}

