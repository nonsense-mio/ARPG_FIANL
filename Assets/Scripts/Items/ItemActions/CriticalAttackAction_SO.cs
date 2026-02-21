
using UnityEngine;

namespace ARPG
{
    [CreateAssetMenu(menuName = "Item Actions/Critical Attack Action")]
    public class CriticalAttackAction_SO : ItemAction_SO
    {
        public override void PerformAction(CharacterManager character)
        {
            if (!character.CanPerformAction())
                return;
            character.characterCombatManager.AttemptBackStabOrRiposte();
        }
    }
}

