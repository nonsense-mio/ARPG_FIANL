using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 法术系统 - 负责弹道类法术的生命周期
    /// 包括：生成弹道游戏对象、注入战斗参数、施加物理推力
    /// </summary>
    public class SpellSystem : AbstractSystem, ISpellSystem
    {
        private IPoolSystem poolSystem;

        protected override void OnInit()
        {
            poolSystem = this.GetSystem<IPoolSystem>();
            this.RegisterEvent<PlayerCastSpellEvent>(e => OnCastSpell(e.Character, e.Spell));
        }

        private void OnCastSpell(CharacterManager character, SpellItem spell)
        {
            if (!(spell is ProjectileSpell ps)) return;

            PlayerManager player = character as PlayerManager;
            GameObject projectile = poolSystem.Spawn(spell.spellCastFXName);

            Transform handSlot = character.isUsingRightHand
                ? character.characterWeaponSlotManager.rightHandSlot.transform
                : character.characterWeaponSlotManager.leftHandSlot.transform;

            projectile.transform.position = handSlot.position;

            // 锁定目标时直接瞄准目标中心点（lockOnTransform），避免近距离时因摄像机角度偏差导致火球打地
            if (player.cameraMgr.currentLockOnTarget != null)
            {
                Vector3 aimTarget = player.cameraMgr.currentLockOnTarget.lockOnTransform.position;
                Vector3 aimDir = (aimTarget - handSlot.position).normalized;
                projectile.transform.rotation = Quaternion.LookRotation(aimDir);
            }
            else
            {
                projectile.transform.rotation = Quaternion.Euler(
                    player.cameraMgr.cameraTransform.eulerAngles.x,
                    player.cameraMgr.cameraTransform.eulerAngles.y,
                    0);
            }

            SpellDamageCollider damageCollider = projectile.GetComponent<SpellDamageCollider>();
            damageCollider.fireDamage = (int)ps.baseDamage;
            damageCollider.teamIDNumber = player.playerStatsManager.teamIDNumber;
            damageCollider.characterManager = player;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.AddForce(projectile.transform.forward * ps.projectileForwardVelocity);
            // 非锁定模式需向上弧线力补偿摄像机平视偏差；锁定模式已直接瞄准目标，无需叠加
            if (player.cameraMgr.currentLockOnTarget == null)
                rb.AddForce(projectile.transform.up * ps.projectileUpWardVelocity);
        }
    }
}
