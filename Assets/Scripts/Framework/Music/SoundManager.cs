using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
            EventCenter.Instance.AddEventListener<Transform>(E_EventType.E_BombHit, BombHitSound);
            EventCenter.Instance.AddEventListener<CharacterManager>(E_EventType.E_Slash, SlashSound);
            EventCenter.Instance.AddEventListener<Vector3>(E_EventType.E_FireBallHit, FireBallHitSound);
            EventCenter.Instance.AddEventListener<Vector3>(E_EventType.E_Character_Damage, OnPlayerDamage);
            EventCenter.Instance.AddEventListener<EnemyManager>(E_EventType.E_BossPhaseShift, OnBossPhaseShift);
            EventCenter.Instance.AddEventListener<PlayerManager>(E_EventType.E_Player_DrinkPotion, OnPlayerHeal);
            EventCenter.Instance.AddEventListener<Interactable>(E_EventType.E_Perform_Interaction, InteractSound);
            EventCenter.Instance.AddEventListener<(CharacterManager, SpellItem)>(E_EventType.E_Player_CastSpell, SpellCastSound);

        }

        private void OnDestroy()
        {
            EventCenter.Instance.RemoveEventListener<Transform>(E_EventType.E_BombHit, BombHitSound);
            EventCenter.Instance.RemoveEventListener<CharacterManager>(E_EventType.E_Slash, SlashSound);
            EventCenter.Instance.RemoveEventListener<Vector3>(E_EventType.E_FireBallHit, FireBallHitSound);
            EventCenter.Instance.RemoveEventListener<Vector3>(E_EventType.E_Character_Damage, OnPlayerDamage);
            EventCenter.Instance.RemoveEventListener<EnemyManager>(E_EventType.E_BossPhaseShift, OnBossPhaseShift);
            EventCenter.Instance.RemoveEventListener<PlayerManager>(E_EventType.E_Player_DrinkPotion, OnPlayerHeal);
            EventCenter.Instance.RemoveEventListener<Interactable>(E_EventType.E_Perform_Interaction, InteractSound);
            EventCenter.Instance.RemoveEventListener<(CharacterManager, SpellItem)>(E_EventType.E_Player_CastSpell, SpellCastSound);

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
