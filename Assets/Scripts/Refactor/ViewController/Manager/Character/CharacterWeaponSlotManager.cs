using ARPG;
using Framework;
using UnityEngine;

namespace ARPG
{
    public class CharacterWeaponSlotManager : ARPGController
    {
        protected CharacterManager character;
        public WeaponItem_SO unarmedWeapon;
        [Header("武器插槽")]
        public WeaponHolderSlot leftHandSlot;
        public WeaponHolderSlot rightHandSlot;
        protected WeaponHolderSlot backSlot;
        [Header("武器碰撞体")]
        public DamageCollider leftHandDamageCollider;
        public DamageCollider rightHandDamageCollider;

        [Header("手部IK目标")]
        public RightHandIKTarget rightHandIKTarget;
        public LeftHandIKTarget leftHandIKTarget;

        public virtual void Init(CharacterManager characterMgr)
        {
            character = characterMgr;
            LoadWeaponHolderSlots();
        }
        protected virtual void LoadWeaponHolderSlots()
        {
            //在该组件挂载的子对象中 寻找WeaponHolderSlot
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            //遍历每个插槽对象 给对应的插槽赋值
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots )
            {
                if (weaponSlot.isLeftHandSlot)
                {
                    leftHandSlot = weaponSlot;
                }
                else if (weaponSlot.isRightHandSlot)
                {
                    rightHandSlot = weaponSlot;
                }
                else if (weaponSlot.isBackSlot)
                {
                    backSlot = weaponSlot;
                }
            }
        }
   
        public virtual void LoadBothWeaponsOnSlots()
        {
            LoadWeaponOnSlot(character.characterInventoryManager.rightWeapon, false);
            LoadWeaponOnSlot(character.characterInventoryManager.leftWeapon, true);
        }

        public virtual void LoadWeaponOnSlot(WeaponItem_SO weaponItem,bool isLeft)
        {
            if(weaponItem != null)
            {
                if (isLeft)
                {
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    //character.characterAnimatorManager.PlayTargetAnimation(weaponItem.offHandIdleAnimation, false,true);
                }
                else
                {
                    if (character.isTwoHandingWeapon)
                    {
                        //把当前左手上的武器移到背后
                        backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                        //销毁左手上的武器模型
                        leftHandSlot.UnloadWeaponAndDestory();
                        
                        character.characterAnimatorManager.PlayTargetAnimation("Left Arm Empty", false,true);
                    }
                    else
                    {
                        //如果不是双手武器 销毁背上的装备
                        backSlot.UnloadWeaponAndDestory();
                    }
                    rightHandSlot.currentWeapon = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    //LoadTwoHandIKTargets(characterManager.isTwoHandingWeapon);
                    //角色动画控制器替换为武器对应的动画控制器
                    character.animator.runtimeAnimatorController = weaponItem.weaponOverrideController;
                }
            }
            // else
            // {
            //     weaponItem = unarmedWeapon;
            //     if (isLeft)
            //     {
            //         playerInventoryManager.leftWeapon = unarmedWeapon;
            //         leftHandSlot.currentWeapon = weaponItem;
            //         leftHandSlot.LoadWeaponModel(weaponItem);
            //         LoadLeftWeaponDamageCollider();
            //         playerAnimatorManager.PlayTargetAnimation(weaponItem.offHandIdleAnimation, false,true);
            //     }
            //     else
            //     {
            //         playerInventoryManager.rightWeapon = unarmedWeapon;
            //         rightHandSlot.currentWeapon = weaponItem;
            //         rightHandSlot.LoadWeaponModel(weaponItem);
            //         LoadRightWeaponDamageCollider();
            //         playerAnimatorManager.animator.runtimeAnimatorController = weaponItem.weaponOverrideController;
            //     }                  
            // }
            
        }
   

          
        /// <summary>
        /// 加载左手武器碰撞体
        /// </summary>
        protected virtual void LoadLeftWeaponDamageCollider()
        {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            //设置左手武器的伤害数值
            leftHandDamageCollider.physicalDamage = character.characterInventoryManager.leftWeapon.physicalDamage;
            leftHandDamageCollider.fireDamage = character.characterInventoryManager.leftWeapon.fireDamage;

            leftHandDamageCollider.characterManager = character;
            //设置队伍ID
            leftHandDamageCollider.teamIDNumber = character.characterStatsManager.teamIDNumber;
            leftHandDamageCollider.poiseBreak = character.characterInventoryManager.leftWeapon.poiseBreak;

        }
        /// <summary>
        /// 加载右手武器碰撞体
        /// </summary>
        protected virtual void LoadRightWeaponDamageCollider()
        {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            //设置右手武器的伤害数值
            rightHandDamageCollider.physicalDamage = character.characterInventoryManager.rightWeapon.physicalDamage;
            rightHandDamageCollider.fireDamage = character.characterInventoryManager.rightWeapon.fireDamage;

            rightHandDamageCollider.characterManager = character;
            //设置队伍ID
            rightHandDamageCollider.teamIDNumber = character.characterStatsManager.teamIDNumber;
            rightHandDamageCollider.poiseBreak = character.characterInventoryManager.rightWeapon.poiseBreak;

        }
   
        /// <summary>
        /// 加载双手武器的IK目标
        /// </summary>
        /// <param name="isTwoHandingWeapon"></param>
        public virtual void LoadTwoHandIKTargets(bool isTwoHandingWeapon)
        {
            rightHandIKTarget = rightHandSlot.currentWeaponModel.GetComponentInChildren<RightHandIKTarget>();
            leftHandIKTarget = rightHandSlot.currentWeaponModel.GetComponentInChildren<LeftHandIKTarget>();

            character.characterAnimatorManager.SetHandIKForWeapon(rightHandIKTarget,leftHandIKTarget,isTwoHandingWeapon);
        }


        /// <summary>
        /// 打开武器的碰撞体
        /// </summary>
        public virtual void OpenDamageCollider()
        {
            if (character.isUsingRightHand)
            {
                rightHandDamageCollider.EnableDamageCollider();
            }
            else if (character.isUsingLeftHand)
            {
                leftHandDamageCollider.EnableDamageCollider();
            }
            //触发挥砍事件
            this.SendEvent(new SlashEvent { Character = character });
        }
 
        /// <summary>
        /// 关闭武器碰撞体
        /// </summary>
        public virtual void CloseDamageCollider()
        {
            if(rightHandDamageCollider != null)
                rightHandDamageCollider.DisableDamageCollider();
            if(leftHandDamageCollider != null)
                leftHandDamageCollider.DisableDamageCollider();
        }
   
        #region 处理武器Poise相关  
        
        public virtual void GrandWeaponAttackingPoiseBonus()
        {
            WeaponItem_SO currentWeaponBeingUsed = character.characterInventoryManager.currentItemBeingUsed as WeaponItem_SO;
            character.characterStatsManager.totalPoiseDefense += currentWeaponBeingUsed.offensivePoiseBonus;
        }

        public virtual void ResetWeaponAttackingPoiseBonus()
        {
            character.characterStatsManager.totalPoiseDefense -= character.characterStatsManager.armorPoiseBonus;
        } 
        #endregion
   
   
    }
}

