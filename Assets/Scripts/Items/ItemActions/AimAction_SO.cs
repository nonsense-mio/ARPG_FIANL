using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
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
                EventCenter.Instance.EventTrigger<bool>(E_EventType.E_AimAction, true);
            }

            character.isAiming = true;
        }
    }
}

