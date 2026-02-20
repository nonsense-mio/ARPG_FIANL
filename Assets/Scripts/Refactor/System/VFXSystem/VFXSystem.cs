using Framework;
using ARPG;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// VFX 特效系统实现 - 订阅事件播放对应特效
    /// 替代原 VFXMgr (SingletonAutoMono)
    /// </summary>
    public class VFXSystem : AbstractSystem, IVFXSystem
    {
        private const string HealFXPath = "FX/Recover";
        private const string BloodFXPath = "FX/Blood";
        private const string SlashFXPath = "FX/Slash";
        private const string SlashBossFXPath = "FX/Slash_Boss";
        private const string PhaseShiftFXPath = "FX/PhaseShift";
        private const string FireBallImpactFXPath = "FX/FireBallImpact";
        private const string BombFXPath = "FX/BombFX";

        private IPoolSystem poolSystem;

        protected override void OnInit()
        {
            poolSystem = this.GetSystem<IPoolSystem>();

            this.RegisterEvent<BombHitEvent>(e => BombHitFX(e.BombTransform));
            this.RegisterEvent<SlashEvent>(e => OnSlash(e.Character));
            this.RegisterEvent<FireBallHitEvent>(e => FireBallHitFX(e.HitPoint));
            this.RegisterEvent<CharacterDamageEvent>(e => OnCharacterDamage(e.HitPoint));
            this.RegisterEvent<BossPhaseShiftEvent>(e => OnBossPhaseShift(e.Boss));
            this.RegisterEvent<PlayerDrinkPotionEvent>(e => OnPlayerHeal(e.Player));
            this.RegisterEvent<PlayerCastSpellEvent>(e => CastSpellFX(e.Character, e.Spell));
            this.RegisterEvent<PlayerWarmUpSpellEvent>(e => WarmUpSpellFX(e.Character, e.Spell));
            this.RegisterEvent<PlayerPosionEvent>(e => OnPoison(e.Character));
        }

        private void OnPoison(CharacterManager player)
        {
            GameObject posion = poolSystem.Spawn("FX/Poison");
            posion.transform.SetParent(player.transform);
            posion.transform.localPosition = player.lockOnTransform.localPosition;
            posion.transform.localRotation = Quaternion.identity;
        }

        private void OnPlayerHeal(PlayerManager player)
        {
            GameObject fx = poolSystem.Spawn(HealFXPath);
            fx.transform.SetParent(player.transform);
            fx.transform.localPosition = Vector3.zero;
            fx.transform.localRotation = Quaternion.Euler(-90,0,0);
        }

        private void OnCharacterDamage(Vector3 hitPoint)
        {
            GameObject blood = poolSystem.Spawn(BloodFXPath);
            blood.transform.position = hitPoint;
            blood.transform.rotation = Quaternion.identity;
            blood.transform.localScale = Vector3.one * 0.5f;
        }

        private void OnSlash(CharacterManager character)
        {
            string currentSlashPath = SlashFXPath;
            if (character is EnemyManager)
            {
                if ((character as EnemyManager).enemyStatsManager.hasPublishedPhaseShift)
                {
                    currentSlashPath = SlashBossFXPath;
                }
            }
            else
            {
                currentSlashPath = SlashFXPath;
            }
            GameObject slash = poolSystem.Spawn(currentSlashPath);
            slash.transform.position = character.transform.position + character.transform.up * 1.0f;
            slash.transform.rotation = character.transform.rotation;
            slash.transform.localScale = Vector3.one * 0.5f;
        }

        private void OnBossPhaseShift(EnemyManager boss)
        {
            GameObject phaseShiftFX = poolSystem.Spawn(PhaseShiftFXPath);
            phaseShiftFX.transform.SetParent(boss.transform);
            phaseShiftFX.transform.localPosition = Vector3.zero + Vector3.up * 0.2f;
            phaseShiftFX.transform.localRotation = Quaternion.identity;
        }

        private void WarmUpSpellFX(CharacterManager character, SpellItem spell)
        {
            if (spell is HealingSpell)
            {
                GameObject fx = poolSystem.Spawn(spell.spellWarmUpFXName);
                fx.transform.SetParent(character.transform);
                fx.transform.localPosition = Vector3.zero + Vector3.up * 2f;
            }
            else if (spell is ProjectileSpell)
            {
                GameObject fx = poolSystem.Spawn(spell.spellWarmUpFXName);
                if (character.isUsingRightHand)
                    fx.transform.SetParent(character.characterWeaponSlotManager.rightHandSlot.transform);
                else
                    fx.transform.SetParent(character.characterWeaponSlotManager.leftHandSlot.transform);

                fx.transform.localPosition = Vector3.zero;
                fx.transform.localScale = Vector3.one * 100;
            }
        }

        private void CastSpellFX(CharacterManager character, SpellItem spell)
        {
            if (spell is HealingSpell)
            {
                GameObject fx = poolSystem.Spawn(spell.spellCastFXName);
                fx.transform.SetParent(character.transform);
                fx.transform.localPosition = Vector3.zero;
                fx.transform.localScale = Vector3.one * 0.5f;
            }
            // ProjectileSpell 弹道逻辑已移至 SpellSystem
        }

        private void FireBallHitFX(Vector3 hitPoint)
        {
            GameObject fx = poolSystem.Spawn(FireBallImpactFXPath);
            fx.transform.position = hitPoint;
            fx.transform.rotation = Quaternion.LookRotation(-Vector3.up);
        }

        private void BombHitFX(Transform bombTransform)
        {
            GameObject fx = poolSystem.Spawn(BombFXPath);
            fx.transform.position = bombTransform.position;
            fx.transform.rotation = Quaternion.identity;
        }
    }
}
