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
            if (player.cameraMgr.currentLockOnTarget != null)
            {
                projectile.transform.rotation = Quaternion.LookRotation(player.cameraMgr.currentLockOnTarget.lockOnTransform.transform.position - player.lockOnTransform.transform.position);
            }
            else
            {
                projectile.transform.rotation = Quaternion.Euler(player.cameraMgr.cameraPivotTransform.eulerAngles.x, player.lockOnTransform.eulerAngles.y, 0);
            }

            SpellDamageCollider damageCollider = projectile.GetComponent<SpellDamageCollider>();
            damageCollider.fireDamage = (int)ps.baseDamage;
            damageCollider.teamIDNumber = player.playerStatsManager.teamIDNumber;
            damageCollider.characterManager = player;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.AddForce(projectile.transform.forward * ps.projectileForwardVelocity);
            rb.AddForce(projectile.transform.up * ps.projectileUpWardVelocity);
        }
    }
}
