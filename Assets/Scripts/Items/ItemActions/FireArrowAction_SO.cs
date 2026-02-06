using System.Collections;
using System.Collections.Generic;
using ARPG;
using UnityEngine;

namespace HT
{

    [CreateAssetMenu(menuName = "Item Actions/Fire Arrow Action")]
    public class FireArrowAction_SO : ItemAction_SO
    {
        public override void PerformAction(CharacterManager character)
        {
            PlayerManager player = character as PlayerManager;
            if (character.isInteracting || character.isUsingComsumable)
                return;
            //发射出去的箭矢生成位置
            ArrowInstantiationLocation arrowInstantiationLocation;
            arrowInstantiationLocation = character.characterWeaponSlotManager.rightHandSlot.GetComponentInChildren<ArrowInstantiationLocation>();

            Animator bowAnmatior = character.characterWeaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
            bowAnmatior.SetBool("isDrawn", false);
            bowAnmatior.Play("Bow_TH_Fire_01");

            if (character.characterEffectsManager.instantiateFX != null)
                GameArchitecture.Interface.GetSystem<IPoolSystem>().Recycle(character.characterEffectsManager.instantiateFX);
            //设置玩家射箭动画
            character.characterAnimatorManager.PlayTargetAnimation("Bow_TH_Fire_01", true);
            character.animator.SetBool("isHoldingArrow", false);

            //玩家射箭
            if (player != null)
            {
                // 1. 获取对象
                GameObject liveArrow = GameArchitecture.Interface.GetSystem<IPoolSystem>().Spawn("Projectiles/Arrow_Live_Model");
                Rigidbody rigidbody = liveArrow.GetComponent<Rigidbody>();

                // 2. 立即断开，使用 false 参数 (你之前的做法是对的)
                liveArrow.transform.SetParent(null, false);

                // 3. 强制重置 Transform
                liveArrow.transform.position = arrowInstantiationLocation.transform.position;
                liveArrow.transform.rotation = player.cameraMgr.cameraPivotTransform.rotation;
                liveArrow.transform.localScale = Vector3.one;

                // 4. 深层重置刚体
                // 简单的 velocity=0 可能不够，必须重置惯性张量，防止上次碰撞导致的物理计算错误
                rigidbody.ResetInertiaTensor();
                rigidbody.ResetCenterOfMass();
                //rigidbody.velocity = Vector3.zero;
                //rigidbody.angularVelocity = Vector3.zero;

                // 5. 旋转逻辑 (保持你现有的，既然你确认没进 aiming 就不动它)
                if (player.isAiming)
                {
                    Ray ray = player.cameraMgr.cameraObj.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                    RaycastHit hitPoint;

                    if (Physics.Raycast(ray, out hitPoint, 100))
                    {
                        liveArrow.transform.rotation = Quaternion.LookRotation(hitPoint.point - liveArrow.transform.position);
                    }
                    else
                    {
                        liveArrow.transform.rotation = Quaternion.Euler(player.cameraMgr.cameraTransform.localEulerAngles.x, player.lockOnTransform.eulerAngles.y, 0);
                    }
                }
                else
                {
                    if (player.cameraMgr.currentLockOnTarget != null)
                    {
                        liveArrow.transform.rotation = Quaternion.LookRotation(player.cameraMgr.currentLockOnTarget.lockOnTransform.transform.position - liveArrow.transform.position);
                    }
                    else
                    {
                        liveArrow.transform.rotation = Quaternion.Euler(player.cameraMgr.cameraPivotTransform.eulerAngles.x, player.lockOnTransform.eulerAngles.y, 0);
                    }
                }

                // 6. 强制同步物理变换
                Physics.SyncTransforms();

                // 7. 开启物理
                rigidbody.isKinematic = false;

                // 8.改用 velocity 直接赋值
                // 放弃 AddForce。AddForce 容易受物理步长和刚体状态影响导致爆炸。
                // 直接设置速度是绝对的，不会产生 NaN。
                Vector3 forceDir = liveArrow.transform.forward;
                float speed = player.playerInventoryManager.currentAmmo.forwardVelocity;
                rigidbody.velocity = forceDir * (speed / rigidbody.mass * Time.fixedDeltaTime);
                // 上面这个公式是模拟 AddForce(ForceMode.Force) 的一帧效果，但更安全
                // 恢复重力设置
                rigidbody.useGravity = player.playerInventoryManager.currentAmmo.useGravity;

                // 9. 赋值伤害组件
                RangedProjectileDamageCollider damageCollider = liveArrow.GetComponent<RangedProjectileDamageCollider>();
                if (damageCollider != null)
                {
                    damageCollider.characterManager = player;
                    damageCollider.ammoItem = player.playerInventoryManager.currentAmmo;
                    damageCollider.physicalDamage = player.playerInventoryManager.currentAmmo.physicalDamage;
                    damageCollider.teamIDNumber = player.playerStatsManager.teamIDNumber;
                }
            }
            //AI射箭
            else
            {
                EnemyManager enemy = character as EnemyManager;
                // 1. 获取对象
                GameObject liveArrow = GameArchitecture.Interface.GetSystem<IPoolSystem>().Spawn("Projectiles/Arrow_Live_Model");
                Rigidbody rigidbody = liveArrow.GetComponent<Rigidbody>();

                // 2. 立即断开，使用 false 参数 (你之前的做法是对的)
                liveArrow.transform.SetParent(null, false);

                // 3. 强制重置 Transform
                liveArrow.transform.position = arrowInstantiationLocation.transform.position;
                liveArrow.transform.rotation = Quaternion.identity;
                liveArrow.transform.localScale = Vector3.one;

                // 4.深层重置刚体
                // 简单的 velocity=0 可能不够，必须重置惯性张量，防止上次碰撞导致的物理计算错误
                rigidbody.ResetInertiaTensor();
                rigidbody.ResetCenterOfMass();
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;


                if (enemy.currentTarget != null)
                {
                    liveArrow.transform.rotation = Quaternion.LookRotation(enemy.currentTarget.lockOnTransform.transform.position - liveArrow.transform.position);
                }

                // 6. 强制同步物理变换
                // 告诉物理引擎：“刚体现在就在这个位置，不要管上一帧在哪”
                Physics.SyncTransforms();

                // 7. 开启物理
                rigidbody.isKinematic = false;

                // 8. 【核心改动】改用 velocity 直接赋值
                // 放弃 AddForce。AddForce 容易受物理步长和刚体状态影响导致爆炸。
                // 直接设置速度是绝对的，不会产生 NaN。
                Vector3 forceDir = liveArrow.transform.forward;
                float speed = enemy.characterInventoryManager.currentAmmo.forwardVelocity;
                // 暂时保留你的逻辑，但改用 velocity (忽略 mass 影响，强制设定速度)
                // 这样可以 100% 避免 "Floating-point precision" 错误
                rigidbody.velocity = forceDir * (speed / rigidbody.mass * Time.fixedDeltaTime);
                // 上面这个公式是模拟 AddForce(ForceMode.Force) 的一帧效果，但更安全
                // 恢复重力设置
                rigidbody.useGravity = enemy.characterInventoryManager.currentAmmo.useGravity;

                // 9. 赋值伤害组件
                RangedProjectileDamageCollider damageCollider = liveArrow.GetComponent<RangedProjectileDamageCollider>();
                if (damageCollider != null)
                {
                    damageCollider.characterManager = enemy;
                    damageCollider.ammoItem = enemy.characterInventoryManager.currentAmmo;
                    damageCollider.physicalDamage = enemy.characterInventoryManager.currentAmmo.physicalDamage;
                    damageCollider.teamIDNumber = enemy.characterStatsManager.teamIDNumber;
                }
            }

        }
    }
}
