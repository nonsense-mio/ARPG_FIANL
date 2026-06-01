
using UnityEngine;

namespace ARPG
{
    [CreateAssetMenu(menuName = "Items/Consumbales/Flask")]
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
            GameObject flask = GameArchitecture.Interface.GetSystem<IPoolSystem>().Spawn(itemModelName);
            flask.transform.parent = player.playerWeaponSlotManager.rightHandSlot.transform;
            flask.transform.localPosition = Vector3.zero;
            flask.transform.localRotation = Quaternion.identity;
            //如果是解毒烧瓶
            if (detoxFlask)
            {
                GameArchitecture.Interface.GetSystem<ITimerSystem>().CreateTimer(false, 2, () =>
                {
                    player.playerEffectsManager.CurePoison();
                });
                
            }
            //playerEffectsManager.currentParticleFX = recoveryFX;
            player.playerEffectsManager.amountToBeHealed = healthRecoverAmount;
            player.playerEffectsManager.focusTobeAdded = focusRecoverAmount;
            player.playerEffectsManager.instantiatedFXModel = flask;
            player.playerWeaponSlotManager.rightHandSlot.UnloadWeapon();
        }
    }
}

