
using UnityEngine;
namespace ARPG
{
    [CreateAssetMenu(menuName ="Items/Consumbales/Bomb Item")]
    public class BombConsumableItem_SO : ConsumableItem_SO
    {
        [Header("速度")]
        public int upwardVelocity = 50;
        public int forwardVelocity = 50;
        public int bombMass = 1;
        [Header("炸弹模型")]
        public GameObject liveBombModel;
        public string liveBombModelName;
        [Header("爆炸伤害")]
        public int baseDamage = 200;
        public int explosiveDamage = 75;

        public override void AttemptToConsumeItem(PlayerManager player)
        {
            if(currentItemAmount > 0)
            {
                player.isUsingComsumable = true;
                player.playerWeaponSlotManager.rightHandSlot.UnloadWeapon();
                player.playerAnimatorManager.PlayTargetAnimation(consumeAnimation, true);
                GameObject bombModel = GameArchitecture.Interface.GetSystem<IPoolSystem>().Spawn(itemModelName);
                bombModel.transform.position = player.playerWeaponSlotManager.rightHandSlot.transform.position;
                bombModel.transform.rotation = Quaternion.identity;
                bombModel.transform.SetParent(player.playerWeaponSlotManager.rightHandSlot.transform);
                player.playerEffectsManager.instantiatedFXModel = bombModel;
            }
            else
            {
               player.playerAnimatorManager.PlayTargetAnimation("Shrug", true);
            }

        }
    }
}

