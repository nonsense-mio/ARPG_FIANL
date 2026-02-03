using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    [CreateAssetMenu(menuName = "Spells/Healing Spell")]
    public class HealingSpell : SpellItem
    {
        public int healAmount;

        //治疗法术
        // public override void AttemptToCastSpell(CharacterManager character)
        // {
        //     base.AttemptToCastSpell(character);
        //     character.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true,false,character.isUsingLeftHand);
        // }

        public override void SuccessfullyCastSpell(CharacterManager character)
        {
            base.SuccessfullyCastSpell(character);
            
            character.characterStatsManager.HealCharacter(healAmount);
        }
    }

}
