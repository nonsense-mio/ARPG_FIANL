using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HT
{
    [CreateAssetMenu(menuName = "Item Actions/Pyromancy Spell Action")]
    public class PyromancySpellAction_SO : ItemAction_SO
    {
        public override void PerformAction(CharacterManager character)
        {
            if (character.isInteracting || character.isUsingComsumable)
                return;

            if (character.characterInventoryManager.currentSpell != null && character.characterInventoryManager.currentSpell.isPyroSpell)
            {   //如果当前的focuspoints 大于施法所需的focuspoints
                if (character.characterStatsManager.currentFocus >= character.characterInventoryManager.currentSpell.focusPointCost)
                {
                    character.characterInventoryManager.currentSpell.AttemptToCastSpell(character);
                }
                else
                {
                    //法力不足播放无奈动画
                    character.characterAnimatorManager.PlayTargetAnimation("Shrug", true);
                }
            }
        }
    }
}

