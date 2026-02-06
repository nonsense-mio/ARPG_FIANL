using System.Collections;
using System.Collections.Generic;
using ARPG;
using UnityEngine;

namespace HT
{
    [CreateAssetMenu(menuName ="Items/Consumbales/Flask")]
    public class FlaskItem_SO : ConsumableItem_SO
    {
        [Header("烧瓶种类")]
        public bool estusFlask;
        public bool ashenFlask;
        public bool detoxFlask;

        [Header("回复量")]
        public int healthRecoverAmount;
        public int focusRecoverAmount;


        public override void AttemptToConsumeItem(PlayerManager player)
        {
            base.AttemptToConsumeItem(player);
            //GameObject flask = Instantiate(itemModel, player.playerWeaponSlotManager.rightHandSlot.transform);
            //生成瓶子模型
            GameObject flask = GameArchitecture.Interface.GetSystem<PoolSystem>().Spawn(itemModelName);
            flask.transform.parent = player.playerWeaponSlotManager.rightHandSlot.transform;
            flask.transform.localPosition = Vector3.zero;
            flask.transform.localRotation = Quaternion.identity;
            //如果是解毒烧瓶
            if(detoxFlask)
            {
                player.playerEffectsManager.poisonBuildUp = 0;
                player.playerEffectsManager.poisonAmount = player.playerEffectsManager.defaultPoisonAmount;
                player.playerEffectsManager.isPoisoned = false;
                if(player.playerEffectsManager.currentPoisonParticleFX != null)
                {   //回收中毒特效
                    GameArchitecture.Interface.GetSystem<PoolSystem>().Recycle(player.playerEffectsManager.currentPoisonParticleFX);
                    player.playerEffectsManager.currentPoisonParticleFX = null;
                }
            }
            //playerEffectsManager.currentParticleFX = recoveryFX;
            player.playerEffectsManager.amountToBeHealed = healthRecoverAmount;
            player.playerEffectsManager.focusTobeAdded = focusRecoverAmount;
            player.playerEffectsManager.instantiatedFXModel = flask;
            player.playerWeaponSlotManager.rightHandSlot.UnloadWeapon();
        }
    }
}

