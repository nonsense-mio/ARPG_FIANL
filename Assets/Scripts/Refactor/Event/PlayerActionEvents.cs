using ARPG;

namespace ARPG
{
    public struct PlayerDrinkPotionEvent
    {
        public PlayerManager Player;
    }

    public struct PlayerWarmUpSpellEvent
    {
        public CharacterManager Character;
        public SpellItem Spell;
    }

    public struct PlayerCastSpellEvent
    {
        public CharacterManager Character;
        public SpellItem Spell;
    }
}
