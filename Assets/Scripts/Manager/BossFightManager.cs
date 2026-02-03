using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
namespace HT
{
    [Hotfix]
    public class BossFightManager : MonoBehaviour
    {
        public EnemyManager boss;
        public List<FogWall> fogWalls = new List<FogWall>();
        public PassThroughFogWall passThroughFogWall;

        public bool bossFightIsActive;
        public bool bossHasBeenAwakened;
        public bool bossHasBeenDefeated;
        void OnEnable()
        {
            EventCenter.Instance.AddEventListener<CharacterManager>(E_EventType.E_Character_Death, OnCharacterDeath);
        }

        void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener<CharacterManager>(E_EventType.E_Character_Death, OnCharacterDeath);
        }

        //Boss死亡处理
        private void OnCharacterDeath(CharacterManager boss)
        {
            if (boss is EnemyManager)
            {
                if ((boss as EnemyManager).enemyStatsManager.isBoss)
                    BossHasBeenDefeated();
            }
        }

        /// <summary>
        /// 激活Boss战
        /// </summary> <summary>
        public void ActivateBossFight()
        {
            if (bossHasBeenDefeated) return;
            if (bossFightIsActive) return;
            if( !boss.isActiveAndEnabled) return;
            bossFightIsActive = true;
            bossHasBeenAwakened = true;

            // 开雾墙
            foreach (var fogWall in fogWalls)
            {
                if (fogWall != null) fogWall.ActivateFogWall(true);
            }

            // 显示 HUD（用快照，不传 EnemyManager）
            PublishBossHud();
        }
        /// <summary>
        /// Boss被打败
        /// </summary>
        public void BossHasBeenDefeated()
        {
            if (!bossFightIsActive) return;

            bossFightIsActive = false;
            bossHasBeenDefeated = true;
            
            // 关雾墙
            foreach (var fogWall in fogWalls)
            {
                if (fogWall != null) fogWall.ActivateFogWall(false);
            }

            // 隐藏 HUD
            EventCenter.Instance.EventTrigger<BossHudData?>(E_EventType.E_BossHudChanged, null);
        }


        public void PublishBossHud()
        {
            if (boss == null || boss.enemyStatsManager == null || boss.enemyBossManager == null)
                 return;

            var data = new BossHudData(
                boss.enemyBossManager.bossName,
                boss.enemyStatsManager.currentHealth,
                boss.enemyStatsManager.maxHealth
            );

            EventCenter.Instance.EventTrigger<BossHudData?>(E_EventType.E_BossHudChanged, data);
        }
    }
}

