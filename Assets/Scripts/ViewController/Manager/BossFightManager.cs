using System.Collections.Generic;
using UnityEngine;
using XLua;
using Framework;
using ARPG;

namespace ARPG
{
    [Hotfix]
    public class BossFightManager : ARPGController
    {
        [SerializeField] private string bossID;
        public string BossID => bossID;
        public EnemyManager boss;
        public List<FogWall> fogWalls = new List<FogWall>();
        public PassThroughFogWall passThroughFogWall;

        public bool bossFightIsActive;
        public bool bossHasBeenAwakened;
        public bool bossHasBeenDefeated;

        void Start()
        {
            // 从 SceneStateModel 读取 Boss 击败状态
            var sceneStateModel = this.GetModel<ISceneStateModel>();
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
            this.RegisterEvent<CharacterDeathEvent>(e => OnCharacterDeath(e.Character))
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
        /// </summary>
        public void ActivateBossFight()
        {
            this.SendCommand(new ActivateBossFightCommand(this));
        }

        /// <summary>
        /// Boss被打败
        /// </summary>
        public void BossHasBeenDefeated()
        {
            this.SendCommand(new BossDefeatedCommand(this));
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

            this.SendEvent(new BossHudChangedEvent { Data = data });
        }
    }
}

