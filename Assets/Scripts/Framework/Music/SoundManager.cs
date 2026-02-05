using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

namespace HT
{
    /// <summary>
    /// 声音事件响应模块：集中监听全局事件并调用 MusicMgr 播放对应音效。
    /// </summary>
    public class SoundManager : SingletonAutoMono<SoundManager>
    {
        [Header("音效资源名")]
        public List<string> damageSoundList = new List<string> { "damage1", "damage2" };
        public List<string> slashSoundList = new List<string> { "slash1", "slash2", "slash3" };
        public string playerHealSound = "heal";
        public string bossPhaseShiftSound = "phase_shift";
        public string fireBallCastSound = "fireball";

        private void OnEnable()
        {
            GameArchitecture.Interface.RegisterEvent<BombHitEvent>(e => BombHitSound(e.BombTransform))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            GameArchitecture.Interface.RegisterEvent<SlashEvent>(e => SlashSound(e.Character))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            GameArchitecture.Interface.RegisterEvent<FireBallHitEvent>(e => FireBallHitSound(e.HitPoint))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            GameArchitecture.Interface.RegisterEvent<CharacterDamageEvent>(e => OnPlayerDamage(e.HitPoint))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            GameArchitecture.Interface.RegisterEvent<BossPhaseShiftEvent>(e => OnBossPhaseShift(e.Boss))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            GameArchitecture.Interface.RegisterEvent<PlayerDrinkPotionEvent>(e => OnPlayerHeal(e.Player))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            GameArchitecture.Interface.RegisterEvent<PerformInteractionEvent>(e => InteractSound(e.Target))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            GameArchitecture.Interface.RegisterEvent<PlayerCastSpellEvent>(e => SpellCastSound((e.Character, e.Spell)))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        public void Init()
        {
            print("开启声音管理器");
        }
        //播放音效
        private void PlaySound(string soundName, bool isLoop = false)
        {
            if (string.IsNullOrEmpty(soundName))
                return;
            MusicMgr.Instance.PlaySound(soundName, isLoop);
        }

        //交互音效事件
        private void InteractSound(Interactable interactable)
        {
            if (interactable == null) return;
            PlaySound(interactable.soundEffectName);
        }
        //挥砍音效事件
        private void SlashSound(CharacterManager character)
        {
            int randomIndex = Random.Range(0, slashSoundList.Count);
            PlaySound(slashSoundList[randomIndex]);
        }

        //受伤音效事件
        private void OnPlayerDamage(Vector3 hitPoint)
        {
            int randomIndex = Random.Range(0, damageSoundList.Count);
            PlaySound(damageSoundList[randomIndex]);
        }

        //治疗音效事件
        private void OnPlayerHeal(PlayerManager player)
        {
            PlaySound(playerHealSound);
        }

        // Boss 二阶段切换音效事件
        private void OnBossPhaseShift(EnemyManager boss)
        {
            // 仅在 Boss 不为空时播放；如需区分不同 Boss，可在此根据 boss.enemyBossManager.bossName 切换 key
            if (boss == null) return;
            PlaySound(bossPhaseShiftSound, isLoop: false);
        }

        private void SpellCastSound((CharacterManager, SpellItem) args)
        {
            if (args.Item2 is HealingSpell)
            {
                PlaySound(playerHealSound);
            }
            else if (args.Item2 is ProjectileSpell)
            {
                PlaySound(fireBallCastSound);
            }
        }

        private void FireBallHitSound(Vector3 hitPoint)
        {
            PlaySound("fireball_hit");
        }
        private void BombHitSound(Transform hitPoint)
        {
            PlaySound("bomb_explosion");
        }


    }
}
