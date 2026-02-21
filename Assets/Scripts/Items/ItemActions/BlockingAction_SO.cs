
using UnityEngine;

namespace ARPG
{
    [CreateAssetMenu(menuName = "Item Actions/Blocking Action")]
    public class BlockingAction_SO : ItemAction_SO
    {
        public override void PerformAction(CharacterManager character)
        {
            if (!character.CanPerformAction())
                return;
            if (character.isBlocking)
                return;
            //设置格挡属性
            character.characterCombatManager.SetBlockingAbsorptions();
            character.isBlocking = true;

        }
    }

}
