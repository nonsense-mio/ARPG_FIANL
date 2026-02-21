using Framework;
using ARPG;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 声音事件系统实现 - 订阅事件并委托 IMusicSystem 播放对应音效
    /// 替代原 SoundManager (SingletonAutoMono)
    /// </summary>
    public class SoundSystem : AbstractSystem, ISoundSystem
    {
        private static readonly string[] DamageSounds = { "damage1", "damage2" };
        private static readonly string[] SlashSounds = { "slash1", "slash2", "slash3" };
        private const string HealSound = "heal";
        private const string PhaseShiftSound = "phase_shift";
        private const string FireballCastSound = "fireball";
        private const string FireballHitSound = "fireball_hit";
        private const string BombExplosionSound = "bomb_explosion";

        private IMusicSystem musicSystem;

        protected override void OnInit()
        {
            musicSystem = this.GetSystem<IMusicSystem>();

            this.RegisterEvent<BombHitEvent>(e => BombHitSound());
            this.RegisterEvent<SlashEvent>(e => SlashSound());
            this.RegisterEvent<FireBallHitEvent>(e => FireBallHitSound());
            this.RegisterEvent<CharacterDamageEvent>(e => OnPlayerDamage());
            this.RegisterEvent<BossPhaseShiftEvent>(e => OnBossPhaseShift(e.Boss));
            this.RegisterEvent<PlayerDrinkPotionEvent>(e => OnPlayerHeal());
            this.RegisterEvent<PerformInteractionEvent>(e => InteractSound(e.Target));
            this.RegisterEvent<PlayerCastSpellEvent>(e => SpellCastSound(e.Spell));
        }

        private void PlaySound(string soundName, bool isLoop = false)
        {
            if (string.IsNullOrEmpty(soundName))
                return;
            musicSystem.PlaySound(soundName, isLoop);
        }

        private void InteractSound(Interactable interactable)
        {
            if (interactable == null) return;
            PlaySound(interactable.soundEffectName);
        }

        private void SlashSound()
        {
            int randomIndex = Random.Range(0, SlashSounds.Length);
            PlaySound(SlashSounds[randomIndex]);
        }

        private void OnPlayerDamage()
        {
            int randomIndex = Random.Range(0, DamageSounds.Length);
            PlaySound(DamageSounds[randomIndex]);
        }

        private void OnPlayerHeal()
        {
            PlaySound(HealSound);
        }

        private void OnBossPhaseShift(EnemyManager boss)
        {
            if (boss == null) return;
            PlaySound(PhaseShiftSound);
        }

        private void SpellCastSound(SpellItem spell)
        {
            if (spell is HealingSpell)
            {
                PlaySound(HealSound);
            }
            else if (spell is ProjectileSpell)
            {
                PlaySound(FireballCastSound);
            }
        }

        private void FireBallHitSound()
        {
            PlaySound(FireballHitSound);
        }

        private void BombHitSound()
        {
            PlaySound(BombExplosionSound);
        }
    }
}
