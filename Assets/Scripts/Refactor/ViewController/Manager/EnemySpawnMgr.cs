using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

public class EnemySpawnMgr : ARPGController
{
    public List<Transform> enemyPosList = new List<Transform>();
    public EnemyManager boss;
    public IPoolSystem pool;
    [SerializeField] private List<string> enemys = new List<string>();
    [SerializeField] private string bossEnemy;

    void Start()
    {
        pool = this.GetSystem<IPoolSystem>();

        this.RegisterEvent<SpawnEnemyEvent>(_ => SpawnEnemy())
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<SpawnBossEvent>(_ => SpawnBoss())
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void SpawnEnemy()
    {
        foreach (var pos in enemyPosList)
        {
            GameObject enemyObj = pool.Spawn(enemys[Random.Range(0, enemys.Count)], pos.position, Quaternion.identity);

            // 配置巡逻点：将 pos 的子对象填入 PatrolStateHumanoid 列表
            var patrol = enemyObj.GetComponentInChildren<PatrolStateHumanoid>();
            if (patrol != null)
            {
                patrol.patrolDestinationList.Clear();
                foreach (Transform child in pos)
                    patrol.patrolDestinationList.Add(child);

                // 重置巡逻进度，防止复用回收对象时沿用上次状态
                patrol.patrolDestinationIndex = -1;
                patrol.hasPatrolDestination = false;
                patrol.currentPatrolDestination = null;
                patrol.patorlCompleted = false;
                patrol.endOfPatrolTimer = 0;
            }

        }
    }

    private void SpawnBoss()
    {
        boss.gameObject.SetActive(true);
    }
}
