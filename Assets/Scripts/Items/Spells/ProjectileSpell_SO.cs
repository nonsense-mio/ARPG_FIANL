
using UnityEngine;

namespace ARPG
{
    [CreateAssetMenu(menuName = "Spells/Projectile Spell")]
    public class ProjectileSpell : SpellItem
    {
        [Header("Projectile Stats")]
        public float baseDamage;
        public float projectileForwardVelocity;
        public float projectileUpWardVelocity;
        public float projectileMass;
        public bool isEffectedByGravity;


        // public override void AttemptToCastSpell(CharacterManager character)
        // {
        //     base.AttemptToCastSpell(character);
        //     // if (character.isUsingRightHand)
        //     // {
        //     //     character.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true, false);
        //     // }
        //     // else
        //     // {
        //     //     character.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true, false, true);
        //     // }
        // }

        public override void SuccessfullyCastSpell(CharacterManager character)
        {
            base.SuccessfullyCastSpell(character);
            
        }
    }
}
