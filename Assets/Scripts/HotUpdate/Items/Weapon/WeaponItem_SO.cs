
using UnityEngine;

namespace ARPG
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem_SO : Item_SO
    {
        public GameObject modelPrefab;
        public bool isUnarmed;
        [Header("Animator Override")]
        public AnimatorOverrideController weaponOverrideController;
        [Header("武器类型")]
        public E_WeaponType weaponType;

        [Header("伤害属性")]
        public int physicalDamage;
        public int fireDamage;
        [Header("伤害修正")]
        public float lightAttackDamageModifier = 1f;
        public float heavyAttackDamageModifier = 2f;
        //Running Attack
        //Jumping Attack
        public int criticalDamageMultiplier = 4;
        public float guardBreakModifier = 1;
        [Header("Poise 属性")]
        public float poiseBreak;
        public float offensivePoiseBonus;
        [Header("吸收伤害")]
        public float physicalBlockingDamageAbsorption;
        public float fireBlockingDamageAbsorption;
        [Header("稳定性")]
        public int stability;

        [Header("耐力消耗")]
        public int baseStamina;
        public float lightAttackStaminaMultiplier;
        public float heavyAttackStaminaMultiplier;

        [Header("Item Action")]
        public ItemAction_SO oh_hold_Mouse_L_Action;
        public ItemAction_SO oh_tap_Mouse_L_Action;
        public ItemAction_SO oh_hold_Mouse_R_Action;
        public ItemAction_SO oh_tap_Mouse_R_Action;
        public ItemAction_SO oh_hold_Q_Action;
        public ItemAction_SO oh_tap_Q_Action;
        public ItemAction_SO oh_hold_R_Action;
        public ItemAction_SO oh_tap_R_Action;

        [Header("Two Handed Item Action")]
        public ItemAction_SO th_hold_Mouse_L_Action;
        public ItemAction_SO th_tap_Mouse_L_Action;
        public ItemAction_SO th_hold_Mouse_R_Action;
        public ItemAction_SO th_tap_Mouse_R_Action;
        public ItemAction_SO th_hold_Q_Action;
        public ItemAction_SO th_tap_Q_Action;
        public ItemAction_SO th_hold_R_Action;
        public ItemAction_SO th_tap_R_Action;
    }
}

