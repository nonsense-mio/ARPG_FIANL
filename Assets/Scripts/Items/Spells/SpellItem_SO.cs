
using UnityEngine;

namespace ARPG
{
    public class SpellItem : Item_SO
    {
        public string spellWarmUpFXName = "FX/SpellWarmUp";
        public string spellCastFXName = "FX/SpellCast";
        public string spellAnimation;

        [Header("Spell Cost")]
        public int focusPointCost;

        [Header("Spell Type")]
        public bool isFaithSpell;
        public bool isMagicSpell;
        public bool isPyroSpell;

        [Header("Spell Description")]
        [TextArea]
        public string spellDescription;

        /// <summary>
        /// 施法
        /// </summary>
        /// <param name="animatorHandler"></param>
        /// <param name="playerStats"></param>
        /// <param name="weaponSlotManager"></param>
        public virtual void AttemptToCastSpell(CharacterManager character)
        {
            character.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true,false,character.isUsingLeftHand);
            GameArchitecture.Interface.SendEvent(new PlayerWarmUpSpellEvent { Character = character, Spell = this });
        }
        /// <summary>
        /// 施法成功
        /// </summary>
        /// <param name="animatorHandler"></param>
        /// <param name="playerStats"></param>
        public virtual void SuccessfullyCastSpell(CharacterManager character)
        {
            GameArchitecture.Interface.SendEvent(new PlayerCastSpellEvent { Character = character, Spell = this });
            PlayerManager player = character as PlayerManager;
            if (player == null)
                return;
            //减少focus
            player.playerStatsManager.DeductFocusPoints(focusPointCost);         
        }
    }
}

