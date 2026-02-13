using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

public class EnemySpawnMgr : ARPGController
{
    public List<EnemyManager> enemyList = new List<EnemyManager>();
    public EnemyManager boss;

    void Start()
    {
        this.RegisterEvent<SpawnEnemyEvent>(_ => SpawnEnemy())
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<SpawnBossEvent>(_ => SpawnBoss())
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void SpawnEnemy()
    {
        foreach (var enemy in enemyList)
        {
            enemy.gameObject.SetActive(true);
        }
    }

    private void SpawnBoss()
    {
        boss.gameObject.SetActive(true);
    }
}
