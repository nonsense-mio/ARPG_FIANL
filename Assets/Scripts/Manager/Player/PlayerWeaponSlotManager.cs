using System.Collections;
using System.Collections.Generic;
using ARPG;
using UnityEngine;

namespace HT
{
    public class PlayerWeaponSlotManager : CharacterWeaponSlotManager
    {
        PlayerManager player;

        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            player = characterMgr as PlayerManager;
        }
 
        /// <summary>
        /// 在武器插槽中加载武器 （装备武器）
        /// </summary>
        /// <param name="weaponItem">武器对象</param>
        /// <param name="isLeft">左手还是右手</param>
        public override void LoadWeaponOnSlot(WeaponItem_SO weaponItem,bool isLeft)
        {
            if(weaponItem != null)
            {
                if (isLeft)
                {
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    
                    //player.playerAnimatorManager.PlayTargetAnimation(weaponItem.offHandIdleAnimation, false,true);
                }
                else
                {
                    if (player.inputMgr.twoHandFlag)
                    {
                        //把当前左手上的武器移到背后
                        backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                        //销毁左手上的武器模型
                        leftHandSlot.UnloadWeaponAndDestory();
                        
                        player.playerAnimatorManager.PlayTargetAnimation("Left Arm Empty", false,true);
                    }
                    else
                    {
                        //如果不是双手武器 销毁背上的装备
                        backSlot.UnloadWeaponAndDestory();
                    }
                    rightHandSlot.currentWeapon = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();


                    //角色动画控制器替换为武器对应的动画控制器
                    player.animator.runtimeAnimatorController = weaponItem.weaponOverrideController;
                }
            }
            
        }

        /// <summary>
        /// 成功投掷炸弹
        /// </summary> <summary>
        public void SuccessfulyThrowFireBomb()
        {
            //Destroy(player.playerEffectsManager.instantiatedFXModel);
            //扔出炸弹时将之前的模型回收到对象池中
            GameArchitecture.Interface.GetSystem<PoolSystem>().Recycle(player.playerEffectsManager.instantiatedFXModel);
            BombConsumableItem_SO fireBombItem = player.playerInventoryManager.currentConsumable as BombConsumableItem_SO;
            //GameObject activeModelBomb = Instantiate(fireBombItem.liveBombModel,rightHandSlot.transform.position,player.cameraHandler.cameraPivotTransform.rotation);
            //从池子中获取扔出去的炸弹模型
            GameObject activeModelBomb = GameArchitecture.Interface.GetSystem<PoolSystem>().Spawn(fireBombItem.liveBombModelName);
            activeModelBomb.transform.position = rightHandSlot.transform.position;
            activeModelBomb.transform.rotation = player.cameraMgr.cameraPivotTransform.rotation;
            activeModelBomb.transform.rotation = Quaternion.Euler (player.cameraMgr.cameraPivotTransform.eulerAngles.x,player.cameraMgr.cameraPivotTransform.eulerAngles.y,0);
            BombDamageCollider damageCollider = activeModelBomb.GetComponent<BombDamageCollider>();
            //设置炸弹伤害
            damageCollider.explosionDamage = fireBombItem.baseDamage;
            damageCollider.explosionSplashDamage = fireBombItem.explosiveDamage;
            //为炸弹添加力使其飞出
            damageCollider.bombRigidbody.AddForce(activeModelBomb.transform.forward * fireBombItem.forwardVelocity);
            damageCollider.bombRigidbody.AddForce(activeModelBomb.transform.up * fireBombItem.upwardVelocity);
            //设置队伍ID
            damageCollider.teamIDNumber = player.playerStatsManager.teamIDNumber;
            damageCollider.characterManager = player;

        }

    
    }
}

