using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class CharacterInventoryManager : MonoBehaviour
    {
        protected CharacterManager character;
        [Header("当前在使用的物品")]
        public Item_SO currentItemBeingUsed;
        [Header("快速插槽当前物品")]
        public SpellItem currentSpell;
        public ConsumableItem_SO currentConsumable;
        //玩家当前右手持有的武器
        public WeaponItem_SO rightWeapon;
        //玩家当前左手持有的武器
        public WeaponItem_SO leftWeapon;

        public RangedAmmoItem_SO currentAmmo;
        [Header("当前装备")]
        public HelmetEquipment currentHelmet;
        public BodyEquipment currentBody;
        public LegEquipment currentLegs;
        public HandEquipment currentHands;


        [Header("快速插槽物品数组")]
        public WeaponItem_SO[] weaponInRightHandSlots = new WeaponItem_SO[1];
        public WeaponItem_SO[] weaponInLeftHandSlots = new WeaponItem_SO[1];
        public ConsumableItem_SO[] consumableSlots = new ConsumableItem_SO[1];
        public SpellItem[] spellSlots = new SpellItem[1];
        //左右手当前武器索引
        public int currentRightWeaponIndex = 0;
        public int currentLeftWeaponIndex = 0;
        public int currentConsumableIndex = 0;
        public int currentSpellIndex = 0;
        public virtual void Init(CharacterManager characterMgr)
        {
            character = characterMgr;
            currentRightWeaponIndex = 0;
            currentLeftWeaponIndex = 0;
            currentConsumableIndex = 0;
            currentSpellIndex = 0;
        }
        private void Start()
        {
            character.characterWeaponSlotManager.LoadBothWeaponsOnSlots();
        }
    }

}
