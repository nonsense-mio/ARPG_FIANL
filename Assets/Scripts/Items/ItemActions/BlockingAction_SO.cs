using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    [CreateAssetMenu(menuName = "Item Actions/Blocking Action")]
    public class BlockingAction_SO : ItemAction_SO
    {
        public override void PerformAction(CharacterManager character)
        {
            if (character.isInteracting || character.isUsingComsumable)
                return;
            if (character.isBlocking)
                return;
            //设置格挡属性
            character.characterCombatManager.SetBlockingAbsorptions();
            character.isBlocking = true;

        }
    }

}
