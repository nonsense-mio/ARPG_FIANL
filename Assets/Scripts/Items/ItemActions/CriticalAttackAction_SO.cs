using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    [CreateAssetMenu(menuName = "Item Actions/Critical Attack Action")]
    public class CriticalAttackAction_SO : ItemAction_SO
    {
        public override void PerformAction(CharacterManager character)
        {
            if(character.isInteracting || character.isUsingComsumable)
                return;
            character.characterCombatManager.AttemptBackStabOrRiposte();
        }
    }
}

