
using UnityEngine;

namespace ARPG
{
    [CreateAssetMenu(menuName = "Item Actions/Aim Action")]
    public class AimAction_SO : ItemAction_SO
    {
        public override void PerformAction(CharacterManager character)
        {
            PlayerManager player = character as PlayerManager;
            if (character.isAiming || character.isUsingComsumable)
                return;
            if (player != null)
            {
                GameArchitecture.Interface.SendEvent(new AimActionEvent { IsAiming = true });
            }

            character.isAiming = true;
        }
    }
}

