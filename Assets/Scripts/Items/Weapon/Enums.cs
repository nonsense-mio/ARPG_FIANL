

namespace ARPG
{
    public enum E_WeaponType
    {
        PyroMancyCaster,
        FaithCaster,
        SpellCaster,
        Unarmed,
        StraightSword,
        SmallShield,
        Shield,
        Bow,
    }

    public enum E_AmmoType
    {
        Arrow,
        Bolt,
    }

    public enum E_AttackType
    {
        LightAttack,
        HeavyAttack,
        
        //可以细分 TODO
        //LightAttack_01
        //LightAttack_02
        //HeavyAttack_01
        //HeavyAttack_02
    }

    public enum E_AICombatStyle
    {
        SwordAndShield,
        Archer,
    }
    public enum E_AIAttackActionType
    {
        MeleeAttackAction,
        MagicAttackAction,
        RangedAttackAction,
    }
    public enum E_DamageType
    {
        Physical,
        Fire,
    }
}

