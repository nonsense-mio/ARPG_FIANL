using System.Collections.Generic;
using UnityEngine;
using XLua;
using Framework;
using ARPG;

namespace HT
{
    [Hotfix]
    public class BossFightManager : MonoBehaviour
    {
        [SerializeField] private string bossID;
        public EnemyManager boss;
        public List<FogWall> fogWalls = new List<FogWall>();
        public PassThroughFogWall passThroughFogWall;

        public bool bossFightIsActive;
        public bool bossHasBeenAwakened;
        public bool bossHasBeenDefeated;

        void Start()
        {
            // 从 SceneStateModel 读取 Boss 击败状态
            var sceneStateModel = GameArchitecture.Interface.GetModel<ISceneStateModel>();
            bossHasBeenDefeated = sceneStateModel.IsBossDefeated(bossID);

            // 如果 Boss 已被击败，禁用 Boss 和相关组件
            if (bossHasBeenDefeated)
            {
                if (boss != null) boss.gameObject.SetActive(false);
                foreach (var fogWall in fogWalls)
                {
                    if (fogWall != null) fogWall.ActivateFogWall(false);
                }
            }
        }

        void OnEnable()
        {
            GameArchitecture.Interface.RegisterEvent<CharacterDeathEvent>(e => OnCharacterDeath(e.Character))
                .UnRegisterWhenGameObjectDisabled(gameObject);
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
            GameArchitecture.Interface.SendEvent(new BossHudChangedEvent { Data = null });

            // 更新 SceneStateModel Boss 击败状态
            var sceneStateModel = GameArchitecture.Interface.GetModel<ISceneStateModel>();
            sceneStateModel.SetBossDefeated(bossID, true);
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

            GameArchitecture.Interface.SendEvent(new BossHudChangedEvent { Data = data });
        }
    }
}

